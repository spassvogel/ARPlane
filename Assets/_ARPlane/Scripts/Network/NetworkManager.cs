using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UniversoAumentado.ARCraft.Network {
    public class NetworkManager : MonoBehaviour {
        [SerializeField] private UnityClient client;

        Dictionary<ushort, Transform> networkCrafts = new Dictionary<ushort, Transform>();

        public delegate void SpawnCraftHandler(int id);
        public event SpawnCraftHandler OnSpawnCraft;

        public delegate void UpdateTransformHandler(string id, Vector3 position, Quaternion rotation);
        public event UpdateTransformHandler OnUpdateTransform;

        private static void WriteVector3(DarkRiftWriter writer, Vector3 vector) {
            writer.Write(vector.x);
            writer.Write(vector.y);
            writer.Write(vector.z);
        }

        private static Vector3 ReadVector3(DarkRiftReader reader) {
            return new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );
        }

        public void Add(ushort id, Transform craft) {
            networkCrafts.Add(id, craft);
        }

        public void Awake() {
            client.MessageReceived += MessageReceived;
        }

        void MessageReceived(object sender, MessageReceivedEventArgs e) {
            using (Message message = e.GetMessage() as Message) {
                using (DarkRiftReader reader = message.GetReader()) {
                    if(message.Tag == Tags.SpawnCraft) {
                        HandleSpawnCraft(reader);
                    } else if(message.Tag == Tags.UpdateTransform) {
                        HandleUpdateTransform(reader);
                    }
                }
            }
        }

        void SendMessage(DarkRiftWriter writer, ushort tag) {
            using (Message message = Message.Create(tag, writer)) {
                client.SendMessage(message, SendMode.Unreliable);
            }
        }

        void HandleSpawnCraft(DarkRiftReader reader) {
            int id = reader.ReadUInt16();

            OnSpawnCraft?.Invoke(id);
        }

        void HandleUpdateTransform(DarkRiftReader reader) {
            string id = reader.ReadString();

            Vector3 position = ReadVector3(reader);
            Quaternion rotation = Quaternion.Euler(ReadVector3(reader));

            OnUpdateTransform?.Invoke(id, position, rotation);
        }

        public void SendUpdateTransform(string id, Transform transform) {
            using (DarkRiftWriter writer = DarkRiftWriter.Create()) {
                writer.Write(id);
                WriteVector3(writer, transform.position);
                WriteVector3(writer, transform.rotation.eulerAngles);

                SendMessage(writer, Tags.UpdateTransform);
            }
        }
    }
}