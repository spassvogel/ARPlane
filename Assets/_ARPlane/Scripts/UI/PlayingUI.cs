using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversoAumentado.ARCraft.AR;

public class PlayingUI : MonoBehaviour
{
    [SerializeField] private ARCraft arCraft;

    private void Awake()
    {
        arCraft.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        arCraft.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        arCraft.gameObject.SetActive(false);
    }

}
