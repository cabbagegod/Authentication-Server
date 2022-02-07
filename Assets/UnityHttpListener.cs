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

	Dictionary<AuthRequest, AuthResponse> authenticationQueue = new Dictionary<AuthRequest, AuthResponse>();

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

	//Runs on main thread, runs the requested calls to PlayFab
	void Update() {
		foreach(AuthRequest authRequest in authenticationQueue.Keys) {
            if(!authRequest.started) {
				CheckUserAuthentication(authRequest);
            }
        }
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
			Thread.Sleep(1000);
			//We assume this is the session ticket
			string sessionTicket = new StreamReader(context.Request.InputStream,
								context.Request.ContentEncoding).ReadToEnd();
			sessionTicket = HttpUtility.UrlDecode(sessionTicket);

			try {
				//Create an auth request and add it to the queue for the main thread to handle
				//(PlayFab calls aren't thread safe)
				AuthRequest authRequest = new AuthRequest(sessionTicket);
				authenticationQueue.Add(authRequest, null);

				while(authenticationQueue[authRequest] == null) {
					Thread.Sleep(100);
				}

				//Send Response
				HttpListenerResponse response = context.Response;

				string responseString = $"Is Authentic {authenticationQueue[authRequest].isAuthentic}";
				byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
				// Get a response stream and write the response to it.
				response.ContentLength64 = buffer.Length;

				System.IO.Stream output = response.OutputStream;
				output.Write(buffer, 0, buffer.Length);

				authenticationQueue.Remove(authRequest);
			} catch(Exception e) {
				Debug.Log(e.ToString());
			}
		}

		context.Response.Close();
	}

	async Task CheckUserAuthentication(AuthRequest authRequest) {
		authRequest.started = true;

		Authenticator authenticator = new Authenticator();

		bool authenticated = await authenticator.CheckUserAuthentication(authRequest.authToken);

		authenticationQueue[authRequest] = new AuthResponse(authenticated);
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
