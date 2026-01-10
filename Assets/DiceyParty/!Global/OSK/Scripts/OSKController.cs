using System;
using TMPro;
using UnityEngine;

namespace DiceyParty
{
    public class OSKController : MonoBehaviour
    {
        public static OSKController Instance;
        [SerializeField] TMP_InputField _input;

        private void Awake()
        {
            if(Instance!= null)
                Destroy(this.gameObject);
            else
            {
                Instance = this;
            }
        }

        public void DeleteLetter()
        {
            if(_input.text.Length != 0) {
                _input.text = _input.text.Remove(_input.text.Length - 1, 1);
            }
        }

        public void AddLetter(string letter)
        {
            _input.text += letter;
        }

        public void SubmitWord()
        {
            /*Debug.Log("Text submitted successfully!");*/
        }
    }
}