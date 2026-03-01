using System;
using System.Collections.Generic;
using UnityEngine;

namespace Final.Scripts
{
    public class Point : MonoBehaviour
    {
        public List<Point> neighbours = new();
        public List<Point> visiblePoints = new();
        public bool outConnections;
        public bool inConnections;

        private void OnDrawGizmos() {
            if (!Application.isPlaying || !PointManager.Instance.drawLines || neighbours == null) return; 
            foreach (var point in neighbours) {
                Gizmos.DrawLine(transform.position, point.transform.position);
            }
        }

        private void Start()
        {
            PointManager.Instance.UpdatePoint(this);
        }
    }
}