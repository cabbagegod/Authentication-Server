using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;

public class HttpRequestTest : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        Debug.Log("Started Request");
        //StartCoroutine(GetRequest("http://127.0.0.1:4444/"));
        StartCoroutine(Upload("http://127.0.0.1:4444/"));
    }

    // Update is called once per frame
    void Update() {

    }

    IEnumerator Upload(string uri) {
        string sessionTicket = PlayFabLoginTest.sessionTicket;

        using(UnityWebRequest www = UnityWebRequest.Post(uri, sessionTicket)) {
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.error);
            } else {
                Debug.Log($"Got: {www.downloadHandler.text}");
            }
        }
    }

    IEnumerator GetRequest(string uri) {
        using(UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            Debug.Log("request finished");

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch(webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}
