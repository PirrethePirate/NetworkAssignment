using UnityEngine;
using Unity.Netcode;

public class AssignColors : NetworkBehaviour
{
    private static readonly Color[] PlayerColors = { Color.blue, Color.green, Color.magenta, Color.yellow };

    [Header("References")]
    private MouseCursorController controller;
    private SpriteRenderer spriteRenderer;
    private Shooting shooting;
    private ScoreManager scoreManager;

    private void Start()
    {
        controller = GetComponent<MouseCursorController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shooting = GetComponent<Shooting>();
        scoreManager = FindObjectOfType<ScoreManager>();

        if (IsOwner)
        {
            HowManyPlayersServerRpc();
        }
    }

    [ClientRpc]
    public void AssignColorsAtStartClientRpc()
    {
        if (IsOwner)
        {
            AssignColorServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void AssignColorServerRpc(ulong clientId)
    {
        int playerIndex = (int)clientId % PlayerColors.Length;

        AssignColorClientRpc(playerIndex);
    }

    [ClientRpc]
    private void AssignColorClientRpc(int playerIndex)
    {
        if (spriteRenderer != null)
        {

            spriteRenderer.color = PlayerColors[playerIndex];

            shooting.PlayerNumber(playerIndex);
            controller.ChangeColor(playerIndex);
        }
    }

    [ServerRpc]
    private void HowManyPlayersServerRpc()
    {
        scoreManager.AddPlayersServerRpc();
    }


}
