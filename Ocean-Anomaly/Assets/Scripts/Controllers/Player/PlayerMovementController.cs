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
		private Rigidbody2D rigidBody;
		[SerializeField]
		private Transform playerGraphics;
		// Movement code
		[Header("Movement Settings")]
		[SerializeField]
		private float bodyRotateSpeed = 5.0f;
		[SerializeField]
		[Range(-360, 360)]
		private float bodyAngleOffset = -90f;
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
		private float dashCoolDown = 3f;
		[SerializeField]
		private float dashLength = 1f;
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
		private float dashFactor = 0f;
		[ReadOnly]
		[SerializeField]
		private float dashMultiplier = 0f;
		// Input System
		private PlayerInputActions inputActions;
		private InputAction inputMovement;
		private InputAction inputDash;
		private void Awake()
		{
			if (rigidBody == null)
			{
				rigidBody = GetComponent<Rigidbody2D>();
			}
			InitializeInputActions();
		}
		private void InitializeInputActions()
		{
			inputActions = new PlayerInputActions();
			inputMovement = inputActions.Player.Move;
			inputDash = inputActions.Player.Dash;
			inputDash.performed += DashPerformed;
		}
		private void DashPerformed(InputAction.CallbackContext context)
		{
			if (!canDash)
			{
				return;
			}
			dashed = true;
			canDash = false;
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
		private void FixedUpdate()
		{
			// Apply it to the controller
			rigidBody.velocity = totalVelocity;
		}
		private void CheckInputs()
		{
			moveDirection = inputMovement.ReadValue<Vector2>().normalized;
			// Apply our acceleration adjustments based on our movement direction
			if (moveDirection.magnitude > 0)
			{
				acceleration += (moveDirection.ToVector3() * accelerationRate);
			}
		}
		private void DashHandling()
		{
			// If we can't dash then lets start counting up till we can again
			if (!canDash)
			{
				dashTime += Time.deltaTime;
				// If our time is greater than the coolDown
				if (dashTime >= (dashCoolDown * UpgradeManager.dashTime))
				{
					dashed = false;
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
			// As our dash time increases we will decrease our dashAmount multiplier
			dashFactor = GlobalTools.Map(dashTime, 0, (dashCoolDown * UpgradeManager.dashTime), dashAmount, 0);
			dashMultiplier = dashed && (dashTime <= dashLength) ? (moveDirection.magnitude * dashFactor) : 0;
			// Applies clamping to the velocity based on current player speed upgrades
			float maximumMovement = (maxMoveSpeed * UpgradeManager.moveFactor) + dashMultiplier;
			velocity = Vector3.ClampMagnitude(velocity, maximumMovement);
			
			// Record the current speed
			totalVelocity = velocity.RoundVector((int) roundingDecimalLimit);
			currentSpeed = (float) Math.Round(totalVelocity.magnitude, (int) roundingDecimalLimit);
			// Decelerates the current velocity of the player and resets the acceleration
			velocity -= (velocity * decelerationRate).RoundVector((int)roundingDecimalLimit);
		}
		private void RotateGfx()
		{
			// Figure the angle of rotation from the vector
			Vector3 velocityNormal = velocity.normalized;
			float targetAngle = Mathf.Atan2(velocityNormal.y, velocityNormal.x) * Mathf.Rad2Deg;
			directionAngle = Mathf.LerpAngle(directionAngle, targetAngle + bodyAngleOffset, bodyRotateSpeed * Time.deltaTime);
			// If we have our graphics component referenced, lets set that rotation
			if (playerGraphics != null)
			{
				playerGraphics.rotation = Quaternion.Euler(0f, 0f, directionAngle);
			}
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