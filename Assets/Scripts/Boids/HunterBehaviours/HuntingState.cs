using System;
using StateMachine;
using UnityEngine;

namespace Boids.HunterBehaviours {
    public class HuntingState : IState<HunterStates> {
        private readonly StateMachine<HunterStates> _stateMachine;
        private readonly Hunter _hunter;
        private readonly float _viewRange;
        private readonly float _eatDistance;

        public HuntingState(StateMachine<HunterStates> stateMachine, Hunter hunter, float viewRange, float eatDistance) {
            _stateMachine = stateMachine;
            _hunter = hunter;
            _viewRange = viewRange;
            _eatDistance = eatDistance;
        }
        
        public void OnEnter() {
            Debug.Log("Hunting");
        }

        public void OnExit() {
        }

        public void OnUpdate(float deltaTime) {
            var boids = Manager.instance.Boids;
            
            float minimumDistance = Single.MaxValue;
            Boid targetedBoid = null;
            foreach (var boid in boids) {
                float distance = Vector2.Distance(_hunter.transform.position, boid.transform.position);
                if (distance < minimumDistance && distance <= _viewRange) {
                    minimumDistance = distance;
                    targetedBoid = boid;
                }
            }

            if (!_hunter.HasEnergy()) {
                _stateMachine.ChangeState(HunterStates.Rest);
                return;
            }
            if (targetedBoid == null) {
                _stateMachine.ChangeState(HunterStates.Patrol);
                return;
            }
            
            
            
            float distanceToTargetBoid = Vector2.Distance(_hunter.transform.position, targetedBoid.transform.position);
            if (distanceToTargetBoid < _eatDistance) {
                Manager.instance.DeleteBoid(targetedBoid);
                Manager.instance.amountKilled++;
            }
            _hunter.AddVelocity(_hunter.Pursue(targetedBoid.transform.position, targetedBoid.velocity, distanceToTargetBoid/_hunter.GetMaxSpeed()));
            _hunter.SpendEnergy(deltaTime);
        }
    }
}