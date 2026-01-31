using System;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.TugTheRope
{
    public class RopeController : NetworkBehaviour
    {
        [SerializeField] private TugTheRopeConfig _gameConfig;
        private float _target;
        private float _stepSize;
        [SerializeField] float _goal = 5f;
        private bool _teamWon;
        
        private void Start()
        {
            _stepSize = _gameConfig.BaseStepSize;
            _target = transform.position.x;
        }

        public void AddTug(Team team, int teamSize)
        {
            int direction = team == Team.RightTeam ? 1 : -1;
            _target += direction * (_stepSize / teamSize);
            _stepSize += _gameConfig.StepSizeGrowth;
        }

        private void Update()
        {
            if(!IsServerStarted) return;
            if(_teamWon) return;
            if (Mathf.Abs(transform.position.x) > _goal)
            {
                Team winnerTeam = transform.position.x < 0 ? Team.LeftTeam : Team.RightTeam;
                TugTheRopeManager.Instance.TeamWon(winnerTeam);
                _teamWon = true;

            }
            if (Mathf.Abs(_target - transform.position.x) < 0.05f)
            {
                _target = transform.position.x;
            }
            float k = 1f - Mathf.Exp(-_gameConfig.Sharpness * Time.deltaTime);
            transform.position = new(Mathf.Lerp(transform.position.x, _target, k), transform.position.y, transform.position.z);
        }
    }
}