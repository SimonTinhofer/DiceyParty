using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DiceyParty
{
    public class KeyboardButtonController : MonoBehaviour
    {
        [SerializeField] Image containerBorderImage;
        [SerializeField] Image containerFillImage;
        [SerializeField] Image containerIcon;
        [SerializeField] TextMeshProUGUI containerText;
        [SerializeField] TextMeshProUGUI containerActionText;
        private bool _isShift;

        private void Start()
        {
            OSKController.OnToggleShift += ShiftToggled;
            SetContainerBorderColor(ColorDataStore.GetKeyboardBorderColor());
            SetContainerFillColor(ColorDataStore.GetKeyboardFillColor());
            SetContainerTextColor(ColorDataStore.GetKeyboardTextColor());
            SetContainerActionTextColor(ColorDataStore.GetKeyboardActionTextColor());
        }

        private void OnDestroy()
        {
            OSKController.OnToggleShift -= ShiftToggled;
        }

        private void ShiftToggled(bool toggle)
        {
            _isShift = toggle;
            if (toggle)
            {
                containerText.text = containerText.text.ToUpper();
            }
            else
            {
                containerText.text = containerText.text.ToLower();
            }
        }

        public void SetContainerBorderColor(Color color) => containerBorderImage.color = color;
        public void SetContainerFillColor(Color color) => containerFillImage.color = color;
        public void SetContainerTextColor(Color color) => containerText.color = color;
        public void SetContainerActionTextColor(Color color) { 
            containerActionText.color = color;
            containerIcon.color = color;
        }

        public void AddLetter() 
        {
            OSKController.Instance.AddLetter(containerText.text);
            if(_isShift)
                OSKController.ToggleShift(false);
        }
        public void DeleteLetter() 
        { 
            OSKController.Instance.DeleteLetter();
        }
        public void ActivateShift() 
        {
            if(!_isShift)
                OSKController.ToggleShift(true);
            else
            {
                OSKController.ToggleShift(false);
            }
        }
    }
}