using UnityEngine;
using System.Collections;
using UniversoAumentado.ARPlane.Game;

namespace UniversoAumentado.ARPlane.Events {

    public class GameStateChangeEvent : AbstractEvent
    {
        public GameController.GameStates GameState;

        public GameStateChangeEvent(GameController.GameStates gameState)
        {
            GameState = gameState;
        }
    }
}
