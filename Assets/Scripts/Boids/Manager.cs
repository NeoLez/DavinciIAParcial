using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids {
    public class Manager : MonoBehaviour {
        public static Manager Instance { get; private set; }
        private void Awake() {
            Instance = this;
        }

        public readonly List<Boid> boids = new();

        [SerializeField] private GameObject boidPrefab;
        [SerializeField] private float spawnRange;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float maxTurnSpeed;
        [SerializeField] private int number;

        [SerializeField] private bool generateBoids;
        private void Update() {
            if (generateBoids) {
                generateBoids = false;
                GenerateBoids();
            }
        }

        public void GenerateBoids() {
            for (int i = 0; i < number; i++) {
                GameObject boid = Instantiate(boidPrefab, Random.insideUnitCircle * spawnRange, Quaternion.identity);
                Boid boidComponent = boid.GetComponent<Boid>();
                boidComponent.maxSpeed = maxSpeed;
                boidComponent.maxTurnSpeed = maxTurnSpeed;
                boidComponent.velocity = Random.insideUnitSphere;
                boids.Add(boidComponent);
            }
            
        }
    }
}