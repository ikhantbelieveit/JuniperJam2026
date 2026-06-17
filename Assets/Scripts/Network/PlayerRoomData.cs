using UnityEngine;
using Mirror;

namespace JJ26.Network
{
    public class PlayerRoomData : NetworkBehaviour
    {
        public bool IsLeader = false;
        public bool IsReady = false;

        public void UpdateGameReady(bool isReady)
		{

		}
    }
}
