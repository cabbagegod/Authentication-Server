using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KyoshiStudios.Authentication {
    public class ServerResponse {
        //If the client is authenticated with playfab and allowed to join the server
        public bool isAuthenticated;

        public ServerResponse(bool isAuthenticated) {
            this.isAuthenticated = isAuthenticated;
        }
    }
}