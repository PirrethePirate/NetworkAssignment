using UnityEngine;
using Unity.Netcode;

public class ShootingEffect : NetworkBehaviour
{
    [SerializeField] private float duration = 0.3f;

    private void Start()
    {
        Invoke(nameof(RequestDestroy), duration);
    }

    private void RequestDestroy()
    {
        if (IsServer)
        {
            DestroyObjectClientRpc();
        }
        else
        {
            DestroyObjectServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        DestroyObjectClientRpc();
    }

    [ClientRpc]
    private void DestroyObjectClientRpc()
    {
        if (IsServer)
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
    }
}
