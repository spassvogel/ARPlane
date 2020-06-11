using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ARPlaneServer.Events;
using DarkRift;
using DarkRift.Client.Unity;
using UnityEngine;
using UniversoAumentado.ARCraft.Events;

namespace UniversoAumentado.ARCraft.Network {
    public class NetworkObjectManager : NetworkManager {

        [Serializable]
        public class NetworkPrefab {
            public string type;
            public NetworkObject prefab;
        }

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

        public void RegisterObject(NetworkObject networkObject) {
            networkObjects[networkObject.id] = networkObject;
        }

        public void UpdateObject(NetworkObject networkObject) {
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
                networkObject = Instantiate(networkPrefab.prefab, transform);
            }

            networkObject.SetObjectInfo(updateEvent.newState);

            networkObjects[updateEvent.newState.id] = networkObject;

            return networkObject;
        }
    }
}
