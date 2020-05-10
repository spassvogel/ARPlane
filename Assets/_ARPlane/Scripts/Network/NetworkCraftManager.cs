using System.Collections;
using System.Collections.Generic;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;

namespace UniversoAumentado.ARCraft.Network
{
    public class NetworkCraftManager : MonoBehaviour
    {
        [SerializeField] private UnityClient client;
        [SerializeField] private Transform networkPrefab;

        Dictionary<ushort, Transform> networkCrafts = new Dictionary<ushort, Transform>();

        public void Add(ushort id, Transform craft)
        {
            networkCrafts.Add(id, craft);
        }

        public void Awake()
        {
            client.MessageReceived += MessageReceived;
        }

        void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage() as Message)
            {
                if (message.Tag == Tags.SpawnCraft)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        ushort id = reader.ReadUInt16();
                        var craft = Instantiate(networkPrefab, transform);
                        craft.gameObject.name = $"[client:{id}]";
                        Add(id, craft);
                    }
                }
                else if (message.Tag == Tags.UpdateTransform)
                {
                    using (DarkRiftReader reader = message.GetReader())
                    {
                        ushort id = reader.ReadUInt16();
                        float posX = reader.ReadSingle();
                        float posY = reader.ReadSingle();
                        float posZ = reader.ReadSingle();
                        float rotX = reader.ReadSingle();
                        float rotY = reader.ReadSingle();
                        float rotZ = reader.ReadSingle();
                        var transform = networkCrafts[id];
                        transform.position = new Vector3(posX, posY, posZ);
                        transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
                    }
                }
            }
        }
    }
}
