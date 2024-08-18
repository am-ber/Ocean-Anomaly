using OceanAnomaly.Attributes;
using OceanAnomaly.Managers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OceanAnomaly.Controllers
{
	public class PlayerMovementController : MonoBehaviour
	{
		[SerializeField]
		private Rigidbody2D rigidbody;
		// Movement code
		[Header("Movement Settings")]
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
		[SerializeField]
		private PlayerInput playerInput;
		private InputAction inputMovement;
		private InputAction inputDash;
		private PlayerInputActions inputActions;
		private void Awake()
		{
			if (playerInput == null)
			{
				playerInput = GetComponent<PlayerInput>();
				
				inputActions = new PlayerInputActions();
			}
			if (rigidbody == null)
			{
				rigidbody = GetComponent<Rigidbody2D>();
			}
		}
		
		private void OnEnable()
		{
			// Enable Movement
			inputMovement = inputActions.Player.Move;
			inputMovement.Enable();
			// Enable Dashing
			inputDash = inputActions.Player.Dash;
			inputDash.Enable();
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
			Debug.Log("Movement Controls Enabled");
		}
		private void Update()
		{
			CheckInputs();
			DashHandling();
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
			velocity = new Vector3(Mathf.Clamp(velocity.x, -(maxMoveSpeed * PlayerUpgradeManager.moveFactor), (maxMoveSpeed * PlayerUpgradeManager.moveFactor)),
					Mathf.Clamp(velocity.y, -(maxMoveSpeed * PlayerUpgradeManager.moveFactor), (maxMoveSpeed * PlayerUpgradeManager.moveFactor)), 0);
			// Decelerates the current velocity of the player and resets the acceleration
			velocity = (velocity * decelerationRate);
			acceleration = new Vector3(0, 0, 0);

			// Record the current speed
			totalVelocity = velocity + dashFactor;
			currentSpeed = totalVelocity.magnitude;

			rigidbody.MovePosition(new Vector3(rigidbody.position.x, rigidbody.position.y) + totalVelocity * Time.fixedDeltaTime);
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
				if (dashTime >= (dashCoolDown * PlayerUpgradeManager.dashTime) / 10)
					dashed = false;
				if (dashTime >= (dashCoolDown * PlayerUpgradeManager.dashTime))
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
	}
}