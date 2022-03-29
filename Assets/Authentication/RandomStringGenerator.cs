using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStringGenerator : MonoBehaviour {
    static int stringLength = 16; 

    const string characters = "abcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateRandomString() {
        string randomString = "";

        for(int i = 0; i < stringLength; i++) {
            randomString += characters[Random.Range(0, characters.Length)];
        }

        return randomString;
    }
}
