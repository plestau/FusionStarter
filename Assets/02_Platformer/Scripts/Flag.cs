using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace Starter.Platformer
{
    public class Flag : NetworkBehaviour
    {
        public UnityEvent<Player> FlagReached;
        public bool esBanderaFinal = false;

        private void OnTriggerEnter(Collider other)
        {
            if (HasStateAuthority == false)
                return;

            var player = other.transform.parent != null ? other.transform.parent.GetComponent<Player>() : null;
            if (player != null)
            {
                RPC_HandleFlagReached(player, esBanderaFinal);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_HandleFlagReached(Player player, bool isFinalFlag)
        {
            if (isFinalFlag)
            {
                FlagReached?.Invoke(player);
            }
            else
            {
                player.SetLastFlagPosition(transform.position);
            }
        }
    }
}