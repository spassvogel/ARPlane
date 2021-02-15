using ARPlaneServer.Events;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;
using UniversoAumentado.ARCraft.Events;
using static UniversoAumentado.ARCraft.Game.GameController;

namespace UniversoAumentado.ARCraft.Network
{

    public class NetworkManager : MonoBehaviour {
        [SerializeField] protected UnityClient client;

        protected virtual void Awake() {
            client.MessageReceived += MessageReceived;
            client.Disconnected += Disconnected;
        }

        private void Disconnected(object sender, DisconnectedEventArgs e)
        {
            Debug.LogWarning("Disconnected from server, reason unknown");
            var startState = GameStates.Lobby;
            GlobalEventDispatcher.Instance.DispatchEvent(new GameStateChangeEvent(startState));
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