using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace KyoshiStudios.Authentication {
    public class HttpRequestTest : MonoBehaviour {
        UnityEvent<string> responseEvent;

        // Start is called before the first frame update
        void Start() {
            responseEvent = new UnityEvent<string>();

            //Setup event to be called after the request is finished
            responseEvent.AddListener(HandleClientResponse);

            //Setup our client request
            ClientRequest clientRequest = new ClientRequest();
            //The client's playfab session ticket
            clientRequest.playfabSessionTicket = PlayFabLoginTest.sessionTicket;
            //The server IP that the client is attempting to connect to
            //This is used for security reasons and acts as a unique identifier, for this example we'll just use a dummy IP
            clientRequest.connectingIp = "1.1";
            //Mark the request as a client request, this is used so the Auth server can tell them apart easily
            clientRequest.requestType = RequestType.Client;
            //Serialize the ClientRequest so we can send it over a web request
            string serializedRequest = JsonUtility.ToJson(clientRequest);

            //Begin upload to auth server
            StartCoroutine(Upload("http://127.0.0.1:4444/", serializedRequest));

            Debug.Log("Started Client Request");
        }

        void HandleClientResponse(string response) {
            responseEvent.RemoveAllListeners();
            responseEvent.AddListener(HandleServerResponse);

            //Now the client can handle the response that it recieved
            ClientResponse clientResponse = JsonUtility.FromJson<ClientResponse>(response);

            //Check to see if we were authenticated before going forward
            if(!clientResponse.isAuthenticated) {
                Debug.LogError("Something went wrong and authentication failed! Is playfab set up correctly? Did you activate PlayFabLoginTest?");
                return;
            }

            //Now the client should connect to the server and pass clientResponse.serverAuthenticationKey into the server's authentication request.
            //Everything going forward should be done on the player run server, not the client.
            //For this example we're just going to handle it as if we are already on the server.

            //Create our server request
            ServerRequest serverRequest = new ServerRequest(clientResponse.serverAuthenticationKey, "1.1", RequestType.Server);
            //Serialize our request
            string serializedRequest = JsonUtility.ToJson(serverRequest);

            //Now the player run server should upload the request to the 
            StartCoroutine(Upload("http://127.0.0.1:4444/", serializedRequest));

            Debug.Log("Started Server Request");
        }

        void HandleServerResponse(string response) {
            ServerResponse serverResponse = JsonUtility.FromJson<ServerResponse>(response);

            if(serverResponse.isAuthenticated) {
                Debug.Log("Authenticated! The user may now connect.");

                //The user is authenticated
                //Run some code that connects them to the server
            } else {
                Debug.LogError("Uh oh! Something went wrong, looks like the server auth key didn't work. Perhaps the session timed out? (60 seconds)");
            }
        }

        IEnumerator Upload(string uri, string serializedRequest) {
            //Send JSON to auth server
            using(UnityWebRequest www = UnityWebRequest.Post(uri, serializedRequest)) {
                yield return www.SendWebRequest();

                if(www.result != UnityWebRequest.Result.Success) {
                    //An error has occurred
                    Debug.Log(www.error);
                } else {
                    //Handle response
                    Debug.Log($"Got: {www.downloadHandler.text}");

                    responseEvent.Invoke(www.downloadHandler.text);
                }
            }
        }
    }
}