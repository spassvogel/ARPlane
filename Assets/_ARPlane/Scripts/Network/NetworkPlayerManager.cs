using ARPlaneServer.Classes;
using ARPlaneServer.Events;
using DarkRift;
using System.Collections.Generic;
using UnityEngine;

namespace UniversoAumentado.ARCraft.Network {
    public class NetworkPlayerManager : NetworkManager {

        readonly Dictionary<int, Player> players = new Dictionary<int, Player>();
        Player player;

        string playerName;

        protected override void MessageReceived(Message message, DarkRiftReader reader) {
            switch ((Tag)message.Tag) {
                case Tag.PlayerConnected:
                    PlayerConnectedEvent playerConnectedEvent = reader.ReadSerializable<PlayerConnectedEvent>();
                    HandlePlayerConnected(playerConnectedEvent);
                    break;
                case Tag.PlayerStates:
                    PlayerStatesEvent playerStatesEvent = reader.ReadSerializable<PlayerStatesEvent>();
                    HandlePlayerStates(playerStatesEvent);
                    break;
                case Tag.PlayerUpdate:
                    PlayerUpdateEvent playerUpdateEvent = reader.ReadSerializable<PlayerUpdateEvent>();
                    HandlePlayerUpdate(playerUpdateEvent);
                    break;
            }
        }

        void HandlePlayerConnected(PlayerConnectedEvent playerConnectedEvent) {
            Debug.Log($"Player {playerConnectedEvent.player.id} connected.");
            players[playerConnectedEvent.player.id] = playerConnectedEvent.player;
        }

        void HandlePlayerUpdate(PlayerUpdateEvent playerUpdateEvent) {
            Debug.Log($"Player {playerUpdateEvent.newPlayerState.id} info updated.");
            players[playerUpdateEvent.newPlayerState.id] = playerUpdateEvent.newPlayerState;
        }

        void HandlePlayerStates(PlayerStatesEvent playerStatesEvent) {
            foreach(Player player in playerStatesEvent.players) {
                players[player.id] = player;
            }
            player = players[client.ID];

            Debug.Log($"Received info for {players.Count} players.");

            // Player name was set before we had player info, set it officially now
            if (!string.IsNullOrEmpty(playerName)) {
                SetPlayerName(playerName);
            }
        }

        public void SetPlayerName(string name) {
            if(client.ConnectionState != ConnectionState.Connected) {
                Debug.LogError("Not connected. Cannot set player name.");
                return;
            }
            // No player information received yet
            if(player == null) {
                Debug.Log("Player name set before player info was received. Storing for later.");
                playerName = name; // store name until we do
                return;
            }

            Debug.Log($"Setting player name to '{name}.'");
            player.name = name;
            SendMessage(Tag.PlayerUpdate, new PlayerUpdateEvent() {
                newPlayerState = player
            });
        }
    }

}