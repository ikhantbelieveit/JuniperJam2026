using UnityEngine;
using UnityEngine.UI;
using JJ26.Framework;

namespace JJ26.UI
{
	public class MainMenuUISystem : BaseGameSystem, IUISystem
	{
		#region IUISystem
		public UIController Controller => _controller;

		private UIStateSystem _uiStateSystem;

		public void SetActive(bool active)
		{
			if (!_isInitialised) { return; }
			_controller.SetActive(active);
			_active = active;
		}

		#endregion //IUISystem

		#region BaseGameSystem

		public override void Initialise()
		{
			base.Initialise();

			_uiStateSystem = FindAnyObjectByType(typeof(UIStateSystem)) as UIStateSystem;

			InitialiseController();
		}

		private void InitialiseController()
		{
			if (null == _uiPrefab)
			{
				Debug.LogError("No UI prefab asset for system " + this.name);
				return;
			}

			GameObject uiRoot = Instantiate(_uiPrefab);
			uiRoot.transform.SetParent(transform);
			uiRoot.transform.position = Vector3.zero;
			uiRoot.transform.rotation = Quaternion.identity;
			uiRoot.transform.localScale = Vector3.one;
			_controller = uiRoot.GetComponent<MainMenuUIController>();
			_controller.Initialise();
			_controller.SetActive(false);
		}

		public override void UpdateSystem()
		{
			base.UpdateSystem();
			_controller.UpdateController();
		}

		#endregion //BaseGameSystem

		[SerializeField] GameObject _uiPrefab;

		private static MainMenuUIController _controller;
		private bool _active = false;
		public bool Active => _active;

		public static string GetDisplayName()
		{
			return _controller.DisplayName;
		}

		#region InputSignals


		#endregion //InputSignals
	}
}

