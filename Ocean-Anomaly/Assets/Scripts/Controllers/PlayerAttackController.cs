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
		private InputAction inputAim;
		[SerializeField]
		private float deltaPullActivation = 3f;
		[SerializeField]
		private float aimSpeed = 10f;
		[SerializeField]
		private float aimSmoothing = 0.5f;
		[ReadOnly]
		public float aimAngle = 0f;
		[ReadOnly]
		[SerializeField]
		private float targetAimAngle = 0f;
		// Attacking
		[Header("Attack Variables")]
		private InputAction inputAttack;
		[ReadOnly]
		[SerializeField]
		private bool canAttack = true;
		[ReadOnly]
		[SerializeField]
		private float timeTillAttack = 0;
		private void Awake()
		{
			inputActions = new PlayerInputActions();

			if (movementController == null)
			{
				movementController = GetComponent<PlayerMovementController>();
			}
			if (indicator == null)
			{
				indicator = GetComponentInChildren<IndicatorController>();
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
			Debug.Log("Attack Controls Enabled");
		}
		private void Update()
		{
			CheckForAiming();
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
			Vector3 crosshairPosition = crosshair.transform.position - transform.position;
			targetAimAngle = Mathf.Atan2(crosshairPosition.y, crosshairPosition.x) * Mathf.Rad2Deg;
			// Calculate the rotation angle and then rotate the crosshair along its forward direction (Z axis)
			aimAngle = Mathf.LerpAngle(aimAngle, targetAimAngle, aimSpeed * Time.deltaTime);
			indicator.transform.rotation = Quaternion.AngleAxis(aimAngle - (90), Vector3.forward);
		}
		private void CheckForAttacking()
		{
			if (inputAttack.ReadValue<Single>() > 0)
			{
				if (CurrentWeapon == null)
				{
					return;
				}
				if (CurrentWeapon.weaponFunction != null)
				{
					this.StartCoroutine(0f, () => { });
				}
				indicator.ResetAttackIndicator();
				indicator.FadeAttackIndicator();
				canAttack = false;
				// Using the global attack upgrade will only work if we constantly disable and 
				// re-enabled the attack controller (which honestly, we probably should do)...
				timeTillAttack = CurrentWeapon.attackTime * PlayerUpgradeManager.attackTime;
			}
		}
		private void OnDisable()
		{
			inputAim.Disable();
			inputAttack.Disable();
			Debug.Log("Attack Controls Disabled");
		}
	}
}