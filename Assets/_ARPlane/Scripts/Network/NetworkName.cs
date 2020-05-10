using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift.Client;
using DarkRift.Client.Unity;
using DarkRift;

namespace UniversoAumentado.ARCraft.Network
{
    // Updates the server when this player changes her name 
    public class NetworkName : MonoBehaviour
    {
        [SerializeField] private UnityClient client;

        public void NameChanged(string name)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(name);

                using (Message message = Message.Create(Tags.UpdateName, writer))
                {
                    client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
    }
}
