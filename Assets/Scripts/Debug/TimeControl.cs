using UnityEngine;

namespace Debug
{
    public class TimeControl : MonoBehaviour
    {
        [Range(0,1)]
        public float timeScale = 1.0f;

        private void Update()
        {
            Time.timeScale = timeScale;
        }
        
    }
}