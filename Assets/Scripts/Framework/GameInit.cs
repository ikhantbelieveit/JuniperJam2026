using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JJ26.Framework
{
    public class GameInit : MonoBehaviour
    {
        [SerializeField] List<BaseGameSystem> _systemPrefabs;

        private List<BaseGameSystem> _systems = new();

		private void Start()
		{
            InitGame();
        }

		void InitGame()
		{
            InstantiateSystems();
            InitialiseSystems();
            SetSystemCallbacks();
		}

        void InstantiateSystems()
		{
            _systems.Clear();

            foreach(BaseGameSystem prefab in _systemPrefabs)
			{
                BaseGameSystem system = Instantiate(prefab);
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


