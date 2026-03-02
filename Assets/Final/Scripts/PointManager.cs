using System;
using System.Collections.Generic;
using System.Linq;
using Parcial_2.Scripts;
using UnityEngine;

namespace Final.Scripts
{
    public class PointManager : MonoBehaviour
    {
        public static PointManager Instance { get; private set; }
        
        [SerializeField] private List<Point> pointStartList;
        private HashSet<Point> _points = new();
        [SerializeField] public LayerMask layerMask;
        public bool drawLines;

        private void Awake()
        {
            Instance = this;
            _points = pointStartList.ToHashSet();
            pointStartList.Clear();
        }
        
        public void AddPoint(Point point) {
            if (_points.Contains(point)) return;
            
            _points.Add(point);
        }

        public bool ContainsPoint(Point point) {
            return _points.Contains(point);
        }

        public void UpdatePoint(Point point) {
            if (!_points.Contains(point)) return;
            List<Point> visiblePoints = FindAllAccessibleNodes(point);

            var excludedPoints = point.visiblePoints.ToHashSet();
            excludedPoints.ExceptWith(visiblePoints);

            foreach (var excludedPoint in excludedPoints)
            {
                excludedPoint.neighbours.Remove(point);
            }

            point.visiblePoints = visiblePoints;
            point.neighbours.Clear();
            
            if (point.outConnections) {
                foreach (var visiblePoint in visiblePoints) {
                    if (visiblePoint.inConnections)
                        point.neighbours.Add(visiblePoint);
                }
            }

            if (point.inConnections) {
                foreach (var n in visiblePoints) {
                    if (n.outConnections && !n.neighbours.Contains(point))
                        n.neighbours.Add(point);
                }
            }
        }

        public void RemovePoint(Point point) {
            if (!_points.Contains(point)) return;
            
            foreach (var p in _points) {
                p.neighbours.Remove(point);
            }
            

            _points.Remove(point);
        }

        public List<Point> FindAllAccessibleNodes(Transform pos) {
            List<Point> result = new();
            foreach (var point in _points) {
                Vector2 direction = pos.transform.position - point.transform.position;
                if (Physics2D.Raycast(point.transform.position, direction.normalized, direction.magnitude,
                        layerMask)) continue;

                result.Add(point);
            }

            return result;
        }

        public List<Point> FindAllAccessibleNodes(Point basePoint) {
            List<Point> result = new();
            foreach (var point in _points) {
                if (point == basePoint) continue;
                if (ArePointsInView(basePoint, point))
                    result.Add(point);
            }

            return result;
        }
        
        public bool ArePointsInView(Point a, Point b) {
            Vector2 direction = a.transform.position - b.transform.position;
            return !Physics2D.Raycast(b.transform.position, direction.normalized, direction.magnitude, layerMask);
        }
        
        public Stack<Point> CalculatePath(Point start, Point goal) {
            if(!_points.Contains(start) || !_points.Contains(goal)) return null;
            
            Stack<Point> steps = new();
            PriorityQueue<Point> frontier = new();
            frontier.Enqueue(start, 0);
            Dictionary<Point, Point> cameFrom = new();
            Dictionary<Point, float> costSoFar = new();
            costSoFar.Add(start, 0);

            while (frontier.Count != 0) {
                Point current = frontier.Dequeue();

                if (current == goal) break;

                foreach (var next in current.neighbours) {
                    float newCost = costSoFar[current] + CalculateCost(current, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                        costSoFar[next] = newCost;
                        frontier.Enqueue(next, newCost + CalculateHeuristic(goal, next));
                        cameFrom[next] = current;
                    }
                }
            }

            if (costSoFar.ContainsKey(goal)) {
                Point current = goal;
                steps.Push(goal);
                while (current != start) {
                    current = cameFrom[current];
                    steps.Push(current);
                }
            }

            return steps;
        }

        private float CalculateCost(Point a, Point b) {
            return Vector2.Distance(a.transform.position, b.transform.position);
        }

        public Point GetClosestNode(Vector2 position, Func<Point, bool> condition) {
            Point closest = null;
            float closestDistance = float.MaxValue;
            foreach (var poing in _points) {
                if (!condition(poing)) continue;
                float distance  = Vector2.Distance(position, poing.transform.position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closest = poing;
                }
            }

            return closest;
        }

        private float CalculateHeuristic(Point a, Point b) {
            return Vector2.Distance(a.transform.position, b.transform.position);
        }
    }
}