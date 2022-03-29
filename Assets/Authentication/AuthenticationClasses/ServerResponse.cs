using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerResponse {
    //If the client is authenticated with playfab and allowed to join the server
    public bool isAuthenticated;

    public ServerResponse(bool isAuthenticated) {
        this.isAuthenticated = isAuthenticated;
    }
}
