using System;
using System.Collections.Generic;
using DefaultNamespace.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids {
    public class Manager : MonoBehaviour {
        public static Manager Instance { get; private set; }
        private void Awake() {
            Instance = this;
            foodTimer = new Timer(foodSpawnRate);
        }

        public readonly List<Boid> boids = new();
        public readonly List<Target> foodItems = new();
        public readonly List<Hunter> hunters = new();

        [SerializeField] private GameObject boidPrefab;
        [SerializeField] private float spawnRange;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float maxTurnSpeed;
        [SerializeField] private int number;
        
        [SerializeField] private GameObject hunterPrefab;

        [SerializeField] private GameObject foodPrefab;
        [SerializeField] private float foodSpawnRange;
        [SerializeField] private float foodPickupRange;
        [SerializeField] private float foodSpawnRate;
        private Timer foodTimer;
        
        [SerializeField] private bool generateBoids;
        [SerializeField] private bool generateHunter;
        [SerializeField] private List<Target> patrolPoints;
        
        private void Update() {
            if (generateBoids) {
                generateBoids = false;
                GenerateBoids();
            }
            if (generateHunter) {
                generateHunter = false;
                GenerateHunter();
            }

            if (foodTimer.hasFinished()) {
                foodTimer.AddTime(foodSpawnRate);
                SpawnFood();
            }
        }

        private void SpawnFood() {
            GameObject food = Instantiate(foodPrefab, Random.insideUnitCircle * foodSpawnRange, Quaternion.identity);
            Target target = food.GetComponent<Target>();
            target.radius = foodPickupRange;
            target.position = food.transform.position;
            foodItems.Add(target);
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

        private List<Boid> boidsToDelete = new();
        public void DeleteBoid(Boid boid) {
            boidsToDelete.Add(boid);
        }

        private void LateUpdate() {
            foreach (var boidToDelete in boidsToDelete) {
                boids.Remove(boidToDelete);
                Destroy(boidToDelete.gameObject);
            }
            boidsToDelete.Clear();
        }

        public void GenerateHunter() {
            GameObject hunter = Instantiate(hunterPrefab);
            hunter.GetComponent<Hunter>().SetPatrolPositions(new List<Target>(patrolPoints));
            hunters.Add(hunter.GetComponent<Hunter>());
        }
    }
}