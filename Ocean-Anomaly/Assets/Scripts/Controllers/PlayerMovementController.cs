using OceanAnomaly.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OceanAnomaly.Controllers
{
	public class PlayerMovementController : MonoBehaviour
	{
		[SerializeField]
		private float movementSpeed = 5f;
		[ReadOnly]
		[SerializeField]
		private float currentSpeed = 0f;
		[ReadOnly]
		[SerializeField]
		private Vector2 moveDirection = Vector2.zero;
		private PlayerInputActions inputActions;
		public LinkedList<PlayerInputData> inputBuffer;
		[SerializeField]
		private int inputBufferLimit = 20;
		private void Awake()
		{
			inputActions = new PlayerInputActions();
		}
		private void Update()
		{

		}
		private void FixedUpdate()
		{

		}
		private void InputCaller(InputAction.CallbackContext context)
		{
			Debug.Log($"{context.action.name} : {context.control.name} [{context.action.ReadValueAsObject().GetType()}: " +
				$"{context.action.ReadValueAsObject()}] ({context.duration.ToString("F3")})");

		}
		private void OnEnable()
		{
			foreach (var action in inputActions.Player.Get())
			{
				action.performed += InputCaller;
				action.Enable();
			}
		}
		private void OnDisable()
		{
			foreach (var action in inputActions.Player.Get())
			{
				action.Disable();
			}
		}
	}
}