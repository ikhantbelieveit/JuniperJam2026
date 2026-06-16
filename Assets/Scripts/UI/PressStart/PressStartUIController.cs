using UnityEngine;
using UnityEngine.UI;

namespace JJ26.UI
{
    public class PressStartUIController : UIController
    {
		private PressStartUISystem _pressStartUISystem;

		#region UIController

		public override void Initialise()
		{
			base.Initialise();

			_pressStartUISystem = FindAnyObjectByType(typeof(PressStartUISystem)) as PressStartUISystem;
		}

		public override void SetActive(bool active)
		{
			base.SetActive(active);
		}

		public override void UpdateController()
		{
			if (!_isActive)
			{
				return;
			}

			base.UpdateController();

			UpdateInput();
		}

		#endregion //UIController

		public void UpdateInput()
		{

		}

		#region InputSignals

		public void Input_StartButtonClicked()
		{
			_pressStartUISystem.Input_StartButtonPress();
		}

		public void Input_CreditsButtonClicked()
		{
			_pressStartUISystem.Input_CreditsButtonPress();
		}

		#endregion //InputSignals
	}
}

