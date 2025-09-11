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
            if (!_hunter.HasEnergy()) {
                _stateMachine.ChangeState(HunterStates.Rest);
                return;
            }
            
            float minimumDistance = _viewRange;
            Boid targetedBoid = FindClosestBoid(ref minimumDistance);
            if (targetedBoid == null) {
                _stateMachine.ChangeState(HunterStates.Patrol);
                return;
            }
            
            _hunter.AddVelocity(_hunter.Pursue(targetedBoid.transform.position, targetedBoid.GetVelocity(), minimumDistance/_hunter.GetMaxSpeed()));
            if (minimumDistance < _eatDistance) {
                Manager.instance.DeleteBoid(targetedBoid);
                Manager.instance.amountKilled++;
            }
            _hunter.AddEnergy(-deltaTime);
        }

        private Boid FindClosestBoid(ref float minimumDistance) {
            Boid targetedBoid = null;
            foreach (var boid in Manager.instance.Boids) {
                float distance = Vector2.Distance(_hunter.transform.position, boid.transform.position);
                if (distance < minimumDistance) {
                    minimumDistance = distance;
                    targetedBoid = boid;
                }
            }

            return targetedBoid;
        }
    }
}