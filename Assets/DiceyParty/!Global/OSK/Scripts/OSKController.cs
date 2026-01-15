using System;
using TMPro;
using UnityEngine;

namespace DiceyParty
{
    public class OSKController : MonoBehaviour
    {
        public static OSKController Instance;
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private GlobalConfigSO _globalConfig;

        public static Action<bool> OnToggleShift; 

        private void Awake()
        {
            if(Instance!= null)
                Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            OnToggleShift = null;
        }

        public void DeleteLetter()
        {
            if(_input.text.Length != 0) {
                _input.text = _input.text.Remove(_input.text.Length - 1, 1);
            }
        }

        public void AddLetter(string letter)
        {
            if(_input.text.Length < _globalConfig.MaxNameLength)
                _input.text += letter;
        }

        public static void ToggleShift(bool toggle)
        {
            OnToggleShift?.Invoke(toggle);
        }
    }
}