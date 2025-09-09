using System.Collections.Generic;
using Hunter;
using StateMachine;
using UnityEngine;

namespace Boids {
    public class Hunter : MovementAgent {
        private StateMachine<HunterStates> _stateMachine;
        [SerializeField] private List<Target> patrolPositions;
        [SerializeField] private float timeToRecovery;
        [SerializeField] private float minimumPatrolPointDistance;
        [SerializeField] private float viewDistance;
        [SerializeField] private float stopSlowdownSpeed;
        [SerializeField] private float boidEatDistance;
        public float energy { get; private set; }
        [SerializeField] private float maxEnergy;

        private void Start() {
            _stateMachine = new();
            _stateMachine.AddState(HunterStates.Rest, new RestState(_stateMachine, this, timeToRecovery));
            _stateMachine.AddState(HunterStates.Patrol, new PatrolState(_stateMachine, this, patrolPositions, minimumPatrolPointDistance, viewDistance));
            _stateMachine.AddState(HunterStates.Hunting, new HuntingState(_stateMachine, this, viewDistance, boidEatDistance));
            _stateMachine.ChangeState(HunterStates.Patrol);
            energy = maxEnergy;
        }

        private void Update() {
            _stateMachine.UpdateState(Time.deltaTime);
            
            ProcessMovement();
        }

        public new void Arrive(Vector2 target, float radius) {
            velocity += base.Arrive(target, radius);
        }

        public new void Pursue(Vector2 target, Vector2 targetSpeed, float predictionTime) {
            velocity += base.Pursue(target, targetSpeed, predictionTime);
        }

        public void StopMoving() {
            velocity *= 1 - (stopSlowdownSpeed * Time.deltaTime);
        }

        public void RestoreEnergy() {
            energy = maxEnergy;
        }

        public void SpendEnergy(float energyToSpend) {
            energy -= energyToSpend;
        }

        public bool HasEnergy() {
            return energy > 0;
        }

        public void SetPatrolPositions(List<Target> positions) {
            patrolPositions = positions;
        }
    }
}