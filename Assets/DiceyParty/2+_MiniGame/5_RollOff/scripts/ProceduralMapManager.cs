using System;
using System.Collections.Generic;
using System.IO;
using FishNet.Object;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DiceyParty.MiniGame.RollOff
{
    public class ProceduralMapManager : NetworkBehaviour
    {
        [SerializeField] private RollOffConfigSO _gameConfig;
        [SerializeField] private Transform _mapTransform;
        [SerializeField] private GameObject _pathBlockPrefab;

        private List<Vector3> _waypoints = new();

        private Vector3[] _directions;
        private int _currentDirectionIndex = -1;
        private int _currentLevel;

        public override void OnStartServer()
        {
            _directions = new[] { new Vector3(-1, 0, 1), new Vector3(1, 0, 1)};
            _waypoints.Add(new Vector3(0,0,0));
            var pathBlocks = CreatePathBlocks();
            CreateMap(pathBlocks);
        }

        [ObserversRpc (BufferLast = true)]
        private void CreateMap(List<PathBlock> pathBlocks)
        {
            foreach (PathBlock block in pathBlocks)
            {
                GameObject go = Instantiate(_pathBlockPrefab, _mapTransform);
                go.transform.position = block.Position;
                go.transform.localScale = block.Scale;
                go.transform.rotation = block.Rotation;
            }
        }

        private List<PathBlock> CreatePathBlocks()
        {
            List<PathBlock> pathBlocks = new();
            int i = 0;
            while (_currentLevel < _gameConfig.MaxLevel)
            {
                _currentLevel++;
                while (_waypoints[i].z < _gameConfig.LevelLength * _currentLevel)
                {
                    Vector3 nextWaypoint = CreateNextWaypoint(_waypoints[i]);
                    _waypoints.Add(nextWaypoint);
                    pathBlocks.Add(CreateNextPathBlock(_waypoints[i], nextWaypoint));
                    i++;
                }
            }
            return pathBlocks;
        }

        private Vector3 CreateNextWaypoint(Vector3 lastWaypoint)
        {
            if(_currentDirectionIndex == -1)
                _currentDirectionIndex = UnityEngine.Random.Range(0, 2);
            else
            {
                _currentDirectionIndex = (_currentDirectionIndex + 1) % 2;
            }
            
            if (lastWaypoint.x + _directions[_currentDirectionIndex].x * _gameConfig.MaxWaypointDistanceMultiplyer > _gameConfig.LateralBoundary)
            {
                _currentDirectionIndex = 0;
            }
            else if (lastWaypoint.x + _directions[_currentDirectionIndex].x * _gameConfig.MaxWaypointDistanceMultiplyer < -_gameConfig.LateralBoundary)
            {
                _currentDirectionIndex = 1;
            }
            
            return lastWaypoint + _directions[_currentDirectionIndex] * UnityEngine.Random.Range(_gameConfig.MinBlockWaypointDistanceMultiplayer, _gameConfig.MaxWaypointDistanceMultiplyer);
        }

        private PathBlock CreateNextPathBlock(Vector3 lastWaypoint, Vector3 nextWaypoint)
        {
            Vector3 diffrence = nextWaypoint - lastWaypoint;
            PathBlock pathBlock = new();
            pathBlock.Position = (lastWaypoint + nextWaypoint) / 2;
            float width = 2.1f - (0.2f * _currentLevel);
            float length = diffrence.magnitude + width;
            pathBlock.Scale = new Vector3(width, 0.2f, length);
            
            //going left
            if (_currentDirectionIndex == 0)
                pathBlock.Rotation.eulerAngles = new Vector3(0, -45, 0);
            //going right
            else if(_currentDirectionIndex == 1)
                pathBlock.Rotation.eulerAngles = new Vector3(0, 45, 0);
            
            return pathBlock;
        }
    }

    public class PathBlock
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public PathBlock()
        {
        }
    }
}