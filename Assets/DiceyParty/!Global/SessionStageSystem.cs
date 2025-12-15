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
        public static void ChangeState(SessionStage state) => _instance.HandleChangeState(state);

        private void HandleChangeState(SessionStage state)
        {
            CheckIfServer();

            switch (state)
            {
                case SessionStage.Lobby:
                    LoadSceneByIndex(1);
                    break;

                case SessionStage.MiniGame:
                    LoadSceneByIndex(_nextMiniGameSceneId);
                    break;
                
                default:
                    throw new NotImplementedException();
            }
        }

        private void LoadSceneByIndex (int sceneIndex)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
            SceneLoadData sld = new SceneLoadData(sceneName)
            {
                ReplaceScenes = ReplaceOption.All
            };
            NetworkManager.SceneManager.LoadGlobalScenes(sld);
        }

        private void CheckIfServer()
        {
            if (!IsServerInitialized)
                throw new Exception("This method can not be called on the client");
        }
    }
    public enum SessionStage
    {
        Lobby,
        MiniGame,
    }
}

