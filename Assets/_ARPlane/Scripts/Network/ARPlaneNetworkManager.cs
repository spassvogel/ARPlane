﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UniversoAumentado.ARCraft.Events;

namespace UniversoAumentado.ARCraft.Network
{
    public class ARPlaneNetworkManager : NetworkManager
    {
        public GameObject PlanePrefab;
        public Transform CameraTransform;

        private int _curPlayer;

        void Start()
        {
#if UNITY_EDITOR
            this.StartServer();
#else
            // this.GetComponent<NetworkManagerHUD>().enabled = false;
#endif
        }


        //Called on client when connect
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            GetComponent<ARPlaneNetworkDiscovery>().StopBroadcast();

            Debug.Log("On client connect!!!!!!00");
            // Create message to set the player
            // IntegerMessage msg = new IntegerMessage(_curPlayer);

            // Call Add player and pass the message
            // ClientScene.AddPlayer(conn, 0, msg);

            GlobalEventDispatcher.Instance.DispatchEvent(new GameStateChangeEvent(Game.GameController.GameStates.SearchingForMarker));
        }

        // Server
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            // Read client message and receive index
            if (extraMessageReader != null)
            {
                var stream = extraMessageReader.ReadMessage<IntegerMessage>();
                _curPlayer = stream.value;
            }
            // Create player object with prefab (this is the cursor!)
            ARPlayer player = (Instantiate(playerPrefab, Vector3.zero, Quaternion.identity)).GetComponent<ARPlayer>();
            player.name = "Player " + conn.connectionId;
            
            // Add player object for connection
            NetworkServer.AddPlayerForConnection(conn, player.gameObject, playerControllerId);
            var aircraft = SpawnAircraft();
            player.arCraft = aircraft;
            aircraft.gameObject.name = "Aircraft for Player " + conn.connectionId + " (" + spawnPrefabs[0].name + ")";

            aircraft.target = player.cursor.transform;
            aircraft.cam = player.transform;

            Debug.Log("Adding player " + playerControllerId + "  " + conn.connectionId);
        }

        private ARCraft SpawnAircraft()
        {
            var spawnPosition = new Vector3(
                    Random.Range(-8.0f, 8.0f),
                    0.0f,
                    Random.Range(-8.0f, 8.0f));

            var spawnRotation = Quaternion.Euler(
                0.0f,
                Random.Range(0, 180),
                0.0f);

            // Todo: spawn where?
            var plane = Instantiate(spawnPrefabs[0], spawnPosition, spawnRotation);
            NetworkServer.Spawn(plane);

            return plane.GetComponent<ARCraft>();
        }

        public override void OnServerConnect(NetworkConnection nc)
        {
            base.OnServerConnect(nc);

            int cid = nc.connectionId;
            int hid = nc.hostId;

            Debug.Log("server: on server connect " + cid + " " + hid);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("Client disconnect");
            base.OnClientDisconnect(conn);

            StopClient();
        }



        public override void OnDropConnection(bool success, string extendedInfo)
        {
            Debug.Log("Client dropped " + extendedInfo);
            base.OnDropConnection(success, extendedInfo);
        }

        public override void OnStopClient()
        {
            Debug.Log("Client stopped");
        }
    }
}
