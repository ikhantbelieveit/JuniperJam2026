using UnityEngine;
using JJ26.Network;
using TMPro;

namespace JJ26.UI
{
    public class PlayerScoreDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text _nameText;
        [SerializeField] TMP_Text _scoreText;

        public void Initialise(PlayerGameData data)
		{
            _nameText.text = data.DisplayName;
		}
    }
}