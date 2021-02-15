using System;
using System.Linq;
using System.Net;
using DarkRift;
using DarkRift.Client.Unity;
using UnityEngine;
using UnityEngine.UI;
using UniversoAumentado.ARCraft.Events;
using UniversoAumentado.ARCraft.Network;
using UniversoAumentado.ARCraft.Utils;

namespace UniversoAumentado.ARCraft.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private NetworkPlayerManager networkPlayerManager;
        [SerializeField] private Button joinButton;
        [SerializeField] private TMPro.TMP_InputField ipAddressText;
        [SerializeField] private TMPro.TMP_Dropdown ipAddressDropdown;
        [SerializeField] private TMPro.TMP_InputField nameText;
        [SerializeField] private UnityClient client;

        private static int port = 4296;
        private static string playerPrefipAddress = "ipAddress";
        private static string playerPrefName = "name";

        private void Awake()
        {
            joinButton.onClick.AddListener(Join);
            ipAddressDropdown.onValueChanged.AddListener(IPAddressSelected);
        }

        private void Start()
        {
            var list = PlayerPrefsX.GetStringArray(playerPrefipAddress).ToList();            
            ipAddressDropdown.AddOptions(list);
            ipAddressText.text = list.FirstOrDefault();

            if (!string.IsNullOrEmpty(PlayerPrefs.GetString(playerPrefName)))
            {
                nameText.text = PlayerPrefs.GetString(playerPrefName);
            }
        }

        private void IPAddressSelected(int index)
        {
            string selectedIPAddress = ipAddressDropdown.options[index].text;
            ipAddressText.text = selectedIPAddress;
        }

        void Join()
        {
            IPAddress ipAddress = IPAddress.Parse(ipAddressText.text);
            // todo: fail when not an ip address!
            client.ConnectInBackground(ipAddress, port, false, ConnectComplete);
        }

        private void ConnectComplete(Exception e)
        {
            if (client.ConnectionState == ConnectionState.Connected)
            {
                StoreInPlayerPrefs();
                SetName();

                GlobalEventDispatcher.Instance.DispatchEvent(new GameStateChangeEvent(Game.GameController.GameStates.Playing));
            }
            else
            {
                Debug.LogError($"Could not connect to {ipAddressText.text}.{Environment.NewLine}{e.Message}");
            }
        }

        private void StoreInPlayerPrefs()
        {
            var list = PlayerPrefsX.GetStringArray(playerPrefipAddress).ToList();
            list.Remove(ipAddressText.text);    // Remove if already somewhere in the list
            list.Insert(0, ipAddressText.text); // Add to top      
            PlayerPrefsX.SetStringArray(playerPrefipAddress, list.ToArray());
            PlayerPrefs.SetString(playerPrefName, nameText.text);
        }

        private void SetName()
        {
            networkPlayerManager.SetPlayerName(nameText.text);
        }
    }
}
