using System;
using System.Collections.Generic;
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

        [SerializeField] private GameObject _sessionNameContainer;
        [SerializeField] private Button _sessionNameCancelButton;
        [SerializeField] private Button _sessionNameConfirmButton;
        [SerializeField] private TMP_InputField _sessionNameInput;
        
        [SerializeField] private GameObject _joinContainer;
        [SerializeField] private TMP_Text _joinStatusText;
        [SerializeField] private TMP_Text _joinInfoText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private GameObject _sessionCardPrefab;
        [SerializeField] private Transform _sessionCardParent;

        [SerializeField] private EdgeGapConnectionStarter _edgeGapConnect;

        private bool _SynchronizeActive;
        private Dictionary<string, Session> _sessions = new();
        private Dictionary<string, SessionCardHandler> _sessionCards = new();
        private Dictionary<string, Button> _sessionButtons = new();
        private bool _waitingForCreation;
        private string _sessionName;

        private void Start()
        {
            _hostButton.onClick.AddListener(HostButtonClicked);
            _joinButton.onClick.AddListener(JoinButtonClicked);
            _cancelButton.onClick.AddListener(CancelButtonClicked);
            
            _sessionNameInput.onValueChanged.AddListener(SessionNameInputChanged);
            _sessionNameCancelButton.onClick.AddListener(SessionNameCancelButtonClicked);
            _sessionNameConfirmButton.onClick.AddListener(SessionNameConfirmButtonClicked);

            _connectionContainer.SetActive(true);
            _hostButton.interactable = true;
            _joinButton.interactable = true;

            _joinContainer.SetActive(false);
            _cancelButton.interactable = true;

            _SynchronizeActive = true;
            TrySynchronizeSessions();
        }

        private void OnDestroy()
        {
            _SynchronizeActive = false;
        }

        private async void TrySynchronizeSessions()
        {
            try
            {
                await SynchronizeSessions();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"{this.name}.SynchronizeSessions failed: {e.Message}");
            }
        }
        
        private async Awaitable SynchronizeSessions()
        {

            while (_SynchronizeActive)
            {
                var sessions= await _edgeGapConnect.FetchSessions();
                UpdateSessionList(sessions);
                await Awaitable.WaitForSecondsAsync(3, destroyCancellationToken);
            }
        }

        private void UpdateSessionList(List<Session> sessions)
        {
            Dictionary<string, Session> newSessions = new();
            HashSet<string> processedSessionNames = new();
            if(sessions.Count > 0)
                Debug.Log(sessions[0].Name);
            foreach (Session s in sessions)
            {
                if (!newSessions.TryAdd(s.Name, s)) continue;

                processedSessionNames.Add(s.Name);

                if (!_sessions.ContainsKey(s.Name))
                {
                    CreateSessionCard(s);
                }
            }

            // Clean up anything in _sessions that wasn't in the new list
            foreach (var existingName in _sessions.Keys)
            {
                if (!processedSessionNames.Contains(existingName))
                {
                    DestroySessionCard(existingName);
                }
            }

            _sessions = newSessions;
        }

        private void CreateSessionCard(Session session)
        {
            string requestId = session.Name;
            
            GameObject go = Instantiate(_sessionCardPrefab, _sessionCardParent);
            SessionCardHandler handler = go.GetComponent<SessionCardHandler>();
            _sessionCards.Add(requestId, handler);
            
            Button button = handler.Setup(session.Name);
            button.onClick.AddListener(() => SessionButtonClicked(requestId));
            _sessionButtons.Add(requestId, button);
        }

        private void DestroySessionCard(string requestId)
        {
            _sessionButtons.Remove(requestId);
            Destroy(_sessionCards[requestId].gameObject);
            _sessionCards.Remove(requestId);
        }

        #region UI

            #region Host

            private void HostButtonClicked()
            {
                _hostButton.interactable = false;
                _joinButton.interactable = false;
                _sessionNameContainer.SetActive(true);
            }
        
            private void SessionNameInputChanged(string newInput)
            {
                if (newInput.Length < 1)
                    _sessionNameConfirmButton.interactable = false;
                else
                {
                    _sessionNameConfirmButton.interactable = true;
                    _sessionName = newInput;
                }

            }

            private void SessionNameCancelButtonClicked()
            {
                _sessionNameInput.text = "";
                _sessionNameConfirmButton.interactable = false;
                _sessionNameContainer.SetActive(false);
                _hostButton.interactable = true;
                _joinButton.interactable = true;
            }

            private void SessionNameConfirmButtonClicked()
            {
                _sessionNameInput.text = "";
                _sessionNameConfirmButton.interactable = false;
                _sessionNameContainer.SetActive(false);
                CreateSession();
            }

            #endregion

            #region  Join

            private void JoinButtonClicked()
            {
                _connectionContainer.SetActive(false);
                _joinContainer.SetActive(true);
                Debug.Log("Opened JoinContainer");
            }
        
            private void SessionButtonClicked(string requestId)
            {
                foreach (var entry in  _sessionButtons)
                {
                    entry.Value.enabled = false;
                }

                var session = _sessions[requestId];
                Debug.Log(session.Host + session.Port);
                _edgeGapConnect.JoinSession(session.Host, session.Port);
            }

            private void CancelButtonClicked()
            {
                _joinContainer.SetActive(false);
                _connectionContainer.SetActive(true);
            }

            #endregion
        
        #endregion

        private async void CreateSession()
        {
            _waitingForCreation = true;
            TrySessionCreationStatusUpdater();
            bool success = await _edgeGapConnect.CreateSession(_sessionName);
            _waitingForCreation = false;
            if (!success)
            {
                _statusText.text = "Error occured during session creation";
                _hostButton.interactable = true;
                _joinButton.interactable = true;
            }
            else
            {
                _statusText.text = "Successfully hosted session";
            }
        }

        private async void TrySessionCreationStatusUpdater()
        {
            try
            {
                await SessionCreationStatusUpdater();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"{this.name}.SessionCreationStatusUpdater failed: {e.Message}");
            }
        }
        
        private async Awaitable SessionCreationStatusUpdater()
        {
            int counter = 0;
            string[] loadingIndicator = { "", " .", " ..", " ..." };
            float timeGap = 0.5f;
            while (_waitingForCreation)
            {
                int i = counter % 4;
                _statusText.text = "Hosting session (might take a while)" + loadingIndicator[i];
                counter++;
                await Awaitable.WaitForSecondsAsync(timeGap, destroyCancellationToken);
            }
        }
    }
}
