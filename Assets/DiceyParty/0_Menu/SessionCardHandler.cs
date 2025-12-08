using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.Menu
{
    public class SessionCardHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _playerCount;
        [SerializeField] private Button _joinButton;

        public Button Setup(string sessionName, int playerCount)
        {
            _name.text = sessionName;
            _playerCount.text = "p: " + playerCount;
            return _joinButton;
        }

        public void UpdatePlayerCount(int playerCount)
        {
            _playerCount.text = "p: " + playerCount;
        }
    }
}

