using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

public class AuthenticationHandler : MonoBehaviour {
    //Queues
    Dictionary<ClientRequest, ClientResponse> clientAuthenticationQueue = new Dictionary<ClientRequest, ClientResponse>();
    Dictionary<ServerRequest, ServerResponse> serverAuthenticationQueue = new Dictionary<ServerRequest, ServerResponse>();
    //Use removal queue instead of removing them directly to prevent them from being removed in another thread while the for loop is iterating
    List<ClientRequest> clientRemovalQueue = new List<ClientRequest>();
    List<ServerRequest> serverRemovalQueue = new List<ServerRequest>();

    //Existing server auth keys
    List<ServerAuthenticationKey> validAuthKeys = new List<ServerAuthenticationKey>();

    // Update is called once per frame
    private void Update() {
        //Backwards iteration to prevent removing an element during iteration
        for(int i = clientAuthenticationQueue.Keys.Count - 1; i >= 0; i--) {
            ClientRequest clientRequest = clientAuthenticationQueue.Keys.ElementAt(i);

            //Remove object if in removal queue
            if(clientRemovalQueue.Contains(clientRequest)) {
                clientAuthenticationQueue.Remove(clientRequest);
                clientRemovalQueue.Remove(clientRequest);
                continue;
            }

            if(!clientRequest.IsStarted()) {
                _ = CheckClientAuthentication(clientRequest);
            }
        }

        for(int i = serverAuthenticationQueue.Keys.Count - 1; i >= 0; i--) {
            ServerRequest serverRequest = serverAuthenticationQueue.Keys.ElementAt(i);

            //Remove object if in removal queue
            if(serverRemovalQueue.Contains(serverRequest)) {
                serverAuthenticationQueue.Remove(serverRequest);
                serverRemovalQueue.Remove(serverRequest);
                continue;
            }

            if(!serverRequest.IsStarted()) {
                _ = CheckServerAuthentication(serverRequest);
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

    private async Task CheckServerAuthentication(ServerRequest authRequest) {
        authRequest.MarkStarted();

        bool authenticated = true;

        ServerAuthenticationKey authKey = GetServerAuthenticationKey(authRequest.serverAuthenticationKey);
        //If it returns null then that means the submitted key doesn't exist
        if(authKey == null) {
            authenticated = false;
        } else {
            if(!IPExists(authKey.serverIp))
                authenticated = false;
        }

        serverAuthenticationQueue[authRequest] = new ServerResponse(authenticated);
    }

    public void AddClientRequestToQueue(ClientRequest clientRequest) {
        clientAuthenticationQueue.Add(clientRequest, null);
    }

    public void AddServerRequestToQueue(ServerRequest serverRequest) {
        serverAuthenticationQueue.Add(serverRequest, null);
    }

    public void RemoveServerRequestFromQueue(ServerRequest serverRequest) {
        if(serverAuthenticationQueue.ContainsKey(serverRequest)) {
            serverRemovalQueue.Add(serverRequest);
        }
    }

    public void RemoveClientRequestFromQueue(ClientRequest clientRequest) {
        if(clientAuthenticationQueue.ContainsKey(clientRequest)) {
            clientRemovalQueue.Add(clientRequest);
        }
    }

    public bool ResponseIsNull(ServerRequest serverRequest) {
        if(!serverAuthenticationQueue.ContainsKey(serverRequest)) {
            Debug.LogError("Attempted to check if a reponse is null that doesn't exist inside of the authentication queue.");
            return false;
        }

        return serverAuthenticationQueue[serverRequest] == null;
    }

    public bool ResponseIsNull(ClientRequest clientRequest) {
        if(!clientAuthenticationQueue.ContainsKey(clientRequest)) {
            Debug.LogError("Attempted to check if a reponse is null that doesn't exist inside of the authentication queue.");
            return false;
        }

        return clientAuthenticationQueue[clientRequest] == null;
    }

    public ServerResponse GetServerResponse(ServerRequest serverRequest) {
        return serverAuthenticationQueue[serverRequest];
    }

    public ClientResponse GetClientResponse(ClientRequest clientRequest) {
        return clientAuthenticationQueue[clientRequest];
    }

    public ServerAuthenticationKey GetServerAuthenticationKey(string serverAuthenticationKey) {
        foreach(ServerAuthenticationKey key in validAuthKeys) {
            if(key.serverAuthenticationKey == serverAuthenticationKey) {
                return key;
            }
        }
        return null;
    }

    public bool IPExists(string ip) {
        foreach(ServerAuthenticationKey key in validAuthKeys) {
            if(key.serverIp == ip) {
                return true;
            }
        }
        return false;
    }
}
