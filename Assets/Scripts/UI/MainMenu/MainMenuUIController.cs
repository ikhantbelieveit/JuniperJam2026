using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JJ26.UI
{
	public class MainMenuUIController : UIController
	{
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] Button _continueButton;

		private string _displayName;

		private const string _playerPrefsNameKey = "PlayerName";

		private MainMenuUIController _mainMenuUISystem;

		#region UIController

		public override void Initialise()
		{
			base.Initialise();

			_mainMenuUISystem = FindAnyObjectByType(typeof(MainMenuUIController)) as MainMenuUIController;

			InitialiseInputField();
			RefreshConfirmActive();
		}

		private void InitialiseInputField()
		{
			if(!PlayerPrefs.HasKey(_playerPrefsNameKey)) { return; }
			_inputField.text = PlayerPrefs.GetString(_playerPrefsNameKey);
		}

		public void SavePlayerName()
		{
			Debug.Log("Saving player name " + _inputField.text);

			_displayName = _inputField.text;
			PlayerPrefs.SetString(_playerPrefsNameKey, _displayName);
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

		private void RefreshConfirmActive()
		{
			string name = _inputField.text;
			_continueButton.interactable = !string.IsNullOrEmpty(name);
		}

		#endregion //UIController

		public void UpdateInput()
		{

		}

		#region InputSignals

		public void Input_OnInputValueChanged(string newValue)
		{
			RefreshConfirmActive();
		}

		public void Input_OnConfirmPressed()
		{
			if(string.IsNullOrEmpty(_inputField.text)) { return; }
			SavePlayerName();
		}

		#endregion //InputSignals
	}
}

