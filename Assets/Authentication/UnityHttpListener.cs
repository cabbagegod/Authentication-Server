using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;

public class UnityHttpListener : MonoBehaviour {
	[SerializeField]
	private HttpListener listener;
	private Thread listenerThread;
	[SerializeField]
	private AuthenticationHandler authenticationHandler;

	void Start() {
		listener = new HttpListener();
		listener.Prefixes.Add("http://localhost:4444/");
		listener.Prefixes.Add("http://127.0.0.1:4444/");
		listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
		listener.Start();

		listenerThread = new Thread(ListenerThread);
		listenerThread.Start();
		Debug.Log("Server Started on " + "http://127.0.0.1:4444/");
	}

	private void ListenerThread() {
		while(true) {
			var result = listener.BeginGetContext(ListenerCallback, listener);
			result.AsyncWaitHandle.WaitOne();
		}
	}

	private void ListenerCallback(IAsyncResult result) {
		var context = listener.EndGetContext(result);

		if(context.Request.QueryString.AllKeys.Length > 0) {
			foreach(var key in context.Request.QueryString.AllKeys) {
				Debug.Log("Key: " + key + ", Value: " + context.Request.QueryString.GetValues(key)[0]);
			}
		}

		if(context.Request.HttpMethod == "POST") {
			//I don't remember the purpose for this sleep, needs testing to see if it's useless
			Thread.Sleep(100);
			//We assume this is the request
			string requestSerialized = new StreamReader(context.Request.InputStream,
								context.Request.ContentEncoding).ReadToEnd();
			requestSerialized = HttpUtility.UrlDecode(requestSerialized);

			ClientRequest clientRequest = JsonUtility.FromJson<ClientRequest>(requestSerialized);

			//This could definitely get cleaned up so much but async scares me so...
			if(clientRequest.requestType == RequestType.Client) {
				try {
					authenticationHandler.AddClientRequestToQueue(clientRequest);

					while(authenticationHandler.ResponseIsNull(clientRequest)) {
						Thread.Sleep(100);
					}

					ClientResponse clientResponse = authenticationHandler.GetClientResponse(clientRequest);

					//Send Response
					HttpListenerResponse response = context.Response;

					string responseString = JsonUtility.ToJson(clientResponse);
					byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
					// Get a response stream and write the response to it.
					response.ContentLength64 = buffer.Length;

					System.IO.Stream output = response.OutputStream;
					output.Write(buffer, 0, buffer.Length);

					authenticationHandler.RemoveClientRequestFromQueue(clientRequest);
				} catch(Exception e) {
					Debug.Log(e.ToString());
				}
			} else {
				try {
					ServerRequest serverRequest = JsonUtility.FromJson<ServerRequest>(requestSerialized);

					authenticationHandler.AddServerRequestToQueue(serverRequest);

					while(authenticationHandler.ResponseIsNull(serverRequest)) {
						Thread.Sleep(100);
					}

					ServerResponse serverResponse = authenticationHandler.GetServerResponse(serverRequest);

					//Send Response
					HttpListenerResponse response = context.Response;

					string responseString = JsonUtility.ToJson(serverResponse);
					byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
					// Get a response stream and write the response to it.
					response.ContentLength64 = buffer.Length;

					System.IO.Stream output = response.OutputStream;
					output.Write(buffer, 0, buffer.Length);

					authenticationHandler.RemoveServerRequestFromQueue(serverRequest);
				} catch(Exception ee) {
					Debug.Log(ee.ToString());
				}
			}
		}

		context.Response.Close();
	}

    private void OnDisable() {
		if(listener.IsListening)
			listener.Close();
    }

    private void OnApplicationQuit() {
		if(listener.IsListening)
			listener.Close();
	}
}
