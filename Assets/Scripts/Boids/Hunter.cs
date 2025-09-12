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

        private float _energy;
        

        private void Start() {
            _stateMachine = new();
            _stateMachine.AddState(HunterStates.Rest, new RestState(_stateMachine, this, _settings.timeToRecovery));
            _stateMachine.AddState(HunterStates.Patrol, new PatrolState(_stateMachine, this, _patrolPositions, _settings.minimumPatrolPointDistance, _settings.viewDistance));
            _stateMachine.AddState(HunterStates.Hunting, new HuntingState(_stateMachine, this, _settings.viewDistance, _settings.boidEatDistance));
            _stateMachine.ChangeState(HunterStates.Patrol);
            _energy = _settings.maxEnergy;
        }

        private void Update() {
            _stateMachine.UpdateState(Time.deltaTime);
            
            ProcessMovement();
        }
        
        public void SlowDown() {
            velocity *= 1 - _settings.stopSlowdownSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Restores current energy to the maximum amount.
        /// </summary>
        public void RestoreEnergy() {
            _energy = _settings.maxEnergy;
        }

        public void AddEnergy(float energyToAdd) {
            _energy += energyToAdd;
            if (_energy > _settings.maxEnergy)
                _energy = _settings.maxEnergy;
        }

        public bool HasEnergy() {
            return _energy > 0;
        }

        /// <summary>
        /// Sets the list of positions the <c>Hunter</c> will use to patrol.
        /// <para>The list is saved as a reference. If it's changed externally, the behaviour of the <c>Hunter</c> will change accordingly.
        /// Create a copy if that's not what you want.
        /// </para>
        /// </summary>
        /// <param name="positions">List of positions to be used for the <c>Hunter</c> to patrol.</param>
        public void SetPatrolPositions(List<Target> positions) {
            _patrolPositions = positions;
        }
    }
}