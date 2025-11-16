using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Parcial_2.Scripts.EnemyBehaviours {
    public class PatrolState : IState<EnemyBehaviour> {
        List<Node> path;
        int currentIndex = 0;
        private Enemy enemy;
        private StateMachine<EnemyBehaviour> stateMachine;
        private Stack<Node> pathToNode;
        
        public PatrolState(StateMachine<EnemyBehaviour> stateMachine, List<Node> path, Enemy enemy) {
            this.path = path;
            this.enemy = enemy;
            this.stateMachine = stateMachine;
        }
        
        public void OnEnter() {
        }

        public void OnExit() {
        }

        public void OnUpdate(float deltaTime) {
            if (NodeManager.Instance.ContainsNode(NodeManager.Instance.enemyTarget)) {
                pathToNode?.Clear();
                stateMachine.ChangeState(EnemyBehaviour.Chase);
                return;
            }

            if (pathToNode == null || pathToNode.Count == 0) {
                var node = path[currentIndex];
            
                if (enemy._node.neighbours.Contains(node)) {
                    Vector2 distanceVector = node.transform.position - enemy.transform.position;
                    float distanceToTarget = distanceVector.magnitude;
                    float maxDistanceThisFrame = enemy.speed * deltaTime;

                    if (distanceToTarget < maxDistanceThisFrame) {
                        maxDistanceThisFrame = distanceToTarget;
                        currentIndex = (currentIndex + 1) % path.Count;
                    }
                
                    enemy.transform.position += distanceVector.normalized.ToVector3() * maxDistanceThisFrame;
                    enemy.viewDetectionAngleOffset = Mathf.Atan2(distanceVector.y,distanceVector.x);
                }
                else {
                    pathToNode = NodeManager.Instance.CalculatePath(enemy._node, node);
                }
            }

            if (pathToNode != null && pathToNode.Count != 0) {
                Node node = pathToNode.Peek();
                Vector2 distanceVector = node.transform.position - enemy.transform.position;
                float distanceToTarget = distanceVector.magnitude;
                distanceVector.Normalize();
                float maxDistanceThisFrame = enemy.speed * deltaTime;

                if (distanceToTarget < maxDistanceThisFrame) {
                    maxDistanceThisFrame = distanceToTarget;
                    pathToNode.Pop();
                }
                
                enemy.transform.position += (node.transform.position - enemy.transform.position).normalized * maxDistanceThisFrame;
                enemy.viewDetectionAngleOffset = Mathf.Atan2(distanceVector.y,distanceVector.x);
            }
        }
    }
}