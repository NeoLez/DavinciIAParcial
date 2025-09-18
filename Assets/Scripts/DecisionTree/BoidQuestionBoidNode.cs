using Boids;
using UnityEngine;
using UnityEngine.Serialization;

namespace DecisionTree {
    public class BoidQuestionBoidNode : BoidNode
    {
        public BoidNode trueBoidNode;
        public BoidNode falseBoidNode;

        public TypeQuestion type;

        public override void Execute(Boid boid)
        {
            switch (type)
            {
                case TypeQuestion.IsFoodNearby:
                    foreach (var food in Manager.instance.FoodItems) {
                        if (Vector2.Distance(food.transform.position, boid.transform.position) <=
                            boid.Settings.foodDetectionRange) {
                            trueBoidNode.Execute(boid);
                            return;
                        }
                    }
                    falseBoidNode.Execute(boid);
                
                    return;
                case TypeQuestion.IsHunterNearby:
                    if (boid.IsHunterNearby(out _)) {
                        trueBoidNode.Execute(boid);
                        return;
                    }
                    falseBoidNode.Execute(boid);
                    return;
                case TypeQuestion.AreThereBoidsNearby:
                    if (boid.AreBoidsNearby()) {
                        trueBoidNode.Execute(boid);
                        return;
                    }
                    falseBoidNode.Execute(boid);
                    return;
            }
        }
    }

    public enum TypeQuestion
    {
        IsFoodNearby,
        IsHunterNearby,
        AreThereBoidsNearby,
    }
}