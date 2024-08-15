using UnityEngine;
using Unity.Netcode;

public class MouseCursorController : NetworkBehaviour
{
    private NetworkVariable<Vector2> mousePosition = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Owner);

    [Header("Sprites")]
    public Sprite normalCrosshair;
    public Sprite crossedCrosshair;
    public Sprite reloadingCrosshair;
    private SpriteRenderer spriteRenderer;

    [Header("Local Crosshair")]
    public GameObject localCrosshairPrefab;
    private GameObject localCrosshairInstance;
    private LocalCrosshairMovement localCrosshair;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (IsOwner)
        {
            CreateLocalCrosshair();
            gameObject.SetActive(true);
            spriteRenderer.enabled = false;
        }
        else
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            Vector2 newPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            mousePosition.Value = newPosition;

            MoveRPC(newPosition);
            transform.position = newPosition;
        }
        else
        {
            transform.position = mousePosition.Value;
        }
    }

    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector2 data)
    {
        transform.position = data;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
        }
    }



    public void HideCrosshair()
    {
        //spriteRenderer.enabled = false;
    }

    public void ShowCrosshair()
    {
        //spriteRenderer.enabled = true;
    }


    #region Local Crosshair Functions

    private void CreateLocalCrosshair()
    {
        if (localCrosshairPrefab != null)
        {
            localCrosshairInstance = Instantiate(localCrosshairPrefab);
            localCrosshairInstance.transform.position = transform.position;

            localCrosshair = localCrosshairInstance.GetComponent<LocalCrosshairMovement>();
        }
        else
        {
            Debug.LogError("Local Crosshair Prefab is not assigned in the Inspector.");
        }
    }

    public void ChangeCrosshair(int version)
    {
        localCrosshair.ChangeCrosshair(version);
    }

    public void ChangeAmmoCounter(int currentAmmo, int maxAmmo)
    {
        localCrosshair.ChangeAmmoCounter(currentAmmo, maxAmmo);
    }

    public void ChangeEmote(int value)
    {
        localCrosshair.ChangeEmote(value);
    }

    public void ChangeColor(int index)
    {
        localCrosshair.ChangeColor(index);
    }

    #endregion
}
