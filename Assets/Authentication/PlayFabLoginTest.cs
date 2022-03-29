using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayFabLoginTest : MonoBehaviour {

    public static string sessionTicket = "";
    // Start is called before the first frame update
    void Start()
    {
        PlayFab.ClientModels.LoginWithCustomIDRequest request = new PlayFab.ClientModels.LoginWithCustomIDRequest { CreateAccount = true, CustomId = "yo" };
        PlayFab.PlayFabClientAPI.LoginWithCustomID(request, result => {
            Debug.Log("Logged in!");
            sessionTicket = result.SessionTicket;
        }, error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    [ContextMenu("Test Authentication")]
    public void TestAuth(string parSessionTicket) {
        Debug.Log(parSessionTicket);
        Debug.Log(sessionTicket);

        PlayFab.ServerModels.AuthenticateSessionTicketRequest request = new PlayFab.ServerModels.AuthenticateSessionTicketRequest { SessionTicket = parSessionTicket };

        PlayFab.PlayFabServerAPI.AuthenticateSessionTicket(request, result => {
            Debug.Log("Authentic");
        }, error => {
            Debug.LogError(error.GenerateErrorReport());
        });

        PlayFab.ServerModels.AuthenticateSessionTicketRequest request2 = new PlayFab.ServerModels.AuthenticateSessionTicketRequest { SessionTicket = sessionTicket };

        PlayFab.PlayFabServerAPI.AuthenticateSessionTicket(request2, result => {
            Debug.Log("Authentic2");
        }, error => {
            Debug.LogError("2 " + error.GenerateErrorReport());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
