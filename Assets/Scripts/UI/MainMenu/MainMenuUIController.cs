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
		private const string _playerPrefsIPKey = "PlayerIP";


		#region UIController

		public override void Initialise()
		{
			base.Initialise();

			GameNetworkManager.Instance.OnClientConnected -= HandleClientConnected;
			GameNetworkManager.Instance.OnClientDisconnected -= HandleClientDisconnected;

			GameNetworkManager.Instance.OnClientConnected += HandleClientConnected;
			GameNetworkManager.Instance.OnClientDisconnected += HandleClientDisconnected;


			InitialiseInputFields();
			RefreshConfirmNameActive();
			RefreshConfirmIPActive();
		}

		public override void SetActive(bool active)
		{
			base.SetActive(active);

			if(active)
			{
				GameNetworkManager.Instance.OnClientConnected -= HandleClientConnected;
				GameNetworkManager.Instance.OnClientDisconnected -= HandleClientDisconnected;

				GameNetworkManager.Instance.OnClientConnected += HandleClientConnected;
				GameNetworkManager.Instance.OnClientDisconnected += HandleClientDisconnected;
			}

			_nameInputPanelGO.SetActive(true);
			_buttonRootGO.SetActive(false);
			_ipInputPanelGO.SetActive(false);
		}

		private void InitialiseInputFields()
		{
			if(PlayerPrefs.HasKey(_playerPrefsNameKey))
			{
				_nameInputField.text = PlayerPrefs.GetString(_playerPrefsNameKey);
			}
			
			if(PlayerPrefs.HasKey(_playerPrefsIPKey))
			{
				_ipInputField.text = PlayerPrefs.GetString(_playerPrefsIPKey);
			}
		}

		public void SavePlayerName()
		{
			Debug.Log("Saving player name " + _nameInputField.text);

			_displayName = _nameInputField.text;
			PlayerPrefs.SetString(_playerPrefsNameKey, _displayName);
		}

		public void SaveEnteredIP()
		{
			Debug.Log("Saving IP address " + _ipInputField.text);

			PlayerPrefs.SetString(_playerPrefsIPKey, _ipInputField.text);
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
			SaveEnteredIP();
			_confirmIPButton.interactable = false;
			GameNetworkManager.Instance.networkAddress = ipText;
			GameNetworkManager.Instance.StartClient();
		}


		public void Input_HostLobbyPressed()
		{
			GameNetworkManager.Instance.StartHost();
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

