using FishNet.Managing.Scened;
using FishNet.Object;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DiceyParty
{
    public class SessionStageSystem : NetworkBehaviour
    {
        private static SessionStageSystem _instance;
        private int _nextMiniGameSceneId;
        private SessionStage _lastStage = SessionStage.Menu;
        private int _lastSceneIndex;

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning("there should only be one instantiated objects of this class in a scene");
                Destroy(this.gameObject);
            }
            else
                _instance = this;
        }

        public static void SetNextMiniGame(int sceneId) => _instance._nextMiniGameSceneId = sceneId;
        public static void ChangeState(SessionStage stage) => _instance.HandleChangeState(stage);

        private void HandleChangeState(SessionStage stage)
        {
            CheckIfServer();
            switch (stage)
            {
                case SessionStage.Lobby:
                    if(_lastStage == SessionStage.MiniGame)
                        SessionAnalyticsSystem.Instance.MiniGameStopped(_lastSceneIndex);
                    LoadSceneByIndex(1);
                    break;

                case SessionStage.MiniGame:
                    SessionAnalyticsSystem.Instance.MiniGameStarted();
                    LoadSceneByIndex(_nextMiniGameSceneId);
                    break;
            }
            _lastStage = stage;
        }

        public static SessionStage GetCurrentStage()
        {
            return _instance._lastStage;
        }

        private void LoadSceneByIndex (int sceneIndex)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
            SceneLoadData sld = new SceneLoadData(sceneName)
            {
                ReplaceScenes = ReplaceOption.All
            };
            NetworkManager.SceneManager.LoadGlobalScenes(sld);
            _lastSceneIndex = sceneIndex;
        }

        private void CheckIfServer()
        {
            if (!IsServerInitialized)
                throw new Exception("This method can not be called on the client");
        }
    }
    public enum SessionStage
    {
        Menu,
        Lobby,
        MiniGame
    }
}

