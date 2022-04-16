using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KyoshiStudios.Authentication {
    public class AuthResponse {
        public bool isFinished = false;
        public bool isAuthentic = false;

        public AuthResponse(bool isAuthentic) {
            this.isAuthentic = isAuthentic;
            isFinished = true;
        }
    }
}