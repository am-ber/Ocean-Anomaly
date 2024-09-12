using OceanAnomaly.Attributes;
using OceanAnomaly.Components.Weapon;
using OceanAnomaly.Managers;
using OceanAnomaly.Tools;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OceanAnomaly.Controllers
{
	public class PlayerAttackController : MonoBehaviour
	{
		[Header("Generic Variables")]
		public bool lockCursor = true;
		[ExposedScriptableObject]
		public WeaponBaseScriptable HarpoonGun;
		[ExposedScriptableObject]
		public WeaponBaseScriptable CannonGun;
		[ExposedScriptableObject]
		public WeaponBaseScriptable CurrentWeapon;
		[SerializeField]
		private PlayerMovementController movementController;
		[SerializeField]
		private IndicatorController indicator;
		public Transform AttackIndicator;
		[SerializeField]
		private CrosshairController crosshair;
		private PlayerInputActions inputActions;
		// Aiming
		[Header("Aiming Variables")]
		[SerializeField]
		private float aimSpeed = 10f;
		[ReadOnly]
		public float aimAngle = 0f;
		[ReadOnly]
		[SerializeField]
		private float targetAimAngle = 0f;
		[ReadOnly]
		[SerializeField]
		private Vector2 lookDirection = Vector2.zero;
		// Attacking
		[Header("Attack Variables")]
		private InputAction inputAttack;
		private InputAction inputCycleWeapons;
		private InputAction inputLook;
		[SerializeField]
		private float accuracy = 0f;
		[ReadOnly]
		[SerializeField]
		private bool canAttack = true;
		[ReadOnly]
		[SerializeField]
		private float timeTillAttack = 0;
		[ReadOnly]
		[SerializeField]
		private bool isKeyboardAndMouse = true;
		private void Awake()
		{
			// Setup input actions
			inputActions = new PlayerInputActions();
			inputAttack = inputActions.Player.Fire;
			inputCycleWeapons = inputActions.Player.CycleWeapon;
			inputLook = inputActions.Player.Look;
			// Find components that may be missing
			if (movementController == null)
			{
				movementController = GetComponent<PlayerMovementController>();
			}
			if (indicator == null)
			{
				indicator = GetComponentInChildren<IndicatorController>();
			}
			// Hard coded for now of setting the CurrentWeapon
			CurrentWeapon = HarpoonGun;
		}
		private void Start()
		{
			GlobalManager.Instance?.OnInputActionChange.AddListener(OnInputChange);
		}
		private void OnEnable()
		{
			inputCycleWeapons.performed += SwitchWeapon;
			inputAttack.Enable();
			inputCycleWeapons.Enable();
			inputLook.Enable();
			Debug.Log("Attack Controls Enabled");
		}
		private void Update()
		{
			if (isKeyboardAndMouse)
			{
				CheckForCrosshairAiming();
			} else
			{
				CheckForAiming();
			}
			// Indicator rotation
			indicator.transform.rotation = Quaternion.AngleAxis(aimAngle - (90), Vector3.forward);
			// If we can't attack it's likely because we need to subtract our TimeTillAttack
			if (canAttack)
			{
				CheckForAttacking();
			} else
			{
				timeTillAttack -= Time.deltaTime;
				if (timeTillAttack <= 0)
				{
					canAttack = true;
					timeTillAttack = 0;
				}
			}
		}
		private void CheckForAiming()
		{
			// If our crosshair isn't disabled, lets do that and then come back to Check for the input delta
			if (!crosshair.SoftDisabled)
			{
				crosshair.SoftDisabled = true;
				return;
			}
			lookDirection = inputLook.ReadValue<Vector2>().normalized;
			if (lookDirection.magnitude > 0)
			{
				targetAimAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
			}
			// Calculate the rotation angle and then rotate the crosshair along its forward direction (Z axis)
			aimAngle = Mathf.LerpAngle(aimAngle, targetAimAngle, aimSpeed * Time.deltaTime);
		}
		private void CheckForCrosshairAiming()
		{
			if (crosshair.SoftDisabled)
			{
				crosshair.SoftDisabled = false;
				return;
			}
			Vector3 crosshairPosition = crosshair.transform.position - transform.position;
			targetAimAngle = Mathf.Atan2(crosshairPosition.y, crosshairPosition.x) * Mathf.Rad2Deg;
			// Calculate the rotation angle and then rotate the crosshair along its forward direction (Z axis)
			aimAngle = Mathf.LerpAngle(aimAngle, targetAimAngle, aimSpeed * Time.deltaTime);
		}
		private void CheckForAttacking()
		{
			if (inputAttack.ReadValue<Single>() > 0)
			{
				if (CurrentWeapon == null)
				{
					return;
				}
				// Call fired
				indicator?.Fired();
				
				if (CurrentWeapon.weaponFunction != null)
				{
					CurrentWeapon.weaponFunction.FirePorjectile(
						Instantiate(CurrentWeapon.weaponFunction.projectilePrefab, indicator.transform.position, indicator.transform.rotation),
						aimAngle, accuracy, crosshair.transform.position);
				}
				// Play our sound if we have it
				if (CurrentWeapon.fireSound != null)
				{
					CurrentWeapon.fireSound.Play(gameObject);
				}
				canAttack = false;
				// Using the global attack upgrade will only work if we constantly disable and 
				// re-enabled the attack controller (which honestly, we probably should do)...
				timeTillAttack = CurrentWeapon.attackTime * UpgradeManager.attackTime;
			}
		}
		private void SwitchWeapon(InputAction.CallbackContext context)
		{
			if (CurrentWeapon.Equals(HarpoonGun))
			{
				CurrentWeapon = CannonGun;
			} else
			{
				CurrentWeapon = HarpoonGun;
			}
		}
		private void OnDisable()
		{
			inputAttack.Disable();
			inputCycleWeapons.Disable();
			inputLook.Disable();
			inputCycleWeapons.performed -= SwitchWeapon;
			Debug.Log("Attack Controls Disabled");
		}
		private void OnInputChange(InputDevice device)
		{
			isKeyboardAndMouse = device.name.Equals("Keyboard") || device.name.Equals("Mouse");
		}
	}
}