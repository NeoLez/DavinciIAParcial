using System;
using UnityEngine;

namespace Boids {
    [Serializable]
    public class AlignmentBehaviour : Behaviour {
        public float SearchRange;
        public override Vector2 GetResultingVector() {
            throw new System.NotImplementedException();
        }
    }
}