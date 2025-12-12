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

        private void Start()
        {
            _hostButton.onClick.AddListener(HostButtonClicked);
            _joinButton.onClick.AddListener(JoinButtonClicked);
            _cancelButton.onClick.AddListener(CancelButtonClicked);

            _connectionContainer.SetActive(true);
            _hostButton.interactable = true;
            _joinButton.interactable = true;

            _joinContainer.SetActive(false);
            _cancelButton.interactable = true;

            _SynchronizeActive = true;
            _ = SynchronizeSessions();
        }

        private void OnDestroy()
        {
            _SynchronizeActive = false;
        }

        private async Awaitable SynchronizeSessions()
        {

            while (_SynchronizeActive)
            {
                var sessions= await _edgeGapConnect.FetchSessions();
                UpdateSessionList(sessions);
                await Awaitable.WaitForSecondsAsync(3);
            }
        }

        private void UpdateSessionList(List<Session> sessions)
        {
            Dictionary<string, Session> newSessions = new();
            HashSet<string> processedIds = new();

            foreach (Session s in sessions)
            {
                if (newSessions.ContainsKey(s.RequestId)) continue;

                newSessions.Add(s.RequestId, s);
                processedIds.Add(s.RequestId);

                if (!_sessions.ContainsKey(s.RequestId))
                {
                    CreateSessionCard(s);
                }
            }

            // Clean up anything in _sessions that wasn't in the new list
            foreach (var existingId in _sessions.Keys)
            {
                if (!processedIds.Contains(existingId))
                {
                    DestroySessionCard(existingId);
                }
            }

            _sessions = newSessions;
        }

        private void CreateSessionCard(Session session)
        {
            string requestId = session.RequestId;
            
            GameObject go = Instantiate(_sessionCardPrefab, _sessionCardParent);
            SessionCardHandler handler = go.GetComponent<SessionCardHandler>();
            _sessionCards.Add(requestId, handler);
            
            Button button = handler.Setup(session.RequestId);
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

        private async void CreateSession()
        {
            _waitingForCreation = true;
            SessionCreationStatusUpdater();
            bool success = await _edgeGapConnect.CreateSession();
            _waitingForCreation = false;
            if (!success)
            {
                _statusText.text = "Error occured during session creation";
                _hostButton.interactable = false;
                _joinButton.interactable = false;
            }
            else
            {
                _statusText.text = "Successfully hosted session";
            }
        }

        private async void SessionCreationStatusUpdater()
        {
            int counter = 0;
            string[] loadingIndicator = { "", " .", " ..", " ..." };
            float timeGap = 0.5f;
            while (_waitingForCreation)
            {
                int i = counter % 4;
                _statusText.text = "Hosting session (might take a while)" + loadingIndicator[i];
                counter++;
                await Awaitable.WaitForSecondsAsync(timeGap);
            }
        }
    }
}
