using System;
using UnityEngine;

namespace DiceyParty.MiniGame.RollOff
{
    public class SyncTransformPosition : MonoBehaviour
    {
        [SerializeField] private Transform _transformGetPos;
        [SerializeField] private Transform _transformSetPos;
        private Vector3 _offset;

        private void Start()
        {
            _offset = _transformSetPos.position - _transformGetPos.position;
        }

        private void Update()
        {
            _transformSetPos.position = _offset + _transformGetPos.position;
        }
    }
}