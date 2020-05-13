using UnityEngine;
using System.Collections;
using UniversoAumentado.ARCraft.Events;
using System;
using UniversoAumentado.ARCraft.Game;

namespace UniversoAumentado.ARCraft.UI
{
    public class UIController : MonoBehaviour
    {
        public GameObject LobbyUI;
        public GameObject PlayingUI;

        void OnEnable()
        {
            GlobalEventDispatcher.Instance.AddEventListener<GameStateChangeEvent>(OnGameStateChanged);
        }

        void OnDisable()
        {
            GlobalEventDispatcher.Instance.RemoveEventListener<GameStateChangeEvent>(OnGameStateChanged);
        }


        private void OnGameStateChanged(AbstractEvent e)
        {
            var evt = e as GameStateChangeEvent;

            LobbyUI.SetActive(evt.GameState == GameController.GameStates.Lobby);
            PlayingUI.SetActive(evt.GameState == GameController.GameStates.Playing);
        }

    }
}
