using ARPlaneServer.Events;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversoAumentado.ARCraft.Network {

    public class NetworkManager : MonoBehaviour {
        [SerializeField] protected UnityClient client;

        protected virtual void Awake() {
            client.MessageReceived += MessageReceived;
        }

        protected void MessageReceived(object sender, MessageReceivedEventArgs e) {
            // Note: Unity cannot handle simplified syntax
            using (Message message = e.GetMessage() as Message) {
                using (DarkRiftReader reader = message.GetReader()) {
                    MessageReceived(message, reader);
                }
            }
        }

        protected virtual void MessageReceived(Message message, DarkRiftReader reader) {
            // For override
        }

        protected void SendMessage(Tag tag, NetworkEvent networkEvent) {
            using (DarkRiftWriter writer = DarkRiftWriter.Create()) {
                writer.Write(networkEvent);
                using (Message message = Message.Create((ushort)tag, writer)) {
                    client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
    }

}