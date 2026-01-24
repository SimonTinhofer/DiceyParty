using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceyParty.MiniGame.TugTheRope
{
    public class RopeController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private TugTheRopeConfig _gameConfig;
        private bool _readyForTug;
        private Vector3 _netAppliedForce;
        private InputAction _moveAction;

        public void Setup(int playerCount)
        {
            int maxTeamsize = Mathf.FloorToInt((float)(playerCount +1)/2);
            _rb.mass = _gameConfig.BaseMass + maxTeamsize * _gameConfig.MassPerPlayer;
        }
        
        public async void ApplyForce(Vector3 move, float balancingMultiplyer)
        {
            try
            {
                await HandleApplyForce(move, balancingMultiplyer);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Due to GO being destroyed during async operation it was canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"OnStartGamePhase loop failed: {e.Message}");
            }
        }

        private async Awaitable HandleApplyForce(Vector3 move, float balancingMultiplyer)
        {
            _netAppliedForce += move.normalized * _gameConfig.TugForceIncrease * balancingMultiplyer;
            await Awaitable.WaitForSecondsAsync(_gameConfig.TimeApplyTugForceIncrease, destroyCancellationToken);
            _netAppliedForce -= move.normalized * _gameConfig.TugForceIncrease * balancingMultiplyer;
        }

        private void FixedUpdate()
        {
            if(_netAppliedForce.magnitude < 1f) return;
            _rb.AddForce(_netAppliedForce);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Left"))
                TugTheRopeManager.Instance.TeamWon(Team.LeftTeam);
            else if (other.CompareTag("Right"))
                TugTheRopeManager.Instance.TeamWon(Team.RightTeam);
            _rb.Sleep();
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}