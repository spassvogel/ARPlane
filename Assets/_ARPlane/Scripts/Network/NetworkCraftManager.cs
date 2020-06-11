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
                    CreateNetworkCraft(spawnEvent);
                    break;
                case Tag.ARCraftStates:
                    ARCraftStatesEvent statesEvent = reader.ReadSerializable<ARCraftStatesEvent>();
                    SetARCraftStates(statesEvent);
                    break;
            }
        }

        void CreateNetworkCraft(ARCraftSpawnEvent spawnEvent) {
            CreateNetworkCraft(spawnEvent.arCraft);
        }

        void CreateNetworkCraft(ARPlaneServer.Classes.ARCraft arCraft) {
            var craft = Instantiate(arPlanePrefab, transform);
            craft.gameObject.name = $"[client:{arCraft.ownerID}]";
            networkCrafts[arCraft.ownerID] = craft;
        }

        void SetARCraftStates(ARCraftStatesEvent statesEvent) {
            foreach(ARPlaneServer.Classes.ARCraft otherCraft in statesEvent.otherCrafts) {
                CreateNetworkCraft(otherCraft);
            }
        }
    }
}
