using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceyParty.MiniGame.PaintTheBall
{
    public class ProjectileLogic : MonoBehaviour
    {
        [SerializeField] private PaintTheBallConfigSO _paintTheBallConfig;

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("collision: " + collision);
            ProjectileHitDetection painter = collision.collider.GetComponent<ProjectileHitDetection>();
            if (painter != null)
            {
                ContactPoint contact = collision.contacts[0];
                painter.DetectHitTriangles(contact.point, _paintTheBallConfig.PaintRadius);
            }

            Destroy(gameObject); // Projektil entfernen
        }
    }

}
