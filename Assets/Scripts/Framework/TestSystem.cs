using UnityEngine;

namespace JJ26.Framework
{
    public class TestSystem : BaseGameSystem
    {
		public override void UpdateSystem()
		{
			base.UpdateSystem();

			Debug.Log("[TEST] update test system at time " + Time.time.ToString());
		}
	}
}

