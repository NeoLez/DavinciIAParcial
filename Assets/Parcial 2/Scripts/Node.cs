using System.Collections.Generic;
using UnityEngine;

namespace Parcial_2.Scripts {
    public class Node : MonoBehaviour {
        public List<Node> neighbours = new();
        public bool outConnections;
        public bool inConnections;

        private void OnDrawGizmos() {
            if (neighbours == null) return; 
            foreach (var node in neighbours) {
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }

        private void Update() {
            for (int i=neighbours.Count - 1; i >= 0; i--) {
                Node node = neighbours[i];
                if (!NodeManager.Instance.AreNodesInView(this, node)) {
                    neighbours.Remove(node);
                }
            }
        }
    }
}