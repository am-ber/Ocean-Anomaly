using OceanAnomaly.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OceanAnomaly.Controllers
{
	public class PlayerMovementController : MonoBehaviour
	{
		[SerializeField]
		private PlayerInputActions inputActions;
		[SerializeField]
		private float movementSpeed = 5f;
		[ReadOnly]
		[SerializeField]
		private float currentSpeed = 0f;
		[ReadOnly]
		[SerializeField]
		private Vector2 moveDirection = Vector2.zero;
		private InputAction inputMovement;
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
	}
}