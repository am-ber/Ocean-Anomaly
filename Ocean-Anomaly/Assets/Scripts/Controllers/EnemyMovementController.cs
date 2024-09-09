using Dreamteck.Splines;
using OceanAnomaly.Attributes;
using OceanAnomaly.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OceanAnomaly.Controllers
{
	[Serializable]
	public struct MovementData
	{
		public float MaxSpeed;
		public float MinSpeed;
		public float SteerStrength;
		public float WanderStrength;
		public bool RemoveTargetAfterReach;
	}
	[RequireComponent(typeof(SplineFollower))]
	public class EnemyMovementController : MonoBehaviour
	{
		[Header("Current Movement Data")]
		[SerializeField]
		private float currentMaxSpeed = 2f;
		[SerializeField]
		private float currentMinSpeed = 1f;
		[SerializeField]
		private float wanderStrength = 0.2f;
		[SerializeField]
		private float steerStrength = 2f;
		[SerializeField]
		private bool removeTargetAfterReach = true;
		[Header("Movement Settings")]
		[SerializeField]
		private float angleOffset = -90f;
		[SerializeField]
		private float targetReachDistance = 1f;
		[SerializeField]
		private MovementBehavior currentMovementBehavior;
		[SerializeField]
		private EnemyFieldManager fieldManager;
		[SerializeField]
		private SplineFollower splineFollower;
		[SerializeField]
		private Transform headPosition;
		[SerializeField]
		private bool notifiedOnTargetReachSubscribers = false;
		public UnityEvent<Transform> OnTargetReach;
		[Header("Debugging Variables")]
		[ReadOnly]
		[SerializeField]
		private float angle = 0f;
		[ReadOnly]
		[SerializeField]
		private Vector3 position = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		private Vector3 velocity = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		private Vector3 acceleration = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		private Vector3 desiredVelocity = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		private Vector3 desiredDirection = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		private Vector3 desiredSteeringForce = Vector3.zero;
		[SerializeField]
		private Transform target;
		private Action currentMovementAction;
		private void Start()
		{
			if (splineFollower == null)
			{
				splineFollower = GetComponent<SplineFollower>();
				splineFollower.wrapMode = SplineFollower.Wrap.Loop;
				splineFollower.motion.rotationOffset = new Vector3(0, 0, 180);
			}
			if (fieldManager == null)
			{
				if (GlobalManager.Instance != null)
				{
					fieldManager = GlobalManager.Instance.enemyFieldManager;
				}
			}
		}
		private void Update()
		{
			if (currentMovementAction != null)
			{
				currentMovementAction();
			}
			CheckTargetReached();
		}
		private void CheckTargetReached()
		{
			if (target == null)
			{
				return;
			}
			// If we get close enough to the target, and we want to remove the target, then lets remove the target from our movement path
			if (Vector3.Distance(target.position, DetermineHeadOffset()) < targetReachDistance)
			{
				if (!notifiedOnTargetReachSubscribers)
				{
					// Call people listening if we reached our target
					OnTargetReach?.Invoke(target);
					// And removal of the target if we need to
					if (removeTargetAfterReach)
					{
						target = null;
					}
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
		private Vector3 DetermineHeadOffset()
		{
			// Handle the case were we might not have a head set. we can just guess with the up vector relative to this transform
			Vector3 headOffset = transform.position + (transform.up * 2);
			if (headPosition != null)
			{
				headOffset = headPosition.position;
			}
			return headOffset;
		}
		public void SetMovementBehavior(MovementBehavior behavior)
		{
			currentMovementBehavior = behavior;
			switch (behavior)
			{
				case MovementBehavior.WanderFollow:
					currentMovementAction = WanderMovement;
					break;
				case MovementBehavior.OnTrack:
					currentMovementAction = null;
					OnTrackMovement();
					break;
				default:
					currentMovementAction = null;
					break;
			}
		}
		private void OnTrackMovement()
		{
			if (fieldManager != null)
			{
				fieldManager.SubscribeToSpline(splineFollower);
			}
			splineFollower.followSpeed = currentMaxSpeed;
		}
		private void OnTrackMovementLeave()
		{
			if (fieldManager != null)
			{
				fieldManager.UnsubscribeToSpline(splineFollower);
			}
		}
		private void WanderMovement()
		{
			// Determine a new direction to head to
			if (target == null)
			{
				desiredDirection = (desiredDirection + UnityEngine.Random.insideUnitCircle.ToVector3() * wanderStrength).normalized;
			} else
			{
				desiredDirection = (target.position - DetermineHeadOffset()).normalized;
			}
			// Solve our desired velocity and acceleration
			desiredVelocity = desiredDirection * currentMaxSpeed;
			desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
			acceleration = Vector3.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;
			// Solve velocity and position
			velocity = Vector3.ClampMagnitude(velocity + (acceleration * Time.deltaTime), currentMaxSpeed);
			position += velocity * Time.deltaTime;
			// Figure steering angle
			angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
			transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle + angleOffset));
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
		/// Used to quickly update the different movement variables with a nice MovementData holder.
		/// </summary>
		/// <param name="data"></param>
		public void UpdateMovement(MovementData data)
		{
			currentMaxSpeed = data.MaxSpeed;
			currentMinSpeed = data.MinSpeed;
			steerStrength = data.SteerStrength;
			wanderStrength = data.WanderStrength;
			removeTargetAfterReach = data.RemoveTargetAfterReach;
		}
		/// <summary>
		/// Sets the current movement target to a desired Transform.
		/// </summary>
		/// <param name="target"></param>
		public void SetTargetPosition(Transform target)
		{
			this.target = target;
		}
	}
}