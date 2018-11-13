using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace UniversoAumentado.ARPlane.Network
{
    public class ARPlaneNetworkManager : NetworkManager
    {
        public GameObject PlanePrefab;
        public Transform CameraTransform;

        private int _curPlayer;

        void Start()
        {
#if UNITY_EDITOR
            this.StartHost();
#else
        this.GetComponent<NetworkManagerHUD>().enabled = false;
#endif
        }


        //Called on client when connect
        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("On client connect!!!!!!00");
            // Create message to set the player
            IntegerMessage msg = new IntegerMessage(_curPlayer);

            // Call Add player and pass the message
            ClientScene.AddPlayer(conn, 0, msg);
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
            // Create player object with prefab
            var player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;

            // Add player object for connection
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            var plane = SpawnPlane();

            plane.target = player.transform;
            plane.cam = CameraTransform;

            Debug.Log("Adding player " + playerControllerId + " " + _curPlayer);
        }

        private Plane SpawnPlane()
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
            var plane = (GameObject)Instantiate(PlanePrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(plane);

            return plane.GetComponent<Plane>();
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
