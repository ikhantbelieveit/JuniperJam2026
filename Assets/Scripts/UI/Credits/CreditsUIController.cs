using JJ26.Input;
using JJ26.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace JJ26.UI
{
	public class CreditsUIController : UIController
	{
		#region UIController

		public override void Initialise()
		{
			base.Initialise();

		}

		public override void SetActive(bool active)
		{
			base.SetActive(active);

			if(active)
			{
				ToggleCredits(false);
			}
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

		[SerializeField] GameObject _infoGO;
		[SerializeField] GameObject _creditsGO;

		[SerializeField] TMP_Text _toggleCreditsText;

		private bool _creditsActive;


		#endregion //UIController

		public void UpdateInput()
		{
			if(InputSystem.UICancelPressed)
			{
				Input_BackButtonPressed();
			}
		}

		public void Input_BackButtonPressed()
		{
			UIStateSystem.EnterPreviousScreen();
		}

		public void Input_ToggleCreditsPressed()
		{
			ToggleCredits(!_creditsActive);
		}

		public void ToggleCredits(bool active)
		{
			_infoGO.SetActive(!active);
			_creditsGO.SetActive(active);

			_toggleCreditsText.text = active ? "Info" : "Credits";

			_creditsActive = active;
		}
	}
}

