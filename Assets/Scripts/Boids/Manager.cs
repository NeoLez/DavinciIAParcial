using System.Collections.Generic;
using Boids.SO;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Boids {
    public class Manager : MonoBehaviour {
        public static Manager instance { get; private set; }
        private static Camera _cam;
        
        [Header("Boids")]
        [SerializeField] private GameObject boidPrefab;
        [SerializeField] private BoidSO boidSettings;
        [SerializeField] private float boidSpawnRange;
        [SerializeField] private int numberOfBoids;
        [SerializeField] private bool generateBoids;
        
        [Header("Hunter")]
        [SerializeField] private GameObject hunterPrefab;
        [SerializeField] private HunterSO hunterSettings;
        [SerializeField] private List<Target> hunterPatrolPoints;
        [SerializeField] private bool generateHunter;

        [Header("Food")]
        [SerializeField] private GameObject foodPrefab;
        [SerializeField] private float foodSpawnRange;
        [SerializeField] private float foodPickupRange;
        [SerializeField] private float foodSpawnRate;
        
        private void Awake() {
            instance = this;
            _foodTimer = new Timer(foodSpawnRate);
            _cam = Camera.main;
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
                GenerateBoidsArea();
            }
            if (generateHunter) {
                generateHunter = false;
                GenerateHunter(Vector2.zero);
            }

            if (_foodTimer.HasFinished()) {
                _foodTimer.AddTime(foodSpawnRate);
                SpawnFoodArea();
            }

            if (Input.GetMouseButtonDown(0)) {
                GenerateBoid(_cam.ScreenToWorldPoint(Input.mousePosition));
            }
            if (Input.GetMouseButtonDown(1)) {
                GenerateHunter(_cam.ScreenToWorldPoint(Input.mousePosition));
            }
            if (Input.GetMouseButtonDown(2)) {
                GenerateFood(_cam.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        private void GenerateFood(Vector2 position) {
            GameObject food = Instantiate(foodPrefab, position, Quaternion.identity);
            Target target = food.GetComponent<Target>();
            target.radius = foodPickupRange;
            FoodItems.Add(target);
        }
        private void SpawnFoodArea() {
            GenerateFood(Random.insideUnitCircle * foodSpawnRange);
        }

        private void GenerateBoid(Vector2 position) {
            GameObject boid = Instantiate(boidPrefab, position, Quaternion.identity);
            Boid boidComponent = boid.GetComponent<Boid>();
            boidComponent.Initialize(boidSettings);
            boidComponent.SetInitialVelocity(Random.insideUnitCircle);
            Boids.Add(boidComponent);
        }
        
        private void GenerateBoidsArea() {
            for (int i = 0; i < numberOfBoids; i++) {
                GenerateBoid(Random.insideUnitCircle * boidSpawnRange);
            }
        }
        
        private void GenerateHunter(Vector2 position) {
            GameObject hunter = Instantiate(hunterPrefab, position, Quaternion.identity);
            Hunter hunterComponent = hunter.GetComponent<Hunter>();
            hunterComponent.Initialize(hunterSettings);
            hunterComponent.SetPatrolPositions(new List<Target>(hunterPatrolPoints));
            Hunters.Add(hunter.GetComponent<Hunter>());
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
    }
}