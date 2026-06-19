using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using JJ26.Framework;

namespace JJ26.Input
{
	public enum EControllerType
	{
		Mouse,
		Keyboard,
		Gamepad,

	}

	public enum EControllerPlatformType
	{
		None,
		Xbox,
		Playstation,
		Switch,

	}

	public class InputSystem : BaseGameSystem
	{
		private static readonly string[] INPUT_ACTION_MAP_IDS =
		{
			"UI",
			"Player",
		};

		public delegate void ControllerChangeEvent(EControllerType previous, EControllerType current);
		public delegate void InputEvent();
		public event ControllerChangeEvent OnControllerTypeChanged;
		public event InputEvent OnControllerPlatformTypeChanged;

		[System.Serializable]
		public class ControlState
		{
			[SerializeField]
			bool _currentState;
			bool _previousState;

			public void SetState(bool state)
			{
				_previousState = _currentState;
				_currentState = state;
			}

			public void Clear()
			{
				_previousState = false;
				_currentState = false;
			}

			public bool IsDown
			{
				get => _currentState;
			}

			public bool Pressed
			{
				get => !_previousState && _currentState;
			}

			public bool Released
			{
				get => _previousState && !_currentState;
			}
		}

		protected InputSystem() { }

		public Vector2 MoveCursor { get; private set; }
		public Vector2 MousePosition { get; private set; }

		public bool MouseOverlapsUI { get; private set; }
		public bool FastForward { get; private set; }

		public static bool UIConfirm { get; private set; }
		public static bool UIConfirmPressed { get; private set; }

		public static bool UICancel { get; private set; }
		public static bool UICancelPressed { get; private set; }

		public static Vector2 WalkValue { get; private set; }

		public static bool Jump { get; private set; }
		public static bool JumpPressed { get; private set; }
		public static bool JumpReleased { get; private set; }

		public InputDevice LastInputDevice;

		private PlayerInput _playerInput;
		private int InputDelay = 0;

		[SerializeField] GameObject _inputPrefab;

		#region BaseGameSystem

		public override void Initialise()
		{
			base.Initialise();

			_playerInput = PlayerInput.Instantiate(_inputPrefab);
			DontDestroyOnLoad(_playerInput.gameObject);

			SetUpActionMaps(_playerInput);
			SetUpActions();
		}

		public override void UpdateSystem()
		{
			//base.UpdateSystem();

#if !UNITY_EDITOR
			if(!Application.isFocused)
			{
				Clear();
				return;
			}
#endif

			if (InputDelay > 0)
			{
				InputDelay -= 1;
				return;
			}

			//Clear all press/release values
			UICancelPressed = false;
			UIConfirmPressed = false;
			WalkValue = Vector2.zero;
			JumpPressed = false;
			JumpReleased = false;

#if (UNITY_XBOXONE || UNITY_PS4 || UNITY_GAMECORE || UNITY_SWITCH) && !UNITY_EDITOR
			MousePosition = new Vector2(-9999, -9999);
			Cursor.lockState = CursorLockMode.Locked;
#else
			MousePosition = Mouse.current.position.ReadValue();
#endif

			//UI Cancel
			bool lastUICancel = UICancel;

			UICancel = inputAction_Cancel.ReadValue<float>() > 0;

			if (UICancel && !lastUICancel)
			{
				UICancelPressed = true;
			}

			//UI Confirm
			bool lastUIConfirm = UIConfirm;

			UIConfirm = inputAction_Confirm.ReadValue<float>() > 0;

			if (UIConfirm && !lastUIConfirm)
			{
				UIConfirmPressed = true;
			}

			//Walk value
			WalkValue = inputAction_Walk.ReadValue<Vector2>();
			WalkValue = Vector2.ClampMagnitude(WalkValue, 1);

			//Jump
			bool lastJump = Jump;

			Jump = inputAction_Jump.ReadValue<float>() > 0;

			if(Jump && !lastJump)
			{
				JumpPressed = true;
			}

			if(!Jump && lastJump)
			{
				JumpReleased = true;
			}	
		}

		#endregion // BaseGameSystem

		void SetUpActionMaps(PlayerInput playerInput)
		{
			foreach (string inputActionMapId in INPUT_ACTION_MAP_IDS)
			{
				playerInput.actions.FindActionMap(inputActionMapId).Enable();
			}
		}

		private InputAction inputAction_Confirm;
		private InputAction inputAction_Cancel;
		private InputAction inputAction_Walk;
		private InputAction inputAction_Jump;

		private void SetUpActions()
		{
			inputAction_Cancel = _playerInput.actions["UI/Cancel"];
			inputAction_Confirm = _playerInput.actions["UI/Confirm"];

			inputAction_Walk = _playerInput.actions["Player/Walk"];
			inputAction_Jump = _playerInput.actions["Player/Jump"];
		}

		public void Clear()
		{
			UICancel = false;
			UICancelPressed = false;

			UIConfirm = false;
			UIConfirmPressed = false;
		}
	}
}