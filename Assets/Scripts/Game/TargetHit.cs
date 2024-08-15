using Unity.Netcode;

public class TargetHit : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void OnTargetClickedServerRpc()
    {
        if (IsServer)
        {
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn(true);
            }
        }
    }
}
