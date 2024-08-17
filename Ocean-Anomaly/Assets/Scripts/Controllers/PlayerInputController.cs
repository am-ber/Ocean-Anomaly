using OceanAnomaly;
using OceanAnomaly.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : Subject<PlayerInputController, PlayerInputData>
{
	[ListDrawer]
	public LinkedList<PlayerInputData> InputBuffer;
	public int InputBufferLimit = 10;
	public PlayerInput PlayerInput;
	private PlayerInputActions inputActions;
	private void Awake()
	{
		if (PlayerInput == null)
		{
			PlayerInput = gameObject.AddComponent<PlayerInput>();
		}
		inputActions = new PlayerInputActions();
		InputBuffer = new LinkedList<PlayerInputData>();
	}
	private void InputCaller(InputAction.CallbackContext context)
	{
		PlayerInputAction action = PlayerInputAction.None;
		bool heldAction = false;
		bool log = false;
		// Some of these objects may not cast correctly if the input type is changed without adjusting the code
		// So better safe than sorry for checking the input to handle what we can, when we can
		try
		{
			// God I hate the look of these ifs but they have to be dynamic which doesn't work for switches
			if (context.action.id == inputActions.Player.Move.id)
			{
				log = true;
				Vector2 moveDirection = context.action.ReadValue<Vector2>();
				// If our direction doesn't equal zero then we are holding this value
				heldAction = (moveDirection != Vector2.zero);
				if (moveDirection == Vector2.up)
				{
					action = PlayerInputAction.Up;
				}
				if (moveDirection == Vector2.down)
				{
					action = PlayerInputAction.Down;
				}
				if (moveDirection == Vector2.left)
				{
					action = PlayerInputAction.Left;
				}
				if (moveDirection == Vector2.right)
				{
					action = PlayerInputAction.Right;
				}
			}
			if (context.action.id == inputActions.Player.Fire.id)
			{
				log = true;
				action = PlayerInputAction.Attack;
				heldAction = (context.action.ReadValue<Single>() >= 0);
				
			}
			if (context.action.id == inputActions.Player.Dash.id)
			{
				log = true;
				action = PlayerInputAction.Dash;
				heldAction = (context.action.ReadValue<Single>() >= 0);
			}
		}
		catch (Exception e)
		{
			Debug.Log($"{e.Message}: {e.StackTrace}");
			GlobalVariables.CurrentError = $"Inside InputCaller: {e.Message}";
		}
		// This is to control which inputs can actually log because moving the mouse spamming console is no fun for anyone
		if (log)
		{
			Debug.Log($"{context.action.name} : {context.control.name} [{context.action.ReadValueAsObject().GetType()}: " +
				$"{context.action.ReadValueAsObject()}] ({context.duration.ToString("F3")})");
		}
		// Add whatever we resolve from our input action into the input buffer as a new input
		InputBuffer.AddLast(new PlayerInputData(action, heldAction, context.duration));
		// Remove inputs that 
		if (InputBuffer.Count > InputBufferLimit)
		{
			InputBuffer.RemoveFirst();
		}
		// Send a message to all observers looking at the player for input
		Notify(this, InputBuffer.Last.Value);
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
