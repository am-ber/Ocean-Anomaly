using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Components
{
    public class KillSelf : MonoBehaviour
    {
        public float TimeTillDeath = 0.5f;
        private float time = 0;
        public UnityEvent KillActive;
		private void OnEnable()
		{
			KillActive?.Invoke();
		}
		void Update()
        {
            time += Time.deltaTime;
            if (time >= TimeTillDeath)
                Destroy(gameObject);
        }
    }
}