using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversoAumentado.ARCraft.Events;

namespace UniversoAumentado.ARCraft.Game
{
    public class GameController : MonoBehaviour { 

        public enum GameStates
        {
            Lobby,
            Playing
        }       


        // Use this for initialization
        void Start()
        {
            var startState = GameStates.Lobby;
            GlobalEventDispatcher.Instance.DispatchEvent(new GameStateChangeEvent(startState));
        }

    }
}