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
            
            if (minimumDistance < _eatDistance) {
                Manager.instance.Boids.Remove(targetedBoid);
                Manager.instance.amountKilled++;
                return;
            }
            
            _hunter.AddVelocity(_hunter.Pursue(targetedBoid.transform.position, targetedBoid.GetVelocity(), minimumDistance/_hunter.GetMaxSpeed()));
            _hunter.AddEnergy(-deltaTime);
        }
        
        /// <param name="minimumDistance"><c>Boid</c>s that are further from the hunter than this are ignored. The value is altered to match the distance from the hunter to the closest <c>boid</c> if found.</param>
        /// <returns>Returns the closest <c>boid</c> to the hunter which is also closer than minimumDistance. If not found, returns null</returns>
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