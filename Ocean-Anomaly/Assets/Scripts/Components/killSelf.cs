using UnityEngine;

namespace OceanAnomaly.Components
{
    public class KillSelf : MonoBehaviour
    {
        public float TimeTillDeath = 0.5f;
        private float time = 0;

        void Update()
        {
            time += Time.deltaTime;
            if (time >= TimeTillDeath)
                Destroy(gameObject);
        }
    }
}