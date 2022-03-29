using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientRequest {
    //The IP the client is attempting to connect to
    public string connectingIp;
    //The user's playfab session ticket
    public string playfabSessionTicket;
    //What type of request it is, used on the auth server
    public RequestType requestType;
    //If the authentication server has begun processing the request
    private bool started = false;

    public ClientRequest(string playfabSessionTicket, string connectingIp, RequestType requestType) {
        this.playfabSessionTicket = playfabSessionTicket;
        this.connectingIp = connectingIp;
        this.requestType = requestType;
    }

    public ClientRequest() { }

    public void MarkStarted() {
        started = true;
    }

    public bool IsStarted() {
        return started;
    }
}