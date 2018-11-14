﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversoAumentado.ARPlane.Events;

namespace UniversoAumentado.ARPlane.Game
{
    public class GameController : MonoBehaviour { 

        public enum GameStates
        {
            SearchingForServer,
            SearchingForMarker,
            SearchingForTracker,
            Playing
        }       


        // Use this for initialization
        void Start()
        {
            var startState = GameStates.SearchingForServer;
            GlobalEventDispatcher.Instance.DispatchEvent(new GameStateChangeEvent(startState));
        }

        private void OnDestroy()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}