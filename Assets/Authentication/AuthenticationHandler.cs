using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AuthenticationHandler : MonoBehaviour {

    Dictionary<ClientRequest, ClientResponse> clientAuthenticationQueue = new Dictionary<ClientRequest, ClientResponse>();
    List<ServerAuthenticationKey> validAuthKeys = new List<ServerAuthenticationKey>();

    Dictionary<ServerRequest, ServerResponse> serverAuthenticationQueue = new Dictionary<ServerRequest, ServerResponse>();

    // Update is called once per frame
    private void Update() {
        foreach(ClientRequest clientRequest in clientAuthenticationQueue.Keys) {
            if(!clientRequest.IsStarted()) {
                CheckClientAuthentication(clientRequest);
            }
        }
    }

    private async Task CheckClientAuthentication(ClientRequest authRequest) {
        authRequest.MarkStarted();

        Authenticator authenticator = new Authenticator();

        bool authenticated = await authenticator.CheckUserAuthentication(authRequest.playfabSessionTicket);

        ServerAuthenticationKey authenticationKey = new ServerAuthenticationKey(authRequest.connectingIp);

        validAuthKeys.Add(authenticationKey);
        clientAuthenticationQueue[authRequest] = new ClientResponse(authenticated, authenticationKey.serverAuthenticationKey);
    }

    public void AddClientRequestToQueue(ClientRequest clientRequest) {
        clientAuthenticationQueue.Add(clientRequest, null);
    }

    public void RemoveClientRequestFromQueue(ClientRequest clientRequest) {
        if(clientAuthenticationQueue.ContainsKey(clientRequest)) {
            clientAuthenticationQueue.Remove(clientRequest);
        }
    }

    public bool ResponseIsNull(ClientRequest clientRequest) {
        if(!clientAuthenticationQueue.ContainsKey(clientRequest)) {
            Debug.LogError("Attempted to check if a reponse is null that doesn't exist inside of the authentication queue.");
            return false;
        }

        return clientAuthenticationQueue[clientRequest] == null;
    }

    public ClientResponse GetClientResponse(ClientRequest clientRequest) {
        return clientAuthenticationQueue[clientRequest];
    }
}
