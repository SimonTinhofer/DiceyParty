using System;
using FishNet.Managing;
using UnityEngine;
using FishNet.Transporting.Bayou;
using UnityEngine.Networking;

namespace DiceyParty.Menu
{
    public class EdgeGapConnectionStarter : MonoBehaviour
    {
        private readonly string _backendBaseUrl = "https://gamebackend-12w8.onrender.com";
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private Bayou _transport;

        private string _code;

        public async Awaitable<bool> CreateSession()
        {
            CreateSessionResponse createResponse = await CreateSessionAsync();
            if(createResponse == null)
            {
                return false;
            }
            else
            {
                Debug.Log(createResponse.joinCode);
                _code = createResponse.joinCode;
                PassCode();
                _ = JoinSession(_code);
                return true;
            }
        }

        private void PassCode()
        {
        }

        public async Awaitable<bool> JoinSession(string code)
        {
            JoinSessionResponse response = await JoinSessionAsync(code);
            if(response == null)
            {
                return false;
            }
            else
            {
                Debug.Log($"host: {response.serverHost}, port {response.serverPort}");

                ushort port = (ushort)response.serverPort;
                string address = response.serverHost;

                _transport.SetPort(port);
                _transport.SetClientAddress(address);
                _networkManager.ClientManager.StartConnection();
                return true;
            }
        }

        private async Awaitable<CreateSessionResponse> CreateSessionAsync()
        {
            string url = _backendBaseUrl + "/sessions";

            var request = new UnityWebRequest(url, "POST");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.uploadHandler = new UploadHandlerRaw(new byte[0]);
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Awaitable.NextFrameAsync();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("CreateSession failed: " + request.error);
                return null;
            }

            return JsonUtility.FromJson<CreateSessionResponse>(request.downloadHandler.text);
        }

        private async Awaitable<JoinSessionResponse> JoinSessionAsync(string code)
        {
            string url = _backendBaseUrl + "/sessions/join";

            JoinRequest body = new JoinRequest { joinCode = code };
            string jsonBody = JsonUtility.ToJson(body);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

            var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Awaitable.NextFrameAsync();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("JoinSession failed: " + request.error);
                return null;
            }

            return JsonUtility.FromJson<JoinSessionResponse>(request.downloadHandler.text);
        }


        [Serializable]
        public class JoinRequest
        {
            public string joinCode;
        }


        [Serializable]
        public class CreateSessionResponse
        {
            public string sessionId;
            public string joinCode;
        }

        [Serializable]
        public class JoinSessionResponse
        {
            public string sessionId;
            public string joinCode;
            public string state;
            public string serverHost;
            public int serverPort;
        }
    }
}

