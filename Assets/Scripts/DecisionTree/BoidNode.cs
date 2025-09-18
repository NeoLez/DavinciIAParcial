using Boids;
using UnityEngine;

namespace DecisionTree {
    public abstract class BoidNode : MonoBehaviour
    {
        public abstract void Execute(Boid npc);
    }
}
