using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthRequest
{
    public bool started = false;
    public string authToken;

    public AuthRequest(string authToken) {
        this.authToken = authToken;
    }
}
