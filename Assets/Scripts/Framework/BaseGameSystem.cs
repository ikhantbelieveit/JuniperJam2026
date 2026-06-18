using UnityEngine;
using Mirror;

namespace JJ26.Framework
{
	public class BaseGameSystem : MonoBehaviour
	{
		protected bool _isInitialised = false;

		public virtual void Awake()
		{
			DontDestroyOnLoad(this);
		}

		public virtual void Initialise()
		{
			if (_isInitialised)
			{
				return;
			}

			_isInitialised = true;
		}

		public virtual void SetCallbacks()
		{

		}

		public virtual void UpdateSystem()
		{

		}

		public virtual void LateUpdateSystem()
		{

		}

		public virtual void FixedUpdateSystem()
		{

		}

		public virtual void ResetSystem()
		{

		}
	}
}