using System;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.Menu
{
    public class TesterConnectionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private Button _server;
        [SerializeField] private Button _client;

        private void Start()
        {
            _server.onClick.AddListener(ServerButtonClicked);
            _client.onClick.AddListener(ClientButtonClicked);
        }

        private void ClientButtonClicked()
        {
            _server.interactable = false;
            _client.interactable = false;
            _networkManager.ClientManager.StartConnection();
        }

        private void ServerButtonClicked()
        {
            _server.interactable = false;
            _client.interactable = false;
            _networkManager.ServerManager.StartConnection();
        }
    }
}

