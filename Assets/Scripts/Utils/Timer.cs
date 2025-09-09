using UnityEngine;

namespace Utils {
    public class Timer {
        private float _waitUntil;

        public Timer(float duration) {
            Start(duration);
        }

        public Timer() { }

        public bool HasFinished() {
            return Time.time >= _waitUntil;
        }

        public void AddTime(float extraTime) {
            _waitUntil += extraTime;
        }

        public void Start(float duration) {
            _waitUntil = Time.time + duration;
        }
    }
}