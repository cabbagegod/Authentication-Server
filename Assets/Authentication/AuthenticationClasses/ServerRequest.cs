using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerRequest {
    //The server's IP address
    public string serverIP;
    //The temporary authentication key that the client has provided the server
    public string serverAuthenticationKey;
    //If the authentication server has begun processing the request
    bool started = false;

    public void MarkStarted() {
        started = true;
    }

    public bool IsStarted() {
        return started;
    }
}
