using System;
using System.Collections.Generic;
using Final.Scripts.EntityBehaviours;
using UnityEngine;

namespace Final.Scripts
{
    public class Team : MonoBehaviour
    {
        public Leader leader;
        //public List<NPC> npcs = new();
        public Point teamBase;
        public Action<LeaderBehaviours> OnLaderChangedState;
        public Action OnLeaderDefeated;
    }
}