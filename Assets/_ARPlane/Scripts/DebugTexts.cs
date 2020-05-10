﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTexts : MonoBehaviour {

    public Text prototype;

    private Dictionary<string, Text> texts = new Dictionary<string, Text>();

    private void Start() {
        prototype.gameObject.SetActive(false);
    }

    public void SetDebugText(string name, string value) {
        Debug.Log($"Setting debug text {name} to {value}");
        Text text;
        if(!texts.ContainsKey(name)) {
            text = Instantiate(prototype);
            text.transform.parent = transform;
            texts[name] = text;
            text.gameObject.SetActive(true);
        } else {
            text = texts[name];
        }
        text.text = name + " " + value;
    }
}
