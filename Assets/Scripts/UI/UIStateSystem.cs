using UnityEngine;
using JJ26.Framework;

namespace JJ26.UI
{
	public class UIStateSystem : BaseGameSystem
	{
		#region BaseGameSystem
		public override void UpdateSystem()
		{
			base.UpdateSystem();
		}
		#endregion

		public enum EUIState
		{
			None,
			PressStart,
			MainMenu,
			Lobby,
			Gameplay,
			Credits,

			Max
		}

		static EUIState _previousState = EUIState.None;
		static EUIState _currentState = EUIState.None;

		public static EUIState PreviousState => _previousState;
		public static EUIState CurrentState => _currentState;

		static IUISystem _activeUISystem;

		static bool _hasActiveUI;

		static IUISystem GetSystemForUIState(EUIState state)
		{
			switch (state)
			{
				case EUIState.PressStart:
					return FindAnyObjectByType(typeof(PressStartUISystem)) as PressStartUISystem;
				case EUIState.MainMenu:
					return FindAnyObjectByType(typeof(MainMenuUISystem)) as MainMenuUISystem;
				case EUIState.Lobby:
					return FindAnyObjectByType(typeof(LobbyUISystem)) as LobbyUISystem;
				case EUIState.Gameplay:
					return FindAnyObjectByType(typeof(GameplayUISystem)) as GameplayUISystem;
				case EUIState.Credits:
					return FindAnyObjectByType(typeof(CreditsUISystem)) as CreditsUISystem;
				default:
					Debug.LogWarning("[UI] No UI System found for UI State " + state.ToString());
					return null;
			}
		}

		public static void TransitionToState(EUIState state)
		{
			if (state == _currentState)
			{
				return;
			}

			//Must clear InputSystem here
			//Input.InputSystem.Instance.Clear();

			_previousState = _currentState;
			_currentState = state;

			bool hasActiveUIBeforeTransition = _activeUISystem != null;

			if (hasActiveUIBeforeTransition)
			{
				_activeUISystem.SetActive(false);
			}

			_activeUISystem = GetSystemForUIState(state);
			_hasActiveUI = _activeUISystem != null;

			if (_hasActiveUI)
			{
				_activeUISystem.SetActive(true);
			}

		}

		public void HideAllUI()
		{
			EnterScreen(EUIState.None);
		}

		public static void EnterScreen(EUIState state)
		{
			//functionality for changing audio goes here

			TransitionToState(state);
		}

		public static void EnterPreviousScreen()
		{
			EnterScreen(_previousState);
		}

		public static void QuitToDesktop()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif

		}
	}
}
