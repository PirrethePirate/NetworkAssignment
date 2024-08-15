using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Emotes : NetworkBehaviour
{
    [SerializeField] private MouseCursorController controller;

    [Header("Emote Images")]
    public Image emoteImage;
    public Sprite transparent;
    public Sprite emote1;
    public Sprite emote2;
    public Sprite emote3;

    private Sprite[] emotes;

    [Header("Remove Icon Delay")]
    [SerializeField] private float timer;

    private void Start()
    {
        emotes = new Sprite[] { transparent, emote1, emote2, emote3 };

        if (IsOwner)
        {
            emoteImage.enabled = false;
        }

    }

    private void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetEmoteOnAllClientsServerRpc(1);
                controller.ChangeEmote(1);
                CancelAndStartInvoke();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetEmoteOnAllClientsServerRpc(2);
                controller.ChangeEmote(2);
                CancelAndStartInvoke();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetEmoteOnAllClientsServerRpc(3);
                controller.ChangeEmote(3);
                CancelAndStartInvoke();
            }
        }
    }

    [ServerRpc]
    private void SetEmoteOnAllClientsServerRpc(int emoteIndex)
    {
        SetEmoteClientRpc(emoteIndex);
    }

    [ClientRpc]
    private void SetEmoteClientRpc(int emoteIndex)
    {
        if (emoteIndex >= 0 && emoteIndex < emotes.Length)
        {
            emoteImage.sprite = emotes[emoteIndex];
        }
        else
        {
            emoteImage.sprite = transparent;
        }
    }

    private void CancelAndStartInvoke()
    {
        CancelInvoke(nameof(ResetEmote));
        Invoke(nameof(ResetEmote), timer);
    }

    private void ResetEmote()
    {
        if (IsOwner)
        {
            SetEmoteOnAllClientsServerRpc(0);
            controller.ChangeEmote(0);
        }
    }
}
