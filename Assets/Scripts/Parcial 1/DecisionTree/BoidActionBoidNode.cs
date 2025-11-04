using Boids;

namespace DecisionTree {
    public class BoidActionBoidNode : BoidNode
    {
        public TypeAction type;

        public override void Execute(Boid boid)
        {
            switch (type) {
                case TypeAction.ChaseFood:
                    float foodDistance = boid.Settings.foodDetectionRange;
                    boid.IsFoodNearby(out Target food, ref foodDistance);
                    if (foodDistance <= food.radius) {
                        boid.ConsumeFood(food);
                    }
                    else {
                        boid.AddVelocity(boid.Arrive(food.transform.position, food.radius));
                    }
                    break;
                case TypeAction.FleeHunter:
                    boid.IsHunterNearby(out var hunter);
                    boid.AddVelocity(boid.Flee(hunter.transform.position));
                    break;
                case TypeAction.Flocking:
                    boid.AddVelocity(boid.FlockingHighPerformance());
                    break;
                case TypeAction.Random:
                    boid.RandomMovement();
                    break;
            }
        }
    }

    public enum TypeAction
    {
        ChaseFood,
        FleeHunter,
        Flocking,
        Random,
    }
}