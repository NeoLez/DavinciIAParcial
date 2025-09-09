using UnityEngine;

namespace DefaultNamespace.Utils {
    public class Timer {
        private float waitUntil;

        public Timer(float duration) {
            waitUntil = Time.time + duration;
        }

        public bool hasFinished() {
            return Time.time >= waitUntil;
        }

        public void AddTime(float extraTime) {
            waitUntil += extraTime;
        }
    }
}