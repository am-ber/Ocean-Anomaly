using OceanAnomaly.Attributes;
using OceanAnomaly.Managers;
using UnityEngine;

namespace OceanAnomaly.Components
{
	public class BasicMoveBehavior : MonoBehaviour
	{
		public float maxSpeed = 10;
		public float accuracy = 0;
		public float speedUp = 0;
		public float rotationSpeed = 1;
		public float offset = 0;
		public float cullDistance = 1;
		[ReadOnly]
		[SerializeField]
		protected float log = 0;
		[ReadOnly]
		[SerializeField]
		protected Vector3 targetPosition;
		protected float angle;
		public Transform projectileTarget;
		public float travelSpeed;
		public void setTarget(Vector3 target, float accuracy)
		{
			targetPosition = target;
			this.accuracy = accuracy;
		}
		public void setTarget(Transform target, float accuracy)
		{
			projectileTarget = target;
			this.accuracy = accuracy;
		}
		protected void FixedBasicMove()
		{
			transform.position += transform.up * Time.deltaTime * (maxSpeed * PlayerUpgradeManager.projectileSpeed) * (speedUp > 0 ? log : 1);
			transform.position = new Vector3(transform.position.x, transform.position.y, 0);
			log += speedUp;
		}
		protected void rotateTo()
		{
			Vector3 targetPos;
			if (projectileTarget != null)
				targetPos = projectileTarget.position;
			else
				targetPos = targetPosition;

			targetPos.x = targetPos.x - transform.position.x;
			targetPos.y = targetPos.y - transform.position.y;
			angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.RotateTowards(transform.rotation,
				Quaternion.Euler(new Vector3(0, 0, angle + offset + accuracy)), rotationSpeed);
		}
	}
}
