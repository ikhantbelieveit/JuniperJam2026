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

			Max
		}

		EUIState _previousState = EUIState.None;
		EUIState _currentState = EUIState.None;

		public EUIState PreviousState => _previousState;
		public EUIState CurrentState => _currentState;

		IUISystem _activeUISystem;

		bool _hasActiveUI;

		IUISystem GetSystemForUIState(EUIState state)
		{
			switch (state)
			{
				case EUIState.PressStart:
					return FindAnyObjectByType(typeof(PressStartUISystem)) as PressStartUISystem;
				default:
					Debug.LogWarning("[UI] No UI System found for UI State " + state.ToString());
					return null;
			}
		}

		public void TransitionToState(EUIState state)
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

		public void EnterScreen(EUIState state)
		{
			//functionality for changing audio goes here

			TransitionToState(state);
		}

		public void EnterPreviousScreen()
		{
			EnterScreen(_previousState);
		}

		public void QuitToDesktop()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif

		}
	}
}
