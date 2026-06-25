using JJ26.Input;
using JJ26.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace JJ26.UI
{
	public class GameplayUIController : UIController
	{

		[SerializeField] List<PlayerScoreDisplay> _scoreDisplays;

		[SerializeField] TMP_Text _countdownText;
		[SerializeField] TMP_Text _matchTimeText;
		[SerializeField] TMP_Text _returnTimeText;

		[SerializeField] GameObject _countdownTextGO;
		[SerializeField] GameObject _matchTimeTextGO;

		[SerializeField] Color _countdownColour3 = Color.red;
		[SerializeField] Color _countdownColour2 = Color.orange;
		[SerializeField] Color _countdownColour1 = Color.yellow;
		[SerializeField] Color _countdownColourGo = Color.green;

		[SerializeField] GameObject _powerWheelGO;
		[SerializeField] GameObject _steeringWheelGO;

		[SerializeField] DirectionWheel _steeringWheel;
		public DirectionWheel SteeringWheel => _steeringWheel;

		[SerializeField] DirectionWheel _powerWheel;
		public DirectionWheel PowerWheel => _powerWheel;

		[SerializeField] GameObject _postMatchGO;

		[SerializeField] TMP_Text _winnerNameText;
		[SerializeField] TMP_Text _winnerScoreText;

		[SerializeField] List<LeaderboardEntry> _leaderboardEntries;

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
				var gameStateData = GameNetworkManager.Instance.GameState;
				EGameState gameState = (null == gameStateData) ? EGameState.Countdown : gameStateData.CurrentState;
				RefreshActiveObjectsForState(gameState);
				_steeringWheel.ResetWheel();
				_powerWheel.ResetWheel();
			}
		}

		public override void UpdateController()
		{
			if (!_isActive)
			{
				return;
			}

			base.UpdateController();

			switch (GameNetworkManager.Instance.GameState?.CurrentState)
			{
				case EGameState.Countdown:
					int roundedTime = (int)GameNetworkManager.Instance.GameState.CountdownTimeRemaining;
					_countdownText.SetText(roundedTime.ToString());
					switch(roundedTime)
					{
						default:
							_countdownText.color = _countdownColour3;
							break;
						case 2:
							_countdownText.color = _countdownColour2;
							break;
						case 1:
							_countdownText.color = _countdownColour1;
							break;
						case 0:
							_countdownText.color = _countdownColourGo;
							_countdownText.SetText("AHOY!!");
							break;
					}
					break;
				case EGameState.Gameplay:
					float time = GameNetworkManager.Instance.GameState.MatchTimeRemaining;
					TimeSpan t = TimeSpan.FromSeconds(time);
					string display = $"{t.Minutes:00}:{t.Seconds:00}";

					_matchTimeText.SetText(display);
					break;
				case EGameState.PostMatch:
					int roundedTimePostMatch = (int)GameNetworkManager.Instance.GameState.PostMatchTimeRemaining;
					_returnTimeText.SetText("Return in: " + roundedTimePostMatch.ToString());
					break;
			}

			UpdateInput();
		}

		void RefreshActiveObjectsForState(EGameState gameState)
		{
			_countdownTextGO.SetActive(gameState == EGameState.Countdown);
			_matchTimeTextGO.SetActive(gameState == EGameState.Gameplay);
			_powerWheelGO.SetActive(gameState == EGameState.Gameplay);
			_steeringWheelGO.SetActive(gameState == EGameState.Gameplay);
			_postMatchGO.SetActive(gameState == EGameState.PostMatch);
		}


		public void OnAllGameDataReady()
		{
			foreach (var display in _scoreDisplays)
			{
				display.gameObject.SetActive(false);
			}

			List<PlayerGameData> gameData = GameNetworkManager.Instance.GamePlayers;

			for (int index = 0; index < gameData.Count; ++index)
			{
				_scoreDisplays[index].gameObject.SetActive(true);
				_scoreDisplays[index].Initialise(gameData[index]);
			}
		}

		public void RefreshScoreValues()
		{
			List<PlayerGameData> gameData = GameNetworkManager.Instance.GamePlayers;

			for (int index = 0; index < gameData.Count; ++index)
			{
				_scoreDisplays[index].SetScoreText(gameData[index].Score);
			}
		}


		#endregion //UIController

		public void UpdateInput()
		{


		}

		void SetupLeaderboard()
		{
			List<PlayerGameData> playersRanked = GameNetworkManager.Instance.GetPlayerDataRanked();

			foreach(var entry in _leaderboardEntries)
			{
				entry.gameObject.SetActive(false);
			}

			PlayerGameData winner = playersRanked[0];
			string winnerText = winner.DisplayName + " WINS!!";
			string scoreText = string.Format("{0:C}", winner.Score);
			_winnerNameText.text = winnerText;
			_winnerScoreText.text = scoreText;

			for(int runnerUpIndex = 1; runnerUpIndex < playersRanked.Count; ++runnerUpIndex)
			{
				PlayerGameData runnerUp = playersRanked[runnerUpIndex];

				int objectIndex = runnerUpIndex - 1;
				_leaderboardEntries[objectIndex].gameObject.SetActive(true);
				_leaderboardEntries[objectIndex].SetUpForPlayer(runnerUpIndex + 1, runnerUp.DisplayName, runnerUp.Score);
			}
		}

		public void OnGameStateChanged(EGameState newState)
		{
			if(newState == EGameState.PostMatch)
			{
				SetupLeaderboard();
			}

			RefreshActiveObjectsForState(newState);
		}
	}
}