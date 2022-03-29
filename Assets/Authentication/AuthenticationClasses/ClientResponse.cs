using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientResponse {
    public bool isAuthenticated;
    public string serverAuthenticationKey;

    /// <summary>
    /// Tells the client if their request has been accepted
    /// </summary>
    /// <param name="acceptedAuthentication">If the user's authentication has been accepted (still needs to be authenticated with player run server)</param>
    /// <param name="serverAuthenticationKey">A temporary authentication key generated so the server can also authenticate the user</param>
    public ClientResponse(bool isAuthenticated, string serverAuthenticationKey) {
        this.isAuthenticated = isAuthenticated;
        this.serverAuthenticationKey = serverAuthenticationKey;
    }
}