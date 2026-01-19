using System;
using FishNet.Object;
using TMPro;
using UnityEngine;

namespace DiceyParty.MiniGame
{
    public class PlayerNameTag : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _nameTag;

        public override void OnStartClient()
        {
            base.OnStartClient();
            _nameTag.text = SessionDataSystem.Instance.GetPlayerData()[OwnerId].Name;
        }
    }
}