using System;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class TriangleManager : NetworkBehaviour
    {
        private int _clientId;
        private float _startTime;
        
        private Dictionary<int, int> _playerTriangleCount = new();
        private Dictionary<int, IcoTriangle> _claimedTriangles = new();
        
        private static TriangleManager _instance;
        [SerializeField] private TriangleHandler _triangleHandler;
        

        private void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Instance should be null");
            }
            _instance = this;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _clientId = ClientManager.Connection.ClientId;
            _startTime = Time.time;
            AddPlayerTriangleCount(_clientId);
        }

        [ServerRpc (RequireOwnership = false)]
        private void AddPlayerTriangleCount(int clientId)
        {
            _playerTriangleCount.Add(clientId, 0);
        }


        public static void LocalTrianglesHitClient(List<IcoTriangle> hitTriangles) => _instance.HandleLocalTrianglesHitClient(hitTriangles);

        [Client]
        private void HandleLocalTrianglesHitClient(List<IcoTriangle> hitTriangles)
        {
            foreach (IcoTriangle triangle in hitTriangles)
            {
                triangle.Owner = _clientId;
                triangle.HitTime = Time.time - _startTime;
            }
            OnHitTriangleServer(hitTriangles);
            _triangleHandler.RequestColorChange(hitTriangles);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnHitTriangleServer(List<IcoTriangle> hitTriangles)
        {
            List<IcoTriangle> relevantHits = new();
            foreach (IcoTriangle triangle in hitTriangles)
            {
                var prevTriangle = _claimedTriangles.GetValueOrDefault(triangle.ID);
                if (prevTriangle == null || prevTriangle.HitTime < triangle.HitTime)
                {
                    _claimedTriangles[triangle.ID] = triangle;
                    relevantHits.Add(triangle);

                    _playerTriangleCount[triangle.Owner]++;
                    if(prevTriangle != null)
                    {
                        _playerTriangleCount[prevTriangle.Owner]--;
                    }
                }
            }
            if (relevantHits.Count > 0)
                HandleTriangleHitObserver(relevantHits, _playerTriangleCount);
        }
        

        [ObserversRpc]
        private void HandleTriangleHitObserver(List<IcoTriangle> hitTriangles, Dictionary<int, int> playerTriangleCount)
        {
            if (hitTriangles[0].Owner != _clientId)
                _triangleHandler.RequestColorChange(hitTriangles);
            UIManager.UpdateScoreboard(playerTriangleCount);
        }

        public static Dictionary<int, int> GetPlayerTriangleCount()
        {
            return _instance._playerTriangleCount;
        }
    }
}