using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.Menu
{
    public class SessionCardHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Button _joinButton;

        public Button Setup(string sessionName)
        {
            _name.text = sessionName;
            return _joinButton;
        }
    }
}

