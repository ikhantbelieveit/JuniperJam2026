using UnityEngine;
using Mirror;

namespace JJ26.Network
{
    public enum EGameState
	{
        None,
        Countdown,
        Gameplay,
        PostMatch
	}

    public class GameStateData : NetworkBehaviour
    {
        [SerializeField] float _matchDuration = 60f;
        [SerializeField] float _countdownDuration = 5f;
        [SerializeField] float _postMatchDuration = 10f;

        [SyncVar] public float MatchTimeRemaining;
        [SyncVar] public float CountdownTimeRemaining;
        [SyncVar] public float PostMatchTimeRemaining;

        [SyncVar] public EGameState CurrentState;

        public delegate void GameStateEvent(EGameState state);
        public event GameStateEvent OnGameStateChanged;

		public override void OnStartClient()
		{
			base.OnStartClient();

            if(null == GameNetworkManager.Instance.GameState)
			{
                GameNetworkManager.Instance.SetGameState(this);
            }
		}

		[Server]
        void BeginCountdown()
		{
            CountdownTimeRemaining = _countdownDuration;
		}

        [Server]
        void BeginMatchTime()
		{
            MatchTimeRemaining = _matchDuration;
		}

        [Server]
        void BeginPostMatch()
		{
            PostMatchTimeRemaining = _postMatchDuration;
		}


        [Server]
        public void SetGameState(EGameState newState)
		{
            if(CurrentState == newState) { return; }
            CurrentState = newState;
            switch(CurrentState)
			{
                case EGameState.Countdown:
                    Debug.Log("Set state to Countdown");
                    BeginCountdown();
                    break;
                case EGameState.Gameplay:
                    Debug.Log("Set state to Gameplay");
                    BeginMatchTime();
                    break;
                case EGameState.PostMatch:
                    Debug.Log("Set state to PostMatch");
                    BeginPostMatch();
                    break;
			}
            OnGameStateChanged?.Invoke(CurrentState);
		}

        [Server]
		public void Update()
		{
			switch(CurrentState)
			{
                case EGameState.Countdown:
                    UpdateCountdown();
                    break;
                case EGameState.Gameplay:
                    UpdateMatchTime();
                    break;
                case EGameState.PostMatch:
                    UpdatePostMatch();
                    break;
			}
		}

        [Server]
        void UpdateCountdown()
		{
            if(CountdownTimeRemaining <= 0) { return; }
            CountdownTimeRemaining -= Time.deltaTime;
            if(CountdownTimeRemaining <= 0)
			{
                SetGameState(EGameState.Gameplay);
			}
		}

        [Server]
        void UpdateMatchTime()
		{
            if (MatchTimeRemaining <= 0) { return; }
            MatchTimeRemaining -= Time.deltaTime;
            if (MatchTimeRemaining <= 0)
            {
                SetGameState(EGameState.PostMatch);
            }
        }

        [Server]
        void UpdatePostMatch()
		{
            if (PostMatchTimeRemaining <= 0) { return; }
            PostMatchTimeRemaining -= Time.deltaTime;
            if (PostMatchTimeRemaining <= 0)
            {
                GameNetworkManager.Instance.ExitGame();
            }
        }
	}
}