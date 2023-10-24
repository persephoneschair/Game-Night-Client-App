using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientOptions : MonoBehaviour
{
    public Button launchOptionsButton;

    public GameObject optionsPage;

    public void DisplayOptions()
    {
        launchOptionsButton.interactable = false;
        optionsPage.gameObject.SetActive(true);
    }

    public void CloseOptions()
    {
        launchOptionsButton.interactable = true;
        optionsPage.gameObject.SetActive(false);
    }

    public void OnDisconnect()
    {
        ClientManager.Get.client.Disconnect();
        ClientManager.Get.storedRoomCode = "";
        ClientManager.Get.storedDisplayName = "";
        ClientManager.Get.storedTwitchName = "";
        StartCoroutine(SceneReload());
    }

    IEnumerator SceneReload()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("App");

        while (!asyncLoad.isDone)
            yield return null;
    }
}
