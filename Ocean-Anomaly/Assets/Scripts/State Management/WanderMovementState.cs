using OceanAnomaly.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OceanAnomaly.StateManagement
{
	[Serializable]
	public class WanderMovementState : MovementState
	{
		public WanderMovementState(Transform transform, MovementDataScriptable movementData) : base(transform, movementData)
		{
		}
		public override void Update()
		{
			base.Update();
			// Determine a new direction to head to
			if (target == null)
			{
				desiredDirection = (desiredDirection + UnityEngine.Random.insideUnitCircle.ToVector3() * movementData.WanderStrength).normalized;
			} else
			{
				desiredDirection = (target.position - GetHeadOffset()).normalized;
			}
			// Solve our desired velocity and acceleration
			desiredVelocity = desiredDirection * movementData.MaxSpeed;
			desiredSteeringForce = (desiredVelocity - velocity) * movementData.SteerStrength;
			acceleration = Vector3.ClampMagnitude(desiredSteeringForce, movementData.SteerStrength) / 1;
			// Solve velocity and position
			velocity = Vector3.ClampMagnitude(velocity + (acceleration * Time.deltaTime), movementData.MaxSpeed);
			position += velocity * Time.deltaTime;
			// Figure steering angle
			angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
			transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle + angleOffset));
		}
	}
}