using OceanAnomaly.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OceanAnomaly.Controllers
{
	public class PlayerMovementController : MonoBehaviour, Observer<PlayerInputController, PlayerInputData>
	{
		[SerializeField]
		private float movementSpeed = 5f;
		[ReadOnly]
		[SerializeField]
		private float currentSpeed = 0f;
		[ReadOnly]
		[SerializeField]
		private Vector2 moveDirection = Vector2.zero;
		private void Update()
		{

		}
		private void FixedUpdate()
		{

		}
		public void OnNotify(PlayerInputController caller, PlayerInputData input)
		{

		}
	}
}