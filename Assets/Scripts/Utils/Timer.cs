using UnityEngine;

namespace Utils {
    public class Timer {
        private float _waitUntil;

        public Timer(float duration) {
            Start(duration);
        }

        public Timer() { }
        
        /// <returns>True if the timer has finished counting. Otherwise, false.</returns>
        public bool HasFinished() {
            return Time.time >= _waitUntil;
        }

        /// <summary>
        /// Extends timer duration.
        /// </summary>
        /// <param name="extraTime">Amount of time to add to the timer in seconds.</param>
        public void AddTime(float extraTime) {
            _waitUntil += extraTime;
        }

        /// <summary>
        /// Starts or resets <c>Timer</c>.
        /// </summary>
        /// <param name="duration">Amount of time to wait for in seconds</param>
        public void Start(float duration) {
            _waitUntil = Time.time + duration;
        }
    }
}