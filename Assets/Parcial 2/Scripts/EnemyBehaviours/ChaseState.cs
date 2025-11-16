using System.Collections.Generic;
using StateMachine;
using UnityEngine;

namespace Parcial_2.Scripts.EnemyBehaviours {
    public class ChaseState : IState<EnemyBehaviour> {
        
        private Enemy enemy;
        private StateMachine<EnemyBehaviour> stateMachine;
        private Stack<Node> pathToNode;
        private float killDistance;

        public ChaseState(StateMachine<EnemyBehaviour> stateMachine, Enemy enemy, float killDistance) {
            this.enemy = enemy;
            this.stateMachine = stateMachine;
            this.killDistance = killDistance;
        }
        
        public void OnEnter() {
            
        }

        public void OnExit() {
            
        }

        public void OnUpdate(float deltaTime) {
            pathToNode = NodeManager.Instance.CalculatePath(enemy._node, NodeManager.Instance.enemyTarget);
            if (pathToNode == null) {
                stateMachine.ChangeState(EnemyBehaviour.Patrol); 
                return;
            }

            if (pathToNode.Count == 0) {
                pathToNode = NodeManager.Instance.CalculatePath(enemy._node,
                    NodeManager.Instance.GetClosestNode(NodeManager.Instance.player.transform.position, node1 => node1.inConnections && node1.outConnections)
                );
            }
            pathToNode.Pop();
            Node node = pathToNode.Peek();
            Vector2 distanceVector = node.transform.position - enemy.transform.position;
            float distanceToTarget = distanceVector.magnitude;
            float maxDistanceThisFrame = enemy.speed * deltaTime;

            if (distanceToTarget < maxDistanceThisFrame) {
                maxDistanceThisFrame = distanceToTarget;
                if (node == NodeManager.Instance.enemyTarget) {
                    NodeManager.Instance.RemoveNode(node);
                    pathToNode.Clear();
                    stateMachine.ChangeState(EnemyBehaviour.Patrol);
                }
            }
            enemy.transform.position += (node.transform.position - enemy.transform.position).normalized * maxDistanceThisFrame;
            enemy.viewDetectionAngleOffset = Vector2.Angle(Vector2.right, distanceVector) * Mathf.Deg2Rad;

            if (Vector2.Distance(NodeManager.Instance.player.transform.position, enemy.transform.position) <= killDistance) {
                Debug.Log("You got eaten :c");
            }
        }
    }
}