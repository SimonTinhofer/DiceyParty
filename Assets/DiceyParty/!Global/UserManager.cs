using System;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;

namespace DiceyParty
{
    public class UserManager : MonoBehaviour
    {
        [SerializeField] NetworkManager _networkManager;
        private string _alertMsg;
        private static UserManager _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                PassNewNetworkManager(_networkManager);
                Destroy(gameObject);
            }
            else
            {
                _alertMsg = "For the best experience make sure you have a stable internet connection. \n\nIf you are on mobile consider downloading by \"add website to homescreen\" for better fullscreen and performance.\n\nThis game is intended to be played by 3 - 6 players.";
                AlertManager.OnNewAlertManagerLoaded += ShowAlert;
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        
        private void Start()
        {
            Setup();
        }

        private static void PassNewNetworkManager(NetworkManager networkManager) => _instance.HandlePassNewNetworkManager(networkManager);

        private void HandlePassNewNetworkManager(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            Setup();
        }

        private void Setup()
        {
            _networkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
            _networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
        }

        private void OnDestroy()
        {
            _networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
            AlertManager.OnNewAlertManagerLoaded -= ShowAlert;
        }

        private void OnClientConnectionState(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                _alertMsg = "You got disconnected";
                Destroy(_networkManager.gameObject);
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                AlertManager.OnNewAlertManagerLoaded += ShowAlert;
            }
        }
        
        private void OnServerConnectionState(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                _alertMsg = "Server closed";
                Destroy(_networkManager.gameObject);
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                AlertManager.OnNewAlertManagerLoaded += ShowAlert;
            }
        }

        private void ShowAlert()
        {
            AlertManager.Instance.CreateAlert(_alertMsg);
            AlertManager.OnNewAlertManagerLoaded -= ShowAlert;
        }
    }
}