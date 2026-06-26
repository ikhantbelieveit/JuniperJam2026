using JJ26.Input;
using JJ26.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace JJ26.UI
{
	public class LobbyUIController : UIController
	{
		[SerializeField] Button _readyUpButton;
		[SerializeField] Button _startGameButton;
		[SerializeField] Button _singlePlayerStartButton;

		public Button StartGameButton => _startGameButton;

		[SerializeField] GameObject _buttonRootGO;

		[SerializeField] List<TMP_Text> _playerNameTexts;
		[SerializeField] List<TMP_Text> _playerReadyTexts;
		[SerializeField] List<TMP_Text> _playerNotReadyTexts;

		#region UIController

		public override void Initialise()
		{
			base.Initialise();

		}

		public override void SetActive(bool active)
		{
			base.SetActive(active);

			RefreshDisplay();
			bool gameReady = GameNetworkManager.Instance.IsGameReady();
			_startGameButton.gameObject.SetActive(gameReady);
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

		public void RefreshDisplay()
		{
			for (int textIndex = 0; textIndex < _playerNameTexts.Count; textIndex++)
			{
				_playerNameTexts[textIndex].text = "Waiting for player...";
				_playerReadyTexts[textIndex].gameObject.SetActive(false);
				_playerNotReadyTexts[textIndex].gameObject.SetActive(false);
			}

			for(int playerIndex = 0; playerIndex < GameNetworkManager.Instance.LobbyPlayers.Count; playerIndex++)
			{
				var player = GameNetworkManager.Instance.LobbyPlayers[playerIndex];
				_playerNameTexts[playerIndex].text = player.DisplayName;
				_playerReadyTexts[playerIndex].gameObject.SetActive(player.IsReady);
				_playerNotReadyTexts[playerIndex].gameObject.SetActive(!player.IsReady);
			}

			_singlePlayerStartButton.gameObject.SetActive(GameNetworkManager.Instance.LobbyPlayers.Count == 1);
		}

		#endregion //UIController

		public void UpdateInput()
		{
			if(InputSystem.UICancelPressed)
			{
				Input_UICancelPressed();
			}
		}

		public void SetStartButtonActive(bool active)
		{
			_startGameButton.gameObject.SetActive(active);
		}

		#region InputSignals

		public void Input_UICancelPressed()
		{
			if(GameNetworkManager.Instance.IsHostingInLobby())
			{
				GameNetworkManager.Instance.StopHost();
				UIStateSystem.EnterScreen(UIStateSystem.EUIState.MainMenu);
				return;
			}

			GameNetworkManager.Instance.StopClient();
			UIStateSystem.EnterScreen(UIStateSystem.EUIState.MainMenu);
		}

		public void Input_ReadyButtonPressed()
		{
			PlayerLobbyData playerData = GameNetworkManager.Instance.GetLocalPlayerData();
			if(playerData)
			{
				playerData.CmdSetReady(!playerData.IsReady);
			}
		}

		public void Input_StartGameButtonPressed()
		{
			GameNetworkManager.Instance.GetLocalPlayerData().CmdStartGame();
		}

		public void Input_DebugStartButtonPressed()
		{
			GameNetworkManager.Instance.GetLocalPlayerData().CmdForceStartGame();
		}

		#endregion //InputSignals
	}
}

