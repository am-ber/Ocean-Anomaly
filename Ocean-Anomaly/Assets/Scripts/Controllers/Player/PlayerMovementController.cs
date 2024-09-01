using OceanAnomaly.Attributes;
using OceanAnomaly.Managers;
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
		private float bodyRotateSpeed = 1.0f;
		[SerializeField]
		private float maxMoveSpeed = 8f;
		[SerializeField]
		[Range(0f, 1f)]
		private float decelerationRate = 0.9f;
		[SerializeField]
		[Range(0f, 1f)]
		private float accelerationRate = 0.8f;
		[ReadOnly]
		[SerializeField]
		private float currentSpeed = 0f;
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
		private float dashCoolDown = 2;
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
			CheckInputs();
			DashHandling();
			RotateGfx();
		}
		private void FixedUpdate()
		{
			// We should only run these lines if we have movement
			if (moveDirection.magnitude > 0)
			{
				acceleration += (new Vector3(moveDirection.x, moveDirection.y) * accelerationRate * Time.fixedDeltaTime * 100);
			}
			// Applies the current acceleration
			velocity += acceleration;
			// Applies clamping to the velocity based on current player speed upgrades
			velocity = new Vector3(Mathf.Clamp(velocity.x, -(maxMoveSpeed * UpgradeManager.moveFactor), (maxMoveSpeed * UpgradeManager.moveFactor)),
					Mathf.Clamp(velocity.y, -(maxMoveSpeed * UpgradeManager.moveFactor), (maxMoveSpeed * UpgradeManager.moveFactor)), 0);
			// Decelerates the current velocity of the player and resets the acceleration
			velocity = (velocity * decelerationRate);
			acceleration = new Vector3(0, 0, 0);

			// Record the current speed
			totalVelocity = velocity + dashFactor;
			currentSpeed = totalVelocity.magnitude;

			rigidBody.MovePosition(new Vector3(rigidBody.position.x, rigidBody.position.y) + totalVelocity * Time.fixedDeltaTime);
		}
		/// <summary>
		/// Allows you to apply a force to the player.
		/// </summary>
		/// <param name="force"></param>
		public void applyForce(Vector3 force)
		{
			acceleration += force;
		}
		private void CheckInputs()
		{
			moveDirection = inputMovement.ReadValue<Vector2>();
		}
		private void DashHandling()
		{
			dashFactor = dashFactor * 0.9f;
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
		private void OnDisable()
		{
			// Disable input reading because this object is disabled
			inputMovement.Disable();
			inputDash.Disable();
			Debug.Log("Movement Controls Disabled");
		}
		private void RotateGfx()
		{
			float step = bodyRotateSpeed * Time.deltaTime;
			Vector3 velocityNormal = velocity.normalized;
			if (playerGraphics != null)
			{
				playerGraphics.rotation = Quaternion.Euler(velocityNormal);
			}
		}
	}
}