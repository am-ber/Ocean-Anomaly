using OceanAnomaly.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	public class MovementState : State
	{
		public MovementDataScriptable movementData;
		[SerializeField]
		protected Transform transform;
		[SerializeField]
		protected Transform target;
		[SerializeField]
		protected float targetReachDistance = 2f;
		[SerializeField]
		protected Transform headPosition;
		[SerializeField]
		protected float angleOffset = -90f;
		[SerializeField]
		protected bool notifiedOnTargetReachSubscribers = false;
		public UnityEvent<Transform> OnTargetReach;
		[Header("Debugging Variables")]
		[ReadOnly]
		[SerializeField]
		protected float angle = 0f;
		[ReadOnly]
		[SerializeField]
		protected Vector3 position = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		protected Vector3 velocity = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		protected Vector3 acceleration = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		protected Vector3 desiredVelocity = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		protected Vector3 desiredDirection = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		protected Vector3 desiredSteeringForce = Vector3.zero;

		public MovementState(Transform transform, MovementDataScriptable movementData)
		{
			this.transform = transform;
			this.movementData = movementData;

			OnTargetReach = new UnityEvent<Transform>();
		}
		public override void Update()
		{
			CheckTargetReached();
		}
		/// <summary>
		/// Checks if the targets position vector is within the <seealso cref="targetReachDistance"/> variable with current position from <seealso cref="GetHeadOffset()"/>
		/// </summary>
		public void CheckTargetReached()
		{
			if (target == null)
			{
				return;
			}
			// If we get close enough to the target, and we want to remove the target, then lets remove the target from our movement path
			if (Vector3.Distance(target.position, GetHeadOffset()) < targetReachDistance)
			{
				if (!notifiedOnTargetReachSubscribers)
				{
					Debug.Log($"{transform.name} reached {target.name}");
					// Removal of the target first, before notifying potential subscribers
					if (movementData.RemoveTargetAfterReach)
					{
						target = null;
					}
					OnTargetReach?.Invoke(target);
					// This boolean prevents spam protection
					notifiedOnTargetReachSubscribers = true;
				}
			} else
			{
				// When we get out of range lets toggle this off
				if (notifiedOnTargetReachSubscribers)
				{
					notifiedOnTargetReachSubscribers = false;
				}
			}
		}
		/// <summary>
		/// Returns the current transform.position 
		/// </summary>
		/// <returns></returns>
		public Vector3 GetHeadOffset()
		{
			// If we don't have a head then lets solve for some arbitrary position forward I guess...
			if (headPosition == null)
			{
				return transform.position + (transform.up * 2);
			}
			return headPosition.position;
		}
		/// <summary>
		/// Returns the current angle + angleOffset.
		/// </summary>
		/// <returns></returns>
		public float GetFacingAngle()
		{
			return angle + angleOffset;
		}
		/// <summary>
		/// Sets the current movement target to a desired Transform.
		/// </summary>
		/// <param name="target"></param>
		public void SetTarget(Transform target)
		{
			Debug.Log("Setting Target to " + target.name);
			this.target = target;
		}
	}
}