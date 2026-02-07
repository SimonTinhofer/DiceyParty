using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty
{
    public class SessionAnalyticsSystem : NetworkBehaviour
    {
        public static SessionAnalyticsSystem Instance;
        private string _deploymentId;
        private Dictionary<int, string> _sceneIdMiniGames = new()
        {
            {2, "PaintTheBall"},
            {3, "GrabABox"},
            {4, "CoinDilemma"},
            {5, "RollOff"},
            {6, "QuickMath"},
            {7, "TugTheRope"},
        };
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("there should only be one instantiated objects of this class in a scene");
                Destroy(this.gameObject);
                return;
            }
        
            Instance = this;
        }

        public void Setup(string deploymentId)
        {
            _deploymentId = deploymentId;
        }

        public void SessionStarted()
        {
            PostHog.Capture(
                eventName: "session_started",
                distinctId: _deploymentId
            );
        }

        public void SessionStopped()
        {
            PostHog.Capture(
                eventName: "session_stopped",
                distinctId: _deploymentId
            );
        }

        public void JoinAttempt()
        {
            PostHog.Capture(
                eventName: "join_attempt",
                distinctId: _deploymentId
            );
        }

        public void PlayerJoined()
        {
            PostHog.Capture(
                eventName: "player_joined",
                distinctId: _deploymentId
            );
        }

        public void PlayerLeft()
        {
            PostHog.Capture(
                eventName: "player_left",
                distinctId: _deploymentId
                //add reason
            );
        }

        public void MiniGameStarted()
        {
            PostHog.Capture(
                eventName: "minigame_started",
                distinctId: _deploymentId
            );
        }

        public void MiniGameStopped(int sceneId)
        {
            PostHog.Capture(
                eventName: "minigame_stopped",
                distinctId: _deploymentId,
                new Dictionary<string, object>
                {
                    {"minigame", _sceneIdMiniGames[sceneId]}
                    //add reason
                }
            );
        }
    }
}