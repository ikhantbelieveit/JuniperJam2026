using JJ26.Input;
using JJ26.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace JJ26.UI
{
	public class GameplayUIController : UIController
	{

		private GameNetworkManager _networkManager;

		#region UIController

		public override void Initialise()
		{
			base.Initialise();

			_networkManager = FindAnyObjectByType(typeof(GameNetworkManager)) as GameNetworkManager;
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
	}
}

