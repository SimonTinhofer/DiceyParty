using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DiceyParty.Lobby
{
    public class LobbyUIHandler : MonoBehaviour
    {
        private static LobbyUIHandler _instance;
        
        [SerializeField] private TMP_Text _sessionName;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _editNameButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Transform _playerCardParent;
        
        [SerializeField] private GameObject _editNameContainer;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private Button _nameConfirmButton;
        [SerializeField] private Button _nameCancelButton;
        
        [SerializeField] private GameObject _miniGamesContainer;
        [SerializeField] private Button[] _miniGamesPlayButton;
        [SerializeField] private Button _miniGamesCancelButton;

        private string _playerName;

        private void Awake()
        {
            if (_instance != null)
                Destroy(gameObject);
            else
                _instance = this;
        }

        public static Transform GetPlayerCardParent()
        {
            return _instance._playerCardParent;
        }

        private void Start()
        {
            _leaveButton.onClick.AddListener(LeaveButtonClicked);
            
            _editNameButton.onClick.AddListener(OpenNameContainer);
            _nameConfirmButton.onClick.AddListener(NameConfirmButtonClicked);
            _nameCancelButton.onClick.AddListener(NameCancelButtonClicked);
            _nameInput.onValueChanged.AddListener(NameInputChanged);
            
            _playButton.onClick.AddListener(OpenMiniGamesContainer);
            _miniGamesCancelButton.onClick.AddListener(CloseMiniGamesContainer);
            for (int i = 0; i < _miniGamesPlayButton.Length; i++)
            {
                int sceneIndex = i + 2; 
                _miniGamesPlayButton[i].onClick.AddListener(() => PlayMiniGame(sceneIndex));
            }
        }

        public void EnablePlayButton()
        {
            _playButton.interactable = true;
        }

        private void PlayMiniGame(int sceneIndex)
        {
            LobbyManager.PlayMiniGame(sceneIndex);
        }

        private void CloseMiniGamesContainer()
        {
            _miniGamesContainer.SetActive(false);
        }

        private void OpenMiniGamesContainer()
        {
            _miniGamesContainer.SetActive(true);
        }
        
        

        private void NameInputChanged(string newInput)
        {
            if (newInput.Length < 1)
                _nameConfirmButton.interactable = false;
            else
            {
                _nameConfirmButton.interactable = true;
                _playerName = newInput;
            }

        }

        private void NameCancelButtonClicked()
        {
            _nameInput.text = "";
            _nameConfirmButton.interactable = false;
            _editNameContainer.SetActive(false);
        }

        private void NameConfirmButtonClicked()
        {
            _nameInput.text = "";
            _nameConfirmButton.interactable = false;
            _editNameContainer.SetActive(false);
            LobbyManager.UpdateName(_playerName);
        }

        private void OpenNameContainer()
        {
            _editNameContainer.SetActive(true);
        }

        private void LeaveButtonClicked()
        {
            LobbyManager.LeaveSession();
        }

        public void SetSessionName(string sessionName)
        {
            _sessionName.text = "Id: " + sessionName;
        }
    }
}