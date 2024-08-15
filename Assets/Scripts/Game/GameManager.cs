using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    private ScoreManager scoreManager;
    private AssignColors assignColors;

    [Header("Script References")]
    [SerializeField] private SpawnTarget spawntarget;
    [SerializeField] private GameObject startButton;

    [Header("Game Timer")]
    [SerializeField] private float gameDuration = 60;

    private void Start()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        ShowStartButtonClientRpc();
    }

    public void StartGame()
    {
        Invoke(nameof(StopGame), gameDuration);
        HideCursorClientRpc();
        scoreManager.ResetPointsServerRpc();
        StartShootingServerRpc();
        HideStartButtonClientRpc();
        spawntarget.StartGame();

        // Set colors
        AssignColorsToAllPlayersServerRpc();
    }

    private void StopGame()
    {
        spawntarget.StopGame();

        if (IsServer)
        {
            ShowStartButtonClientRpc();
        }
        ShowCursor();
        StopShootingServerRpc();
        DespawnTargetsServerRpc();
        scoreManager.ShowScoreboardServerRpc();
    }


    [ServerRpc]
    private void AssignColorsToAllPlayersServerRpc()
    {
        var playerObjects = NetworkManager.Singleton.ConnectedClientsList;

        foreach (var client in playerObjects)
        {
            var playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                var assignColors = playerObject.GetComponent<AssignColors>();
                if (assignColors != null)
                {
                    assignColors.AssignColorServerRpc(client.ClientId);
                }
            }
        }
    }


    [ServerRpc]
    public void StopShootingServerRpc()
    {
        if (IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var playerObject = client.PlayerObject;
                if (playerObject != null)
                {
                    var shootingScript = playerObject.GetComponent<Shooting>();
                    if (shootingScript != null)
                    {
                        shootingScript.GameHasEnded();
                    }
                }
            }
        }
    }

    [ServerRpc]
    public void StartShootingServerRpc()
    {
        if (IsServer)
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                var playerObject = client.PlayerObject;
                if (playerObject != null)
                {
                    var shootingScript = playerObject.GetComponent<Shooting>();
                    if (shootingScript != null)
                    {
                        shootingScript.GameHasStarted();
                    }
                }
            }
        }
    }

    [ServerRpc]
    public void DespawnTargetsServerRpc()
    {
        TargetHit[] allTargets = FindObjectsOfType<TargetHit>();

        foreach (TargetHit target in allTargets)
        {
            target.OnTargetClickedServerRpc();
        }
    }
    [ClientRpc]
    private void HideCursorClientRpc()
    {
        Cursor.visible = false;
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
    }

    [ClientRpc]
    private void HideStartButtonClientRpc()
    {
        startButton.SetActive(false);
    }

    [ClientRpc]
    private void ShowStartButtonClientRpc()
    {
        if (IsServer)
        startButton.SetActive(true);
    }
}
