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
        [SerializeField] public GameObject player;
        [SerializeField] public LayerMask layerMask;

        private void Awake() {
            Instance = this;
            nodes = nodeStartList.ToHashSet();
        }

        public void AddNode(Node node) {
            nodes.Add(node);
        }

        public bool ContainsNode(Node node) {
            return nodes.Contains(node);
        }

        public void UpdateNode(Node node) {
            if (!nodes.Contains(node)) return;
            
            
            List<Node> visibleNodes = FindAllAccessibleNodes(node);
            
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
        
        public Stack<Node> CalculatePath(Node start, Node goal) {
            if(!nodes.Contains(start) || !nodes.Contains(goal)) return null;
            
            Stack<Node> steps = new();
            PriorityQueue<Node> frontier = new();
            frontier.Enqueue(start, 0);
            Dictionary<Node, Node> came_from = new();
            Dictionary<Node, float> cost_so_far = new();
            cost_so_far.Add(start, 0);

            while (frontier.Count != 0) {
                Node current = frontier.Dequeue();

                if (current == goal) break;

                foreach (var next in current.neighbours) {
                    float new_cost = cost_so_far[current] + CalculateCost(current, next);
                    if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next]) {
                        cost_so_far[next] = new_cost;
                        frontier.Enqueue(next, new_cost + CalculateHeuristic(goal, next));
                        came_from[next] = current;
                    }
                }
            }

            if (cost_so_far.ContainsKey(goal)) {
                Node current = goal;
                steps.Push(goal);
                while (current != start) {
                    current = came_from[current];
                    steps.Push(current);
                }
            }

            if (steps.Count == 0) {
                Debug.Log("No path found");
                Debug.Log(start.gameObject.name);
                Debug.Log(goal.gameObject.name);
            }
            return steps;
        }

        private float CalculateCost(Node a, Node b) {
            return Vector2.Distance(a.transform.position, b.transform.position);
        }

        private float CalculateHeuristic(Node a, Node b) {
            return Vector2.Distance(a.transform.position, b.transform.position);
        }
    }
}