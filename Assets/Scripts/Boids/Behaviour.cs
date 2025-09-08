using System;
using UnityEngine;

namespace Boids {
    [Serializable]
    public abstract class Behaviour {
        public float BehaviourWeight;
        public abstract Vector2 GetResultingVector();
    }
}