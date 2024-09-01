using OceanAnomaly.Attributes;
using OceanAnomaly.Managers;
using OceanAnomaly.Tools;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OceanAnomaly.Controllers
{
	public class PlayerMovementController : MonoBehaviour
	{
		[SerializeField]
		private CharacterController characterController;
		[SerializeField]
		private Transform playerGraphics;
		// Movement code
		[Header("Movement Settings")]
		[SerializeField]
		private float bodyRotateSpeed = 1.0f;
		[SerializeField]
		private float maxMoveSpeed = 8f;
		[SerializeField]
		[Range(0f, 1f)]
		private float decelerationRate = 0.9f;
		[SerializeField]
		[Range(0f, 1f)]
		private float accelerationRate = 0.8f;
		[Header("Movement Debugging")]
		[SerializeField]
		private uint roundingDecimalLimit = 3;
		[ReadOnly]
		[SerializeField]
		private float currentSpeed = 0f;
		[ReadOnly]
		[SerializeField]
		private float directionAngle = 0f;
		[ReadOnly]
		[SerializeField]
		private Vector2 moveDirection = Vector2.zero;
		[ReadOnly]
		[SerializeField]
		private Vector3 acceleration = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		private Vector3 velocity = Vector3.zero;
		[ReadOnly]
		[SerializeField]
		private Vector3 totalVelocity = Vector3.zero;
		// Dashing code
		[Header("Dash Settings")]
		[SerializeField]
		private float dashAmount = 30f;
		[SerializeField]
		private float dashCoolDown = 3;
		[SerializeField]
		private float dashReductionAmount = 0.2f;
		[Header("Dash Debugging")]
		[ReadOnly]
		[SerializeField]
		private bool canDash = true;
		[ReadOnly]
		public bool dashed = false;
		[ReadOnly]
		[SerializeField]
		private float dashTime = 0;
		[ReadOnly]
		[SerializeField]
		private Vector3 dashFactor = Vector3.zero;
		// Input System
		private PlayerInputActions inputActions;
		private InputAction inputMovement;
		private InputAction inputDash;
		private void Awake()
		{
			if (characterController == null)
			{
				characterController = GetComponent<CharacterController>();
			}
			InitializeInputActions();
		}
		private void InitializeInputActions()
		{
			inputActions = new PlayerInputActions();
			inputMovement = inputActions.Player.Move;
			inputDash = inputActions.Player.Dash;
			inputDash.performed += (context) =>
			{
				if (!canDash)
				{
					return;
				}
				dashFactor += (new Vector3(moveDirection.x, moveDirection.y) * dashAmount);
				dashed = true;
				canDash = false;
			};
		}
		
		private void OnEnable()
		{
			// Enable Movement
			inputMovement.Enable();
			// Enable Dashing
			inputDash.Enable();
			Debug.Log("Movement Controls Enabled");
		}
		private void Update()
		{
			// Prepare our acceleration before modifying our velocity against the Movement Controller
			CheckInputs();
			DashHandling();
			// Figure our velocity and apply it towards the Movement Controller
			HandleMovement();
			RotateGfx();
		}
		private void CheckInputs()
		{
			moveDirection = inputMovement.ReadValue<Vector2>();
			// Apply our acceleration adjustments based on our movement direction
			if (moveDirection.magnitude > 0)
			{
				acceleration += (new Vector3(moveDirection.x, moveDirection.y) * accelerationRate);
			}
		}
		private void DashHandling()
		{
			dashFactor -= (dashFactor * dashReductionAmount).RoundVector((int) roundingDecimalLimit);
			if (!canDash)
			{
				dashTime += Time.deltaTime;
				if (dashTime >= (dashCoolDown * UpgradeManager.dashTime) / 10)
					dashed = false;
				if (dashTime >= (dashCoolDown * UpgradeManager.dashTime))
				{
					canDash = true;
					// Right here would be where to play the "Dash Ready" sound
					dashTime = 0;
				}
			}
		}
		private void HandleMovement()
		{
			// Applies the current acceleration
			velocity += acceleration;
			acceleration = new Vector3(0, 0, 0);
			// Applies clamping to the velocity based on current player speed upgrades
			float maximumMovement = ((maxMoveSpeed + ((dashTime > 0) ? dashAmount : 0)) * UpgradeManager.moveFactor);
			velocity = new Vector3(Mathf.Clamp(velocity.x, -maximumMovement, maximumMovement),
									Mathf.Clamp(velocity.y, -maximumMovement, maximumMovement), 0);
			
			// Record the current speed
			totalVelocity = (velocity + dashFactor).RoundVector((int) roundingDecimalLimit);
			currentSpeed = (float) Math.Round(totalVelocity.magnitude, (int) roundingDecimalLimit);

			characterController.Move(totalVelocity * Time.deltaTime);
			// Decelerates the current velocity of the player and resets the acceleration
			velocity -= (velocity * decelerationRate).RoundVector((int) roundingDecimalLimit);
		}
		private void RotateGfx()
		{
			float step = bodyRotateSpeed * Time.deltaTime;
			// Figure the angle of rotation from the vector
			Vector3 velocityNormal = velocity.normalized;
			directionAngle = Mathf.LerpAngle(directionAngle, Mathf.Atan2(velocityNormal.x, velocityNormal.y), step);
			// If we have our graphics component referenced, lets set that rotation
			if (playerGraphics != null)
			{
				playerGraphics.rotation = Quaternion.Euler(0f, 0f, directionAngle);
			}
		}
		/// <summary>
		/// Allows you to apply a force to the player.
		/// </summary>
		/// <param name="force"></param>
		public void applyForce(Vector3 force)
		{
			acceleration += force;
		}
		private void OnDisable()
		{
			// Disable input reading because this object is disabled
			inputMovement.Disable();
			inputDash.Disable();
			Debug.Log("Movement Controls Disabled");
		}
	}
}