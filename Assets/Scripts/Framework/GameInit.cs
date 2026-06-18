using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JJ26.UI;

namespace JJ26.Framework
{
    public class GameInit : MonoBehaviour
    {
        [SerializeField] List<BaseGameSystem> _systemPrefabs;

        private List<BaseGameSystem> _systems = new();

		private void Start()
		{
            DontDestroyOnLoad(this);
            InitGame();
        }

		void InitGame()
		{
            InstantiateSystems();
            InitialiseSystems();
            SetSystemCallbacks();

            var uiStateSystem = FindAnyObjectByType(typeof(UIStateSystem)) as UIStateSystem;
            uiStateSystem.EnterScreen(UIStateSystem.EUIState.PressStart);
        }

        void InstantiateSystems()
		{
            _systems.Clear();

            foreach(BaseGameSystem prefab in _systemPrefabs)
			{
                BaseGameSystem system = Instantiate(prefab);
                system.transform.parent = transform;
                system.transform.position = Vector3.zero;
                _systems.Add(system);
            }
		}

        void InitialiseSystems()
		{
            foreach (BaseGameSystem system in _systems)
            {
                system.Initialise();
            }
		}

        void SetSystemCallbacks()
        {
            foreach (BaseGameSystem system in _systems)
            {
                system.SetCallbacks();
            }
        }

        void Update()
		{
            foreach(BaseGameSystem system in _systems)
			{
                system.UpdateSystem();
			}
		}

        void LateUpdate()
        {
            foreach (BaseGameSystem system in _systems)
            {
                system.LateUpdateSystem();
            }
        }

		private void FixedUpdate()
		{
            foreach (BaseGameSystem system in _systems)
            {
                system.FixedUpdateSystem();
            }
        }
	}
}


