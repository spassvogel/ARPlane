using System.Collections;
using System.Collections.Generic;
using DarkRift;
using DarkRift.Client.Unity;
using UnityEngine;

// This is the players' craft
// Will send position and rotation to the server
namespace UniversoAumentado.ARCraft.Network
    {
    public class NetworkCraft : MonoBehaviour
    {
        [SerializeField] private UnityClient client;

        private void Update()
        {
            // Todo: should we throttle this somehow?

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(transform.position.x);
                writer.Write(transform.position.y);
                writer.Write(transform.position.z);
                writer.Write(transform.rotation.x);
                writer.Write(transform.rotation.y);
                writer.Write(transform.rotation.z);

                using (Message message = Message.Create((int)MessageTag.UpdateTransform, writer))
                {
                    client.SendMessage(message, SendMode.Unreliable);
                }
            }
        }
    }
}