using System;
using UnityEngine;
using UnityEngine.UI;

namespace DiceyParty.Menu
{
    public class LinkHandler : MonoBehaviour
    {
        [SerializeField] private Button _discordButton;
        [SerializeField] private Button _feedbackButton;

        private void Start()
        {
            _discordButton.onClick.AddListener(OpenDiscord);
            _feedbackButton.onClick.AddListener(OpenGoogleForms);
        }

        private void OpenDiscord()
        {
            Application.OpenURL("https://discord.gg/hJ4jcaqadV");
        }

        private void OpenGoogleForms()
        {
            Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSenJ0e9bhVcqkPL8zNCA-7T8O51vjXMJlbDnO7Q-0QzUJ0g7g/viewform?usp=publish-editor");
        }
    }
}