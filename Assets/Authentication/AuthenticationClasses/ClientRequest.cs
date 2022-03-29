using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientRequest {
    //The IP the client is attempting to connect to
    public string connectingIp;
    //The user's playfab session ticket
    public string playfabSessionTicket;
    //If the authentication server has begun processing the request
    private bool started = false;

    public ClientRequest(string playfabSessionTicket, string connectingIp) {
        this.playfabSessionTicket = playfabSessionTicket;
        this.connectingIp = connectingIp;
    }

    public void MarkStarted() {
        started = true;
    }

    public bool IsStarted() {
        return started;
    }
}