using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DiceyParty.Menu
{
    public class ConnectionUIHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _connectionContainer;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;

        [SerializeField] private GameObject _joinContainer;
        [SerializeField] private TMP_Text _joinStatusText;
        [SerializeField] private TMP_Text _joinInfoText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private GameObject _sessionCardPrefab;
        [SerializeField] private Transform _sessionCardParent;

        //[SerializeField] private EdgeGapConnectionStarter _edgeGapConnect;

        private string _playerName;
        private string _code;

        private void Start()
        {
            _hostButton.onClick.AddListener(HostButtonClicked);
            _joinButton.onClick.AddListener(JoinButtonClicked);
            _cancelButton.onClick.AddListener(CancelButtonClicked);

            _connectionContainer.SetActive(true);
            _hostButton.interactable = false;
            _joinButton.interactable = false;

            _joinContainer.SetActive(false);
            _cancelButton.interactable = true;
        }

        #region UI

        private void HostButtonClicked()
        {
            _hostButton.interactable = false;
            _joinButton.interactable = false;
            CreateSession();
        }

        private void JoinButtonClicked()
        {
            _connectionContainer.SetActive(false);
            _joinContainer.SetActive(true);
            Debug.Log("Opened JoinContainer");
        }

        private void ConfirmButtonClicked()
        {
            JoinSession();
        }

        private void CancelButtonClicked()
        {
            _joinContainer.SetActive(false);
            _connectionContainer.SetActive(true);
        }

        #endregion

        private async void CreateSession()
        {
            _statusText.text = "WAITING FOR RESPONSE ...";
            Debug.Log("Created Session");
            /*bool success = await _edgeGapConnect.CreateSession();
            if (!success)
            {
                _statusText.text = "CREATE PROCESS WAS UNSUCCESSFULL";
                _hostButton.interactable = false;
                _joinButton.interactable = false;
            }
            else
            {
                _statusText.text = "CREATE PROCESS WAS SUCCESSFULL";
            }*/
        }

        private async void JoinSession()
        {
            _joinStatusText.text = "WAITING FOR RESPONSE ...";
            Debug.Log("Joined Session");
            /*bool success = await _edgeGapConnect.JoinSession(_code);
            if (!success)
            {
                _joinStatusText.text = "JOIN PROCESS WAS UNSUCCESSFULL";
                _cancelButton.interactable = true;
            }
            else
                _joinStatusText.text = "JOIN PROCESS WAS SUCCESSFULL";*/
        }
    }
}
