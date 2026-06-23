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

		[SerializeField] List<PlayerScoreDisplay> _scoreDisplays;

		#region UIController

		public override void Initialise()
		{
			base.Initialise();

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

		public void OnLevelStarted()
		{
			foreach(var display in _scoreDisplays)
			{
				display.gameObject.SetActive(false);
			}

			List<PlayerGameData> gameData = GameNetworkManager.Instance.GamePlayers;

			for(int index = 0; index < gameData.Count; ++index)
			{
				_scoreDisplays[index].gameObject.SetActive(true);
				_scoreDisplays[index].Initialise(gameData[index]);
			}
		}

		public void UpdateInput()
		{
			if(InputSystem.UICancelPressed)
			{
				var gameData = GameNetworkManager.Instance.GetLocalGameData();
				gameData.CmdExitGame();
			}
		}
	}
}