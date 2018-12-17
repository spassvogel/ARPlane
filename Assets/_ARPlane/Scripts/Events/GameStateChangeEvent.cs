using UnityEngine;
using System.Collections;
using UniversoAumentado.ARCraft.Game;

namespace UniversoAumentado.ARCraft.Events {

    public class GameStateChangeEvent : AbstractEvent
    {
        public GameController.GameStates GameState;

        public GameStateChangeEvent(GameController.GameStates gameState)
        {
            GameState = gameState;
        }
    }
}
