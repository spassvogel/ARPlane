using System;
using System.Collections.Generic;
using System.Linq;
using ARPlaneServer.Events;
using DarkRift;
using UnityEngine;

namespace UniversoAumentado.ARCraft.Network {

    public class NetworkObjectManager : NetworkManager {

        static int nextID = 0;

        [Serializable]
        public class NetworkPrefab {
            public string type;
            public GameObject prefab;
        }

        [SerializeField] private Transform spawnIn;
        [SerializeField] private NetworkPrefab[] prefabs;

        readonly Dictionary<string, NetworkObject> networkObjects = new Dictionary<string, NetworkObject>();

        protected override void MessageReceived(Message message, DarkRiftReader reader) {
            switch ((Tag)message.Tag) {
                case Tag.ObjectUpdate:
                    ObjectUpdateEvent objectUpdateEvent = reader.ReadSerializable<ObjectUpdateEvent>();
                    HandleObjectUpdate(objectUpdateEvent);
                    break;
            }
        }

        void HandleObjectUpdate(ObjectUpdateEvent updateEvent) {
            NetworkObject networkObject = networkObjects[updateEvent.newState.id];
            if (networkObject == null) {
                networkObject = CreateObject(updateEvent);
            }
            if (networkObject == null) return;

            networkObject.SetObjectState(updateEvent.newState);
        }

        string GetNextID() {
            if(client.ConnectionState != ConnectionState.Connected) {
                throw new Exception("Client not connected, cannot generate NetworkObject ID.");
            }
            return $"{client.ID}-{nextID++}";
        }

        public void RegisterObject(NetworkObject networkObject) {
            networkObjects[networkObject.id] = networkObject;
        }

        public void UpdateObject(NetworkObject networkObject) {
            if(client.ConnectionState != ConnectionState.Connected) {
                return;
            }
            if(string.IsNullOrEmpty(networkObject.id)) {
                // NetworkObject may have registered before we were connected so this is the first time we can generate an ID.
                networkObject.SetID(GetNextID());
            }
            SendMessage(Tag.ObjectUpdate, new ObjectUpdateEvent() {
                newState = networkObject.GetNetworkObject()
            });
        }

        public void RemoveObject(NetworkObject networkObject) {
            networkObjects.Remove(networkObject.id);
        }

        NetworkObject CreateObject(ObjectUpdateEvent updateEvent) {
            NetworkPrefab networkPrefab = prefabs.First(prefab => prefab.type == updateEvent.newState.type);
            
            NetworkObject networkObject;
            
            if(networkPrefab == null) {
                Debug.LogError($"No NetworkPrefab set for type {updateEvent.newState.type}.");

                // Create a simple game object to at least have something following the networkobject
                GameObject go = new GameObject("NetworkObject", typeof(NetworkObject));
                go.transform.SetParent(transform);
                networkObject = go.GetComponent<NetworkObject>();
            } else {
                // Instantiate prefab
                Transform spawnTransform = spawnIn != null ? spawnIn : transform;
                GameObject go = Instantiate(networkPrefab.prefab, spawnTransform);

                // Make sure spawned object has a NetworkObject component
                networkObject = go.GetComponent<NetworkObject>();
                if(networkObject == null) {
                    networkObject = go.AddComponent<NetworkObject>();
                }
            }

            // Set NetworkObject info and dependencies
            networkObject.SetObjectInfo(updateEvent.newState);
            networkObject.state = NetworkObject.State.LISTEN;
            networkObject.networkObjectManager = this;
            
            RegisterObject(networkObject);

            return networkObject;
        }
    }
}
