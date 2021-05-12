﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : NetworkBehaviour
{
    [SerializeField] NetworkManager networkManager;

    void Start() {
        // batchmode means it is headless build (which is when it is server so server will start itself)
        if (!Application.isBatchMode) {
            Debug.Log("-- Client Build --");
            networkManager.StartClient();
        }
        else {
            Debug.Log("-- Server Build --");
        }
    }

    public void JoinLocal() {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

}
