using UnityEngine;

namespace Final.Scripts {
    public class TeamBase : MonoBehaviour {
        public Team team;
        public float tickTime;
        public LayerMask entityLayerMask;
        public float radius;
        private float nextTickTime;

        private void Update() {
            if (Time.time >= nextTickTime) {
                nextTickTime = Time.time + tickTime;
                var hits = Physics2D.OverlapCircleAll(transform.position, radius, entityLayerMask);
                foreach (var hit in hits) {
                    var entity = hit.GetComponent<IEntity>();
                    if (entity.GetTeam() == team) {
                        entity.TakeDamage(-1);
                    }
                    else {
                        entity.TakeDamage(1);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}