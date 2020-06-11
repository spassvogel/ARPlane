using ARPlaneServer.Classes;
using ARPlaneServer.Events;
using DarkRift;
using System.Collections.Generic;
using UnityEngine;

namespace UniversoAumentado.ARCraft.Network {
    public class NetworkPlayerManager : NetworkManager {

        readonly Dictionary<int, Player> players = new Dictionary<int, Player>();
        Player player;

        protected override void MessageReceived(Message message, DarkRiftReader reader) {
            switch ((Tag)message.Tag) {
                case Tag.PlayerStates:
                    PlayerStatesEvent playerStatesEvent = reader.ReadSerializable<PlayerStatesEvent>();
                    SetupPlayerStates(playerStatesEvent);
                    break;
                case Tag.PlayerUpdate:
                    PlayerUpdateEvent playerUpdateEvent = reader.ReadSerializable<PlayerUpdateEvent>();
                    UpdatePlayer(playerUpdateEvent);
                    break;
            }
        }

        void UpdatePlayer(PlayerUpdateEvent playerUpdateEvent) {
            players[playerUpdateEvent.newPlayerState.id] = playerUpdateEvent.newPlayerState;
        }

        void SetupPlayerStates(PlayerStatesEvent playerStatesEvent) {
            foreach(Player player in playerStatesEvent.players) {
                players[player.id] = player;
            }
            player = players[client.ID];
        }

        public void SetPlayerName(string name) {
            if(client.ConnectionState != ConnectionState.Connected) {
                Debug.LogError("Not connected. Cannot set player name.");
                return;
            }
            player.name = name;
            SendMessage(Tag.PlayerUpdate, new PlayerUpdateEvent() {
                newPlayerState = player
            });
        }
    }

}