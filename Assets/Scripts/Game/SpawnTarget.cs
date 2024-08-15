using UnityEngine;
using Unity.Netcode;

public class SpawnTarget : NetworkBehaviour
{
    [Header("Spawn Timer")]
    [SerializeField] private float staticSpawnTimer = 2;
    [SerializeField] private float randomValueMin = 0.1f;
    [SerializeField] private float randomValueMax = 0.9f;
    private float spawnTimer;
    private bool gameStarted = false;

    [Header("Target Prefab")]
    [SerializeField] private GameObject target;

    void Update()
    {
        if (IsServer && gameStarted == true)
        {
            if (spawnTimer <= 0)
            {
                SpawnATargetServerRpc();
                StartTimer();
            }
            else
            {
                CountingDown();
            }
        }
    }


    public void StartGame()
    {
        if (IsServer)
        {
            StartTimer();
        }
    }

    public void StopGame()
    {
        gameStarted = false;
    }

    private void StartTimer()
    {
        gameStarted = true;
        spawnTimer = staticSpawnTimer + Random.Range(randomValueMin, randomValueMax);
    }

    private void CountingDown()
    {
        spawnTimer -= Time.deltaTime;
    }

    [ServerRpc]
    private void SpawnATargetServerRpc()
    {
        Camera mainCamera = Camera.main;
        Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        float randomX = Random.Range(-screenBounds.x + 1, screenBounds.x - 1);
        float randomY = Random.Range(-screenBounds.y + 1, screenBounds.y - 1);

        Vector3 spawnPosition = new Vector3(randomX, randomY, 0.5f);
        GameObject targetInstance = Instantiate(target, spawnPosition, Quaternion.identity);

        targetInstance.GetComponent<NetworkObject>().Spawn();
    }
}
