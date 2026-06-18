using JJ26.Input;
using JJ26.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JJ26.UI
{
	public class MainMenuUIController : UIController
	{
		[SerializeField] private TMP_InputField _nameInputField;
		[SerializeField] private TMP_InputField _ipInputField;

		[SerializeField] Button _confirmNameButton;
		[SerializeField] Button _confirmIPButton;

		[SerializeField] GameObject _nameInputPanelGO;
		[SerializeField] GameObject _ipInputPanelGO;
		[SerializeField] GameObject _buttonRootGO;

		private string _displayName;
		public string DisplayName => _displayName;

		private const string _playerPrefsNameKey = "PlayerName";

		private GameNetworkManager _networkManager;


		#region UIController

		public override void Initialise()
		{
			base.Initialise();

			_networkManager = FindAnyObjectByType(typeof(GameNetworkManager)) as GameNetworkManager;

			_networkManager.OnClientConnected -= HandleClientConnected;
			_networkManager.OnClientDisconnected -= HandleClientDisconnected;

			_networkManager.OnClientConnected += HandleClientConnected;
			_networkManager.OnClientDisconnected += HandleClientDisconnected;


			InitialiseInputField();
			RefreshConfirmNameActive();
			RefreshConfirmIPActive();
		}

		private void InitialiseInputField()
		{
			if(!PlayerPrefs.HasKey(_playerPrefsNameKey)) { return; }
			_nameInputField.text = PlayerPrefs.GetString(_playerPrefsNameKey);
		}

		public void SavePlayerName()
		{
			Debug.Log("Saving player name " + _nameInputField.text);

			_displayName = _nameInputField.text;
			PlayerPrefs.SetString(_playerPrefsNameKey, _displayName);
		}

		public override void SetActive(bool active)
		{
			base.SetActive(active);
			_nameInputPanelGO.SetActive(true);
			_buttonRootGO.SetActive(false);
			_ipInputPanelGO.SetActive(false);
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

		private void RefreshConfirmNameActive()
		{
			string name = _nameInputField.text;
			_confirmNameButton.interactable = !string.IsNullOrEmpty(name);
		}

		private void RefreshConfirmIPActive()
		{
			string ip = _ipInputField.text;
			_confirmIPButton.interactable = !string.IsNullOrEmpty(ip);
		}

		#endregion //UIController

		public void UpdateInput()
		{
			if(InputSystem.UICancelPressed)
			{
				Input_UICancelPressed();
			}
		}

		#region InputSignals

		public void Input_OnInputValueChanged(string newValue)
		{
			RefreshConfirmNameActive();
		}

		public void Input_OnConfirmNamePressed()
		{
			if(string.IsNullOrEmpty(_nameInputField.text)) { return; }
			SavePlayerName();
			_nameInputPanelGO.SetActive(false);
			_buttonRootGO.SetActive(true);
		}

		public void Input_OnConfirmIPPressed()
		{
			string ipText = _ipInputField.text;
			if (string.IsNullOrEmpty(ipText)) { return; }

			_confirmIPButton.interactable = false;
			_networkManager.networkAddress = ipText;
			_networkManager.StartClient();
		}


		public void Input_HostLobbyPressed()
		{
			_networkManager.StartHost();
			_buttonRootGO.SetActive(false);
			//go to lobby screen
			Debug.Log("go to lobby screen");
			UIStateSystem.EnterScreen(UIStateSystem.EUIState.Lobby);
		}

		public void Input_JoinLobbyPressed()
		{
			_ipInputPanelGO.SetActive(true);
		}

		public void Input_UICancelPressed()
		{
			if (_ipInputPanelGO.activeSelf)
			{
				_ipInputPanelGO.SetActive(false);
				_buttonRootGO.SetActive(true);
				return;
			}

			if (_buttonRootGO.activeSelf)
			{
				_buttonRootGO.SetActive(false);
				_nameInputPanelGO.SetActive(true);
				return;
			}

			if (_nameInputPanelGO.activeSelf)
			{
				UIStateSystem.EnterScreen(UIStateSystem.EUIState.PressStart);
				return;
			}
		}

		#endregion //InputSignals

		#region Network

		public void HandleClientConnected()
		{
			_confirmIPButton.interactable = true;
			Debug.Log("Client connected!");
			UIStateSystem.EnterScreen(UIStateSystem.EUIState.Lobby);
		}

		public void HandleClientDisconnected()
		{
			_confirmIPButton.interactable = true;
			Debug.Log("Client disconnected!");
		}

		#endregion //Network
	}
}

