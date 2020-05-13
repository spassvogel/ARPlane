using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UniversoAumentado.ARCraft.Network {

    public class NetworkObjectsManager : MonoBehaviour {
        public NetworkManager networkManager;

        private void Awake() {
            networkManager.OnSpawnCraft += OnSpawnCraft;
        }

        private void OnSpawnCraft(int id) {

        }
    }

}