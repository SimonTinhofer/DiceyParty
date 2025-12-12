using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Connection;

namespace DiceyParty
{
    public class SessionStageSystem : NetworkBehaviour
    {
        private static SessionStageSystem _instance;

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

        public override void OnStartServer()
        {
            base.OnStartClient();
            ChangeState(SessionStage.Lobby);
        }

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
                    LoadSceneByIndex(2);
                    break;
                
                default:
                    throw new NotImplementedException();
            }
        }

        private void LoadSceneByIndex (int sceneIndex)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
            SceneLoadData sld = new SceneLoadData(sceneName);
            sld.ReplaceScenes = ReplaceOption.All;
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

