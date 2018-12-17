﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace UniversoAumentado.ARCraft.Network
{
    public class ARPlaneNetworkDiscovery : NetworkDiscovery
    {
        bool hasRecievedBroadcastAtLeastOnce = false;

        void Start()
        {
            Initialize();

#if UNITY_EDITOR
            StartAsServer();
            showGUI = true;
            Debug.Log("Started as server");
#else
            showGUI = false;
            StartAsClient();
#endif
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            Debug.Log("BROADCAST RECEIVED " + fromAddress);
            if (hasRecievedBroadcastAtLeastOnce)
            {
                return;
            }
            hasRecievedBroadcastAtLeastOnce = true;
            NetworkManager.singleton.networkAddress = fromAddress;
            NetworkManager.singleton.StartClient();       // Call this when a marker has been detected
            StopBroadcast();
        }
    }
}