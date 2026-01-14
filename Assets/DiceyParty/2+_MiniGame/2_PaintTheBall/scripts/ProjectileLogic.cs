using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class ProjectileLogic : MonoBehaviour
    {
        [SerializeField] private PaintTheBallConfigSO _paintTheBallConfig;
        [SerializeField] private Renderer _renderer;

        public void PassColor(Color newColor)
        {
            _renderer.material.color = newColor;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            ProjectileHitDetection painter = collision.collider.GetComponent<ProjectileHitDetection>();
            if (painter != null)
            {
                ContactPoint contact = collision.contacts[0];
                painter.DetectHitTriangles(contact.point, _paintTheBallConfig.PaintRadius);
            }
            
            Destroy(gameObject);
        }
        
    }

}
