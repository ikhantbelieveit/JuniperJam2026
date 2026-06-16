using UnityEngine;

namespace JJ26.Framework
{
	public class BaseGameSystem : MonoBehaviour
	{
		protected static GameObject _instance;
		private bool _isInitialised = false;

		private static bool applicationInQuitting = false;

		public virtual void Awake()
		{
			if (_instance != null && _instance != this.gameObject)
			{
				Destroy(this.gameObject);
				return;
			}

			applicationInQuitting = false;

			DontDestroyOnLoad(this);

			_instance = this.gameObject;
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

		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when the application quits.
		/// If any script calls Instance after it's been destroyed,
		///		it creates a buggy ghost object that will stay on the Editor scene
		///		even after stopping playing the Application. Really bad!
		///	So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>

		public virtual void OnDestroy()
		{
			if (_instance == this.gameObject)
			{
				return;
			}

			applicationInQuitting = true;
		}

		public void OnEnable()
		{
			applicationInQuitting = false;
		}
	}
}