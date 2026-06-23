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

		[SerializeField] TMP_Text _gameDataCountText;

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

		public void OnAllGameDataReady()
		{
			foreach (var display in _scoreDisplays)
			{
				display.gameObject.SetActive(false);
			}

			List<PlayerGameData> gameData = GameNetworkManager.Instance.GamePlayers;

			Debug.Log("Game data count: " + gameData.Count);
			_gameDataCountText.text = gameData.Count.ToString();

			for (int index = 0; index < gameData.Count; ++index)
			{
				_scoreDisplays[index].gameObject.SetActive(true);
				_scoreDisplays[index].Initialise(gameData[index]);
				Debug.Log("Setting score display active");
			}
		}


		#endregion //UIController

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