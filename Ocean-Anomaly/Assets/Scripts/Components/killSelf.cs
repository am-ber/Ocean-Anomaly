using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Components
{
	public class KillSelf : MonoBehaviour
	{
		public float TimeTillDeath = 0.5f;
		public bool destroyAttachedGameObject = true;
		private float time = 0;
		public UnityEvent KillActive;
		public UnityEvent KillInactive;
		public UnityEvent DeathReached;
		private void OnEnable()
		{
			KillActive?.Invoke();
		}
		private void Update()
		{
			time += Time.deltaTime;
			if (time >= TimeTillDeath)
			{
				DeathReached?.Invoke();
				if (destroyAttachedGameObject)
				{
					Destroy(gameObject);
				}
			}
		}
		private void OnDisable()
		{
			KillInactive?.Invoke();
		}
	}
}