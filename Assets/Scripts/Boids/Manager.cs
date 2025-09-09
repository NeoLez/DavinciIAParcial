using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Boids {
    public class Manager : MonoBehaviour {
        public static Manager instance { get; private set; }
        
        [Header("Boids")]
        [SerializeField] private GameObject boidPrefab;
        [SerializeField] private float spawnRange;
        [SerializeField] private int number;
        [SerializeField] private bool generateBoids;
        
        [Header("Hunter")]
        [SerializeField] private GameObject hunterPrefab;
        [SerializeField] private List<Target> patrolPoints;
        [SerializeField] private bool generateHunter;

        [Header("Food")]
        [SerializeField] private GameObject foodPrefab;
        [SerializeField] private float foodSpawnRange;
        [SerializeField] private float foodPickupRange;
        [SerializeField] private float foodSpawnRate;
        
        private void Awake() {
            instance = this;
            _foodTimer = new Timer(foodSpawnRate);
        }

        public readonly List<Boid> Boids = new();
        public readonly List<Target> FoodItems = new();
        public readonly List<Hunter> Hunters = new();
        
        private Timer _foodTimer;
        
        public int amountKilled;
        public int amountBorn;
        
        private void Update() {
            Debug.Log($"Killed: {amountKilled} Born: {amountBorn}");
            if (generateBoids) {
                generateBoids = false;
                GenerateBoids();
            }
            if (generateHunter) {
                generateHunter = false;
                GenerateHunter();
            }

            if (_foodTimer.HasFinished()) {
                _foodTimer.AddTime(foodSpawnRate);
                SpawnFood();
            }
        }

        private void SpawnFood() {
            GameObject food = Instantiate(foodPrefab, Random.insideUnitCircle * foodSpawnRange, Quaternion.identity);
            Target target = food.GetComponent<Target>();
            target.radius = foodPickupRange;
            target.position = food.transform.position;
            FoodItems.Add(target);
        }

        private void GenerateBoids() {
            for (int i = 0; i < number; i++) {
                GameObject boid = Instantiate(boidPrefab, Random.insideUnitCircle * spawnRange, Quaternion.identity);
                Boid boidComponent = boid.GetComponent<Boid>();
                boidComponent.Initialize(Random.insideUnitCircle);
                Boids.Add(boidComponent);
            }
        }

        private readonly List<Boid> _boidsToDelete = new();
        public void DeleteBoid(Boid boid) {
            _boidsToDelete.Add(boid);
        }
        
        private readonly List<Target> _foodsToDelete = new();
        public void DeleteFood(Target food) {
            _foodsToDelete.Add(food);
        }

        private void LateUpdate() {
            foreach (var boidToDelete in _boidsToDelete) {
                Boids.Remove(boidToDelete);
                Destroy(boidToDelete.gameObject);
            }
            _boidsToDelete.Clear();
            
            foreach (var foodToDelete in _foodsToDelete) {
                FoodItems.Remove(foodToDelete);
                Destroy(foodToDelete.gameObject);
            }
            _foodsToDelete.Clear();
        }

        private void GenerateHunter() {
            GameObject hunter = Instantiate(hunterPrefab);
            hunter.GetComponent<Hunter>().SetPatrolPositions(new List<Target>(patrolPoints));
            Hunters.Add(hunter.GetComponent<Hunter>());
        }
    }
}