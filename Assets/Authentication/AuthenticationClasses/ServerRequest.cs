using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRequest {
    //The server's IP address
    public string serverIP;
    //The temporary authentication key that the client has provided the server
    public string serverAuthenticationKey;
    //What type of request it is, used on the auth server
    public RequestType requestType;
    //If the authentication server has begun processing the request
    bool started = false;

    public ServerRequest(string serverAuthenticationKey, string serverIP, RequestType requestType) {
        this.serverAuthenticationKey = serverAuthenticationKey;
        this.serverIP = serverIP;
        this.requestType = requestType;
    }

    public ServerRequest() { }

    public void MarkStarted() {
        started = true;
    }

    public bool IsStarted() {
        return started;
    }
}
