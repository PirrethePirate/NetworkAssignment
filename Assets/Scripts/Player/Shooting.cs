using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Shooting : NetworkBehaviour
{
    private ScoreManager scoreManager;
    private MouseCursorController controller;
    private bool canShoot = false;

    [Header("Ammunition")]
    [SerializeField] private int maxBullets = 6;
    [SerializeField] private int currentBullets = 6;
    [SerializeField] private TextMeshProUGUI bulletsUI;

    [Header("Reloading")]
    private bool currentlyReloading;
    [SerializeField] private float reloadTime = 2;

    [Header("Shooting Effect")]
    public GameObject shootingEffect;

    [Header("Interaction Settings")]
    [SerializeField] private float raycastDistance = 10f;
    [SerializeField] private LayerMask targetLayer;
    private int playerNumber;

    private void Start()
    {
        controller = GetComponent<MouseCursorController>();
        scoreManager = FindAnyObjectByType<ScoreManager>();
        currentBullets = maxBullets;
        HideGunUI();
    }

    private void Update()
    {
        if (IsOwner && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (IsOwner && Input.GetMouseButtonDown(1))
        {
            StartReload();
        }
        if(canShoot)
        BulletsUI();
    }


    private void Shoot()
    {
        if (currentBullets > 0 && currentlyReloading == false && canShoot)
        {
            currentBullets -= 1;
            SpawnShootingEffectServerRpc(transform.position, transform.rotation);
            DidWeHitATarget();

            if (currentBullets == 0)
            {
                controller.ChangeCrosshair(2);
            }
        }
        else if (currentlyReloading == true)
        {
            Debug.Log("Reloading!");
        }
        else
        {
            Debug.Log("Clip is empty");
        }
    }

    private void DidWeHitATarget()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, raycastDistance, targetLayer);

        if (hit.collider != null)
        {
            TargetHit target = hit.collider.gameObject.GetComponent<TargetHit>();
            if (target != null && IsOwner)
            {
                target.OnTargetClickedServerRpc();
                scoreManager.AwardPointServerRpc(playerNumber);
            }
        }

    }

    private void StartReload()
    {
        if (currentlyReloading == false)
        {
            currentlyReloading = true;
            controller.ChangeCrosshair(3);
            Invoke(nameof(RefillAmmo), reloadTime);
        }
    }

    private void RefillAmmo()
    {
        currentBullets = maxBullets;
        controller.ChangeCrosshair(1);
        currentlyReloading = false;
    }

    private void BulletsUI()
    {
        if(IsOwner)
        controller.ChangeAmmoCounter(currentBullets, maxBullets);
    }

    [ServerRpc]
    private void SpawnShootingEffectServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject effectInstance = Instantiate(shootingEffect, position, rotation);
        NetworkObject networkObject = effectInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    }

    public void PlayerNumber(int value)
    {
        playerNumber = value + 1;
    }

    public void GameHasStarted()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        controller.ShowCrosshair();
        ShowGunUI();
        RefillAmmo();
        canShoot = true;
    }

    public void GameHasEnded()
    {
        GameHasEndedClientRpc();
    }

    [ClientRpc]
    private void GameHasEndedClientRpc()
    {
        HideGunUI();
        canShoot = false;
    }


    private void HideGunUI()
    {
        controller.HideCrosshair();
        bulletsUI.enabled = false;
    }

    private void ShowGunUI()
    {
        controller.ShowCrosshair();
        //if (IsLocalPlayer)
        //bulletsUI.enabled = true;
    }
}
