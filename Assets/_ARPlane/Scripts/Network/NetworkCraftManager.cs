using System.Collections;
using System.Collections.Generic;
using ARPlaneServer.Events;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;

namespace UniversoAumentado.ARCraft.Network {
    public class NetworkCraftManager : NetworkManager {
        [SerializeField] private GameObject arPlanePrefab;

        readonly Dictionary<ushort, GameObject> networkCrafts = new Dictionary<ushort, GameObject>();

        protected override void MessageReceived(Message message, DarkRiftReader reader) {
            switch ((Tag)message.Tag) {
                case Tag.ARCraftSpawn:
                    ARCraftSpawnEvent spawnEvent = reader.ReadSerializable<ARCraftSpawnEvent>();
                    HandleARCraftSpawn(spawnEvent);
                    break;
                case Tag.ARCraftStates:
                    ARCraftStatesEvent statesEvent = reader.ReadSerializable<ARCraftStatesEvent>();
                    HandleARCraftStates(statesEvent);
                    break;
            }
        }

        void HandleARCraftSpawn(ARCraftSpawnEvent spawnEvent) {
            if(spawnEvent.arCraft.ownerID != client.ID) {
                CreateNetworkCraft(spawnEvent.arCraft);
            }
        }

        void CreateNetworkCraft(ARPlaneServer.Classes.ARCraft arCraft) {
            var craft = Instantiate(arPlanePrefab, transform);
            craft.gameObject.name = $"[player:{arCraft.ownerID}]";
            networkCrafts[arCraft.ownerID] = craft;
        }

        void HandleARCraftStates(ARCraftStatesEvent statesEvent) {
            foreach(ARPlaneServer.Classes.ARCraft otherCraft in statesEvent.otherCrafts) {
                CreateNetworkCraft(otherCraft);
            }
        }
    }
}
