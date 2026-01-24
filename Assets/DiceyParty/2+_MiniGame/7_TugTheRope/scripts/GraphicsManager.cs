using System;
using System.Collections.Generic;
using System.Linq;
using DiceyParty;
using DiceyParty.MiniGame.TugTheRope;
using FishNet.Object;
using UnityEngine;

public class GraphicsManager : NetworkBehaviour
{
    [SerializeField] private Rigidbody _rbToFollow;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerParent;
    [SerializeField] private List<Transform> _spawnPointsTeamLeft;
    [SerializeField] private List<Transform> _spawnPointsTeamRight;
    private Dictionary<int, PlayerHandler> _playerHandlers = new();
    private List<int> _idsTeamLeft = new();
    private List<int> _idsTeamRight = new();
    
    private void FixedUpdate()
    {
        if(!IsServerStarted) return;
        transform.position = new Vector3(_rbToFollow.position.x, transform.position.y, _rbToFollow.position.z);
    }

    public void ShowPlayers(Dictionary<int, Team> playerTeams, float leftMultiplyer, float rightMuliplyer)
    {
        ShowPlayersObservers(playerTeams, leftMultiplyer, rightMuliplyer, new Dictionary<int, PlayerInfo>(SessionDataSystem.Instance.GetPlayerData()));
        
    }

    [ObserversRpc(RunLocally = true, BufferLast = true)]
    private void ShowPlayersObservers(Dictionary<int, Team> playerTeams, float leftMultiplyer, float rightMuliplyer, Dictionary<int, PlayerInfo> playerInfos)
    {
        foreach (var entry in playerTeams)
        {
            var clientId = entry.Key;
            var team = entry.Value;
            var spawnPoint = GetPlayerSpawnPoint(clientId, team);
            var multiplyer = team == Team.LeftTeam ? leftMultiplyer : rightMuliplyer;
            SpawnPlayer(clientId, spawnPoint.localPosition, spawnPoint.localRotation, playerInfos[clientId], multiplyer);
        }
    }

    private Transform GetPlayerSpawnPoint(int clientId, Team team)
    {
        if (team == Team.LeftTeam)
        {
            _idsTeamLeft.Add(clientId);
            return _spawnPointsTeamLeft[_idsTeamLeft.Count - 1];
        }
        if (team == Team.RightTeam)
        {
            _idsTeamRight.Add(clientId);
            return _spawnPointsTeamRight[_idsTeamRight.Count - 1];
        }
        throw new ArgumentOutOfRangeException();
    }

    private void SpawnPlayer(int clientId, Vector3 spawnPointPosition, Quaternion spawnPointRotation, PlayerInfo p, float multiplyer)
    {
        var go = Instantiate(_playerPrefab, spawnPointPosition, _playerPrefab.transform.rotation, _playerParent);
        var playerHandler = go.GetComponent<PlayerHandler>();
        string playerName = LocalConnection.ClientId == p.ClientId ? "You" : p.Name;
        playerHandler.Setup(playerName, p.ColorIndex, spawnPointRotation, multiplyer);
        _playerHandlers.Add(clientId, playerHandler);
    }

    public void UpdateTugTextServer(int clientId, int tugs)
    {
        UpdateTugTextObservers(clientId, tugs);
    }

    [ObserversRpc(RunLocally = true, BufferLast = true)]
    private void UpdateTugTextObservers(int clientId, int tugs)
    {
        _playerHandlers[clientId].UpdateTugsText(tugs);
    }
}
