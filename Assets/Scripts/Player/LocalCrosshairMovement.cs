using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalCrosshairMovement : MonoBehaviour
{
    [Header("References")]
    private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI bulletsUI;
    [SerializeField] private Image emoteImage;

    [Header("Sprites")]
    public Sprite normalCrosshair;
    public Sprite crossedCrosshair;
    public Sprite reloadingCrosshair;
    private SpriteRenderer spriteRenderer;

    [Header("Emote Images")]
    public Sprite transparent;
    public Sprite emote1;
    public Sprite emote2;
    public Sprite emote3;

    private Sprite[] emotes;

    private static readonly Color[] PlayerColors = { Color.blue, Color.green, Color.magenta, Color.yellow };

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        emotes = new Sprite[] { transparent, emote1, emote2, emote3 };
        emoteImage.sprite = transparent;
        StartTutorialText();
    }

    void Update()
    {
        if (mainCamera != null)
        {
            Vector2 newPosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            transform.position = newPosition;
        }
    }

    public void ChangeCrosshair(int version)
    {
        switch (version)
        {
            case 1:
                spriteRenderer.sprite = normalCrosshair;
                break;
            case 2:
                spriteRenderer.sprite = crossedCrosshair;
                break;
            case 3:
                spriteRenderer.sprite = reloadingCrosshair;
                break;
        }
    }

    private void StartTutorialText()
    {
        bulletsUI.text = "Left Mouse Button to Fire\nRight Mouse Button to Reload\n1, 2 and 3 to use emotes";
    }
    public void ChangeAmmoCounter(int currentAmmo, int maxAmmo)
    {
        bulletsUI.text = currentAmmo + "/" + maxAmmo;
    }

    public void ChangeEmote(int value)
    {
        if (value >= 0 && value < emotes.Length)
        {
            emoteImage.sprite = emotes[value];
        }
        else
        {
            emoteImage.sprite = transparent;
        }
    }

    public void ChangeColor(int value)
    {
        spriteRenderer.color = PlayerColors[value];
    }
}
