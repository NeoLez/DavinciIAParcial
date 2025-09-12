using System;
using System.Collections.Generic;
using Boids.HunterBehaviours;
using Boids.SO;
using StateMachine;
using UnityEngine;

namespace Boids {
    public class Hunter : MovementAgent {
        private StateMachine<HunterStates> _stateMachine;
        private List<Target> _patrolPositions;
        [NonSerialized] private HunterSO _settings;
        
        public void Initialize(HunterSO settings) {
            base.Initialize(settings);
            _settings = settings;
        }
        
        public float energy { get; private set; }
        

        private void Start() {
            _stateMachine = new();
            _stateMachine.AddState(HunterStates.Rest, new RestState(_stateMachine, this, _settings.timeToRecovery));
            _stateMachine.AddState(HunterStates.Patrol, new PatrolState(_stateMachine, this, _patrolPositions, _settings.minimumPatrolPointDistance, _settings.viewDistance));
            _stateMachine.AddState(HunterStates.Hunting, new HuntingState(_stateMachine, this, _settings.viewDistance, _settings.boidEatDistance));
            _stateMachine.ChangeState(HunterStates.Patrol);
            energy = _settings.maxEnergy;
        }

        private void Update() {
            _stateMachine.UpdateState(Time.deltaTime);
            
            ProcessMovement();
        }

        public void StopMoving() {
            velocity *= 1 - (_settings.stopSlowdownSpeed * Time.deltaTime);
        }

        public void RestoreEnergy() {
            energy = _settings.maxEnergy;
        }

        public void AddEnergy(float energyToAdd) {
            energy += energyToAdd;
            if (energy > _settings.maxEnergy)
                energy = _settings.maxEnergy;
        }

        public bool HasEnergy() {
            return energy > 0;
        }

        public void SetPatrolPositions(List<Target> positions) {
            _patrolPositions = positions;
        }
    }
}