using UnityEngine;

namespace Boids {
    /// <summary>
    /// Marks a <c>MonoBehaviour</c> as a target and determines its collision radius.
    /// </summary>
    public class Target : MonoBehaviour {
        [SerializeField] private Color gizmoColor;
        public float radius;

        private void OnDrawGizmos() {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}