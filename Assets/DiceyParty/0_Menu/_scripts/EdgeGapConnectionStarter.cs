using System;
using System.Collections.Generic;
using FishNet.Managing;
using UnityEngine;
using FishNet.Transporting.Bayou;
using Newtonsoft.Json;
using UnityEngine.Networking;



namespace DiceyParty.Menu
{
    public class EdgeGapConnectionStarter : MonoBehaviour
    {
        private readonly string _backendBaseUrl = "https://diceypartyapi.onrender.com";
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private Bayou _transport;
        [SerializeField] private SessionSystemSpawner _sessionSystemSpawner;

        public async Awaitable<bool> CreateSession()
        {
            
            try
            {
                var createResponse = await PostSessions();
                if(createResponse == null) return false;
                _sessionSystemSpawner._isHost = true;
                _sessionSystemSpawner._sessionId = createResponse.RequestId;
                return JoinSession(createResponse.Host, createResponse.Port);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
            
        }

        public async Awaitable<List<Session>> FetchSessions( )
        {
            try
            {
                var fetchResponse = await GetSessions();
                return fetchResponse;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return null;
            }
        }

        public bool JoinSession(string host, int port)
        {
            _transport.SetClientAddress(host);
            ushort port32 = (ushort)port;
            _transport.SetPort(port32);

            return _networkManager.ClientManager.StartConnection();
        }

        private async Awaitable<Session> PostSessions()
        {
            using var request = UnityWebRequest.Post($"{_backendBaseUrl}/sessions", "", "application/json");
            await request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Network Error: {request.error} | Response: {request.downloadHandler.text}"); 
            }
            
            return JsonConvert.DeserializeObject<Session>(request.downloadHandler.text);
        }
        
        private async Awaitable<List<Session>> GetSessions()
        {
            using var request = UnityWebRequest.Get($"{_backendBaseUrl}/sessions");
            await request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Network Error: {request.error} | Response: {request.downloadHandler.text}");
            }
            
            return JsonConvert.DeserializeObject<List<Session>>(request.downloadHandler.text);
        }

        
    }
    public class Session { 
        public string RequestId; 
        public string Host; 
        public int Port;
        public int PlayerCount;
    }
}

