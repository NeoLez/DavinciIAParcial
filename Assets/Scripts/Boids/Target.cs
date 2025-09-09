using UnityEngine;

namespace Boids {
    public class Target : MonoBehaviour {
        public Vector2 position;
        public float radius;

        private void Update() {
            position = transform.position;
        }
    }
}