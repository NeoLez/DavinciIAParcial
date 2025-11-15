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
            
            pathToNode.Pop();
            Node node = pathToNode.Peek();
            float distanceToTarget = (node.transform.position - enemy.transform.position).magnitude;
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

            if (Vector2.Distance(NodeManager.Instance.player.transform.position, enemy.transform.position) <= killDistance) {
                GameObject.Destroy(NodeManager.Instance.player.gameObject);
            }
        }
    }
}