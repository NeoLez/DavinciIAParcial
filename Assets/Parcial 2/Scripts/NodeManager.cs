using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Parcial_2.Scripts {

    public class NodeManager : MonoBehaviour {
        public static NodeManager Instance { get; private set; }

        [SerializeField] private List<Node> nodeStartList;
        private HashSet<Node> nodes = new();
        [SerializeField] public Node enemyTarget;
        [SerializeField] public Transform player;
        [SerializeField] private LayerMask layerMask;

        private void Awake() {
            Instance = this;
            nodes = nodeStartList.ToHashSet();
        }

        public void AddNode(Node node) {
            nodes.Add(node);
        }

        public void UpdateNode(Node node) {
            if (!nodes.Contains(node)) return;
            
            
            List<Node> visibleNodes = FindAllAccessibleNodes(node);
            Debug.Log(node.name + " Z" +visibleNodes.Count);
            
            if (node.outConnections) {
                node.neighbours = visibleNodes;
            }

            if (node.inConnections) {
                foreach (var n in visibleNodes) {
                    n.neighbours.Add(node);
                }
            }
        }

        public void RemoveNode(Node node) {
            if (!nodes.Contains(node)) return;

            foreach (var n in node.neighbours) {
                n.neighbours.Remove(node);
            }

            nodes.Remove(node);
        }

        public List<Node> FindAllAccessibleNodes(Transform pos) {
            List<Node> result = new();
            foreach (var node in nodes) {
                Vector2 direction = pos.transform.position - node.transform.position;
                if (Physics2D.Raycast(node.transform.position, direction.normalized, direction.magnitude,
                        layerMask)) continue;

                result.Add(node);
            }

            return result;
        }

        public List<Node> FindAllAccessibleNodes(Node baseNode) {
            List<Node> result = new();
            foreach (var node in nodes) {
                if (node == baseNode) continue;
                if (AreNodesInView(baseNode, node))
                    result.Add(node);
            }

            return result;
        }

        public bool AreNodesInView(Node a, Node b) {
            Vector2 direction = a.transform.position - b.transform.position;
            return !Physics2D.Raycast(b.transform.position, direction.normalized, direction.magnitude, layerMask);
        }

        public List<Node> Pathfind(Node start, Node goal) {
            return null;
        }
    }
}