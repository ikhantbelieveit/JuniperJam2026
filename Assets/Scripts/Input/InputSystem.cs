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

		public bool UIConfirm { get; private set; }
		public bool UIConfirmPressed { get; private set; }

		public bool UICancel { get; private set; }
		public bool UICancelPressed { get; private set; }

		EControllerType _lastControllerType;
		public EControllerType LastControllerType
		{
			get { return _lastControllerType; }
			set
			{
				EControllerType prevType = _lastControllerType;
				bool change = _lastControllerType != value;
				_lastControllerType = value;
				if (change)
				{
					OnControllerTypeChanged?.Invoke(prevType, _lastControllerType);
				}
			}
		}

		EControllerPlatformType _lastControllerPlatformType;
		public EControllerPlatformType LastControllerPlatformType
		{
			get { return _lastControllerPlatformType; }
			set
			{
				bool change = _lastControllerPlatformType != value;
				_lastControllerPlatformType = value;
				if (change)
				{
					OnControllerPlatformTypeChanged?.Invoke();
				}
			}
		}

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

			//Clear all values
			UICancelPressed = false;
			UIConfirmPressed = false;

			EControllerPlatformType platformType = LastControllerPlatformType;
			LastControllerType = GetCurrentController(out platformType, out LastInputDevice);
			LastControllerPlatformType = platformType;

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
		}

		#endregion // BaseGameSystem

		void SetUpActionMaps(PlayerInput playerInput)
		{
#if UNITY_SWITCH
			//SetUpActionMapsForSwitch(playerInput)
			return;
#else
			foreach (string inputActionMapId in INPUT_ACTION_MAP_IDS)
			{
				playerInput.actions.FindActionMap(inputActionMapId).Enable();
			}
#endif  //UNITY_SWITCH
		}

		private InputAction inputAction_Confirm;
		private InputAction inputAction_Cancel;

		private void SetUpActions()
		{
			inputAction_Cancel = _playerInput.actions["UI/Cancel"];
			inputAction_Confirm = _playerInput.actions["UI/Confirm"];
		}

		public void Clear()
		{
			UICancel = false;
			UICancelPressed = false;

			UIConfirm = false;
			UIConfirmPressed = false;
		}

		EControllerType GetCurrentController(out EControllerPlatformType lastPlatformType, out InputDevice lastDevice)
		{
			lastDevice = LastInputDevice;
			EControllerType mostRecentType = LastControllerType;

			InputDevice activeDevice = null;
			InputDevice activeNonGamepad = null;
			InputDevice activeGamepad = null;
			double lastUpdate = 0;
			lastPlatformType = LastControllerPlatformType;

			bool hasKeyboard = Keyboard.current != null;
			bool hasMouse = Mouse.current != null;
			bool hasGamepad = Gamepad.current != null;

			if (hasGamepad && Gamepad.current.lastUpdateTime > lastUpdate)
			{
				mostRecentType = EControllerType.Gamepad;
				activeDevice = Gamepad.current;
				activeGamepad = Gamepad.current;
				lastUpdate = activeDevice.lastUpdateTime;
			}

			if (hasKeyboard && Keyboard.current.lastUpdateTime > lastUpdate)
			{
				mostRecentType = EControllerType.Keyboard;
				activeDevice = Keyboard.current;
				activeGamepad = Keyboard.current;
				lastUpdate = activeDevice.lastUpdateTime;
			}

			if (hasMouse && Mouse.current.lastUpdateTime > lastUpdate)
			{
				mostRecentType = EControllerType.Mouse;
				activeDevice = Mouse.current;
				activeGamepad = Mouse.current;
				lastUpdate = activeDevice.lastUpdateTime;
			}

			if (activeDevice == lastDevice)
			{
				lastPlatformType = LastControllerPlatformType;
				return LastControllerType;
			}

			if (hasGamepad && Gamepad.current.lastUpdateTime == lastUpdate && IsGamepadInputAcceptable(Gamepad.current))
			{
				lastDevice = activeGamepad;
#if UNITY_SWITCH
				lastPlatformType = EControllerPlatformType.Switch;
#else
				if (Gamepad.current is UnityEngine.InputSystem.XInput.XInputController)
				{
					lastPlatformType = EControllerPlatformType.Xbox;
				}
				else if (Gamepad.current is UnityEngine.InputSystem.DualShock.DualShockGamepad)
				{
					lastPlatformType = EControllerPlatformType.Playstation;
				}
				else if (Gamepad.current is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
				{
					lastPlatformType = EControllerPlatformType.Switch;
				}
#endif
				return EControllerType.Gamepad;
			}
			else
			{
				lastDevice = activeNonGamepad;
#if UNITY_GAMECORE
				lastPlatformType = EControllerPlatformType.Xbox;
#elif UNITY_SWITCH
				lastPlatformType =e EControllerPlatformType.Switch;
#else
				lastPlatformType = EControllerPlatformType.None;
#endif
				return mostRecentType;
			}

		}

		private bool IsGamepadInputAcceptable(Gamepad gamepad)
		{
			for (int controlIndex = 0; controlIndex < gamepad.allControls.Count; ++controlIndex)
			{
				InputControl control = gamepad.allControls[controlIndex];
				if (control.valueType == typeof(float))
				{
					float inputFloat = (float)control.ReadValueAsObject();
					if (inputFloat >= 0.2f)
					{
						return true;
					}
				}
				else if (control.valueType == typeof(Vector2))
				{
					Vector2 inputAxes = (Vector2)control.ReadValueAsObject();
					if (inputAxes.sqrMagnitude >= 0.5f)
					{
						return true;
					}
				}
			}

			return false;
		}


	}
}