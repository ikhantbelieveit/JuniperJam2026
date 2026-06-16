using UnityEngine;

namespace JJ26.UI
{
	public interface IUISystem
	{
		public UIController Controller { get; }
		public void SetActive(bool active);
		public void UpdateSystem();
		public void LateUpdateSystem();
	}
}