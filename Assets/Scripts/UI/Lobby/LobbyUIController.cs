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

		public Button StartGameButton => _startGameButton;

		[SerializeField] GameObject _buttonRootGO;

		[SerializeField] List<TMP_Text> _playerNameTexts;
		[SerializeField] List<TMP_Text> _playerReadyTexts;
		[SerializeField] List<TMP_Text> _playerNotReadyTexts;

		private MainMenuUISystem _mainMenuUISystem;
		private InputSystem _inputSystem;
		private UIStateSystem _uiStateSystem;
		private GameNetworkManager _networkManager;

		#region UIController

		public override void Initialise()
		{
			base.Initialise();

			_mainMenuUISystem = FindAnyObjectByType(typeof(MainMenuUISystem)) as MainMenuUISystem;
			_inputSystem = FindAnyObjectByType(typeof(InputSystem)) as InputSystem;
			_uiStateSystem = FindAnyObjectByType(typeof(UIStateSystem)) as UIStateSystem;
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

		public void RefreshDisplay()
		{
			for (int textIndex = 0; textIndex < _playerNameTexts.Count; textIndex++)
			{
				_playerNameTexts[textIndex].text = "Waiting for player...";
				_playerReadyTexts[textIndex].gameObject.SetActive(false);
				_playerNotReadyTexts[textIndex].gameObject.SetActive(false);
			}

			for(int playerIndex = 0; playerIndex < _networkManager.RoomPlayers.Count; playerIndex++)
			{
				var player = _networkManager.RoomPlayers[playerIndex];
				_playerNameTexts[playerIndex].text = player.DisplayName;
				_playerReadyTexts[playerIndex].gameObject.SetActive(player.IsReady);
				_playerNotReadyTexts[playerIndex].gameObject.SetActive(!player.IsReady);

				Debug.Log("Setting UI of player " + playerIndex + " to isReady = " + player.IsReady.ToString());
			}
		}

		#endregion //UIController

		public void UpdateInput()
		{
			if(_inputSystem.UICancelPressed)
			{
				Input_UICancelPressed();
			}
		}

		#region InputSignals

		public void Input_UICancelPressed()
		{
			if(_networkManager.IsHosting())
			{
				//TODO: disconnect host and boot clients back to main menu
				return;
			}

			_networkManager.StopClient();
			_uiStateSystem.EnterScreen(UIStateSystem.EUIState.MainMenu);
		}

		public void Input_ReadyButtonPressed()
		{
			PlayerRoomData playerData = _networkManager.GetLocalPlayerData();
			if(playerData)
			{
				playerData.CmdSetReady(!playerData.IsReady);
			}
		}

		public void Input_StartGameButtonPressed()
		{
			_networkManager.GetLocalPlayerData().CmdStartGame();
		}

		#endregion //InputSignals

		public void OnPlayerLeaderStatusSet()
		{

		}
	}
}

