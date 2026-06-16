using UnityEngine;
using JJ26.Framework;

namespace JJ26.UI
{
    public class PressStartUISystem : BaseGameSystem, IUISystem
    {
		#region IUISystem

		public UIController Controller => _controller;

		public void SetActive(bool active)
		{
			if(!_isInitialised) { return; }
			_controller.SetActive(active);
			_active = active;
		}

		#endregion //IUISystem

		#region BaseGameSystem

		public override void Initialise()
		{
			base.Initialise();

			if(null == _uiPrefab)
			{
				Debug.LogError("No UI prefab asset for system " + this.name);
				return;
			}

			GameObject uiRoot = Instantiate(_uiPrefab);
			uiRoot.transform.SetParent(transform);
			uiRoot.transform.position = Vector3.zero;
			uiRoot.transform.rotation = Quaternion.identity;
			uiRoot.transform.localScale = Vector3.one;
			_controller = uiRoot.GetComponent<PressStartUIController>();
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

		private PressStartUIController _controller;
		private bool _active = false;
		public bool Active => _active;
	}
}

