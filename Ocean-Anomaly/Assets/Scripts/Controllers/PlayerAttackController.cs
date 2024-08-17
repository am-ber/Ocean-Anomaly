using UnityEngine;
using UnityEngine.InputSystem;

namespace OceanAnomaly.Controllers
{
	public class PlayerAttackController : MonoBehaviour
	{
		[SerializeField]
		private PlayerMovementController movementController;
		[SerializeField]
		private CrosshairController crosshair;
		private PlayerInputActions inputActions;
		private InputAction inputAttack;
		private InputAction inputAim;
		private void Awake()
		{
			inputActions = new PlayerInputActions();

			if (movementController == null)
			{
				movementController = GetComponent<PlayerMovementController>();
			}
			if (crosshair == null)
			{
				crosshair = GetComponentInChildren<CrosshairController>();
			}
		}
		private void OnEnable()
		{
			// Looking needs to run in constant update because of the constant vector changes
			inputAim = inputActions.Player.Look;
			inputAim.Enable();
			// This can have an instant OnFire() command
			inputAttack = inputActions.Player.Fire;
			inputAttack.Enable();
		}
		private void Update()
		{

		}
		private void OnDisable()
		{
			inputAim.Disable();
			inputAttack.Disable();
		}
	}
}