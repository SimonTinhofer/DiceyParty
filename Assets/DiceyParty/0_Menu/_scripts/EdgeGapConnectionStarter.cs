using System;
using System.Collections.Generic;
using System.Text;
using FishNet.Managing;
using UnityEngine;
using FishNet.Transporting.Bayou;
using Newtonsoft.Json;
using UnityEngine.Networking;



namespace DiceyParty.Menu
{
    public class EdgeGapConnectionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private Bayou _transport;
        [SerializeField] private SessionSystemSpawner _sessionSystemSpawner;

        public async Awaitable<bool> CreateSession(string sessionName)
        {
            
            try
            {
                var createResponse = await BackendAPI.PostSessions(sessionName);
                if(createResponse == null) return false;
                _sessionSystemSpawner.ClientIsHost = true;
                _sessionSystemSpawner.Session = createResponse;
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
                var fetchResponse = await BackendAPI.GetSessions();
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
    }
}

