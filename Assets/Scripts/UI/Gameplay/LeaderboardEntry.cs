using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JJ26.UI
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] TMP_Text _positionText;
        [SerializeField] TMP_Text _playerNameText;

        public void SetUpForPlayer(int position, string name, float score)
		{
            SetUpPosition(position);
            SetUpName(name, score);
		}

        void SetUpName(string name, float score)
		{
            var roundedScore = string.Format("{0:C}", score);
            _playerNameText.text = name + ": " + roundedScore;
        }

        void SetUpPosition(int position)
		{
            switch(position)
			{
                case 2:
                    _positionText.text = "2nd";
                    break;
                case 3:
                    _positionText.text = "3rd";
                    break;
                case 4:
                    _positionText.text = "4th";
                    break;
			}
		}
    }
}