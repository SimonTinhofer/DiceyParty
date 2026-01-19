using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.GrabABox
{
    public class CollisionHandler : NetworkBehaviour
    {
        [SerializeField] private Collider _collider;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner)
                return;

            _collider.enabled = true;
        }
    }
}