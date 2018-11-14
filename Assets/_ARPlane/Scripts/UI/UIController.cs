using UnityEngine;
using System.Collections;
using UniversoAumentado.ARPlane.Events;
using System;
using UniversoAumentado.ARPlane.Game;

public class UIController : MonoBehaviour
{
    public GameObject SearchingForServerUI;
    public GameObject SearchingForMarker;
    public GameObject SearchingForTrackerUI;
    public GameObject PlayingUI;

    private void Awake()
    {
        GlobalEventDispatcher.Instance.AddEventListener<GameStateChangeEvent>(OnGameStateChanged);
    }

    private void OnDestroy()
    {
        GlobalEventDispatcher.Instance.RemoveEventListener<GameStateChangeEvent>(OnGameStateChanged);

    }


    private void OnGameStateChanged(AbstractEvent e)
    {
        var evt = e as GameStateChangeEvent;

        SearchingForServerUI.SetActive(evt.GameState == GameController.GameStates.SearchingForServer);
        SearchingForMarker.SetActive(evt.GameState == GameController.GameStates.SearchingForMarker);
        SearchingForTrackerUI.SetActive(evt.GameState == GameController.GameStates.SearchingForTracker);
        PlayingUI.SetActive(evt.GameState == GameController.GameStates.Playing);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
