using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerAuthenticationKey {
    //When the key was created (used for timeouts)
    public readonly float creationTime;
    //The string generated to represent the authentication key
    public readonly string serverAuthenticationKey;
    //The IP address that this authentication key was created for
    public readonly string serverIp;

    public ServerAuthenticationKey(string serverIp) {
        creationTime = Time.time;
        serverAuthenticationKey = RandomStringGenerator.GenerateRandomString();
        this.serverIp = serverIp;
    }
}
