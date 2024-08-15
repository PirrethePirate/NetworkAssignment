using UnityEngine;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject startButton;
    private bool showGUI = true;


    private void OnGUI()
    {
        if (showGUI)
        {
            if (GUILayout.Button("Host"))
            {
                networkManager.StartHost();
                startButton.SetActive(true);
                HideUI();
            }

            if (GUILayout.Button("Join"))
            {
                networkManager.StartClient();
                startButton.SetActive(false);
                HideUI();
            }

            if (GUILayout.Button("Quit"))
            {
                Application.Quit();
            }
        }
    }

    private void HideUI()
    {
        showGUI = false;
    }

}
