using System;
using System.Collections.Generic;
using System.Linq;
using DiceyParty;
using DiceyParty.MiniGame.TugTheRope;
using FishNet.Object;
using UnityEngine;

public class GraphicsManager : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _playerParent;
    [SerializeField] private List<Transform> _spawnPointsTeamLeft;
    [SerializeField] private List<Transform> _spawnPointsTeamRight;
    private Dictionary<int, PlayerHandler> _playerHandlers = new();
    private List<int> _idsTeamLeft = new();
    private List<int> _idsTeamRight = new();
    
    public void ShowPlayers(Dictionary<int, Team> playerTeams, float leftTeamSize, float rightTeamSize)
    {
        ShowPlayersObservers(playerTeams, leftTeamSize, rightTeamSize, new Dictionary<int, PlayerInfo>(SessionDataSystem.Instance.GetPlayerData()));
        
    }

    [ObserversRpc(RunLocally = true, BufferLast = true)]
    private void ShowPlayersObservers(Dictionary<int, Team> playerTeams, float leftTeamSize, float rightTeamSize, Dictionary<int, PlayerInfo> playerInfos)
    {
        foreach (var entry in playerTeams)
        {
            var clientId = entry.Key;
            var team = entry.Value;
            var spawnPoint = GetPlayerSpawnPoint(clientId, team);
            float multiplyer = 1;
            if (team == Team.LeftTeam)
            {
                if (rightTeamSize / leftTeamSize > 1)
                    multiplyer = Mathf.Sqrt(rightTeamSize / leftTeamSize);
            }
            else if (team == Team.RightTeam)
            {
                if (leftTeamSize / rightTeamSize > 1)
                    multiplyer = Mathf.Sqrt(leftTeamSize / rightTeamSize);
            }
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
