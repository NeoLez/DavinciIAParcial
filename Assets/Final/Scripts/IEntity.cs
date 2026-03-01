using UnityEngine;

namespace Final.Scripts
{
    public interface IEntity
    {
        public void TakeDamage(int dmg);
        public int GetHealth();
        public GameObject GetGameObject();
        public float GetSize();
    }
}