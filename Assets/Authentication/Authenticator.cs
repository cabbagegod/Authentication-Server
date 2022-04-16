using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KyoshiStudios.Authentication {
    public class Authenticator {
        public bool isFinished = false;
        public bool isAuthentic = false;

        public async Task<bool> CheckUserAuthentication(string sessionTicket) {
            PlayFab.ServerModels.AuthenticateSessionTicketRequest request = new PlayFab.ServerModels.AuthenticateSessionTicketRequest { SessionTicket = sessionTicket };

            PlayFab.PlayFabServerAPI.AuthenticateSessionTicket(request, result => {
                isAuthentic = true;
                isFinished = true;
            }, error => {
                Debug.LogError(error.GenerateErrorReport());
                isFinished = true;
            });

            while(!isFinished) {
                await Task.Delay(100);
            }

            return isAuthentic;
        }
    }
}