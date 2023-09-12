using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientLandingPageManager : SingletonMonoBehaviour<ClientLandingPageManager>
{
    [Header("Landing Page")]
    public TextMeshProUGUI versionMesh;
    public TMP_InputField nameInput;
    public TMP_InputField roomCodeInput;
    public Button joinButton;
    private bool attemptingConnection;

    [Header("Loading Wheel")]
    public GameObject[] wheelSpokes;

    [Header("OTP Fields")]
    private readonly string otpMessage = "It looks like this is the first time you've used this app.\n\nPlease type\n<size=300%><color=yellow>[ABCD]</color></size>\ninto the public chat to connect your Twitch account.\n\nShould you wish to play another game in the future, use the same platform/browser to skip this step.";
    public GameObject otpAlert;
    public TextMeshProUGUI otpMesh;

    private void Update()
    {
        joinButton.interactable = (nameInput.text == "" || roomCodeInput.text.Length != 4) ? false : true;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (nameInput.isFocused)
                roomCodeInput.ActivateInputField();
            else if(roomCodeInput.isFocused)
                nameInput.ActivateInputField();
        }

        if ((joinButton.gameObject.activeInHierarchy && joinButton.interactable) && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
            OnPressJoinRoomButton();
    }

    private void Start()
    {
        versionMesh.text = versionMesh.text.Replace("[##]", Application.version);
    }

    public void OnPressJoinRoomButton()
    {
        ClientManager.Get.AttemptToConnectToRoom(nameInput.text.ToUpperInvariant(), roomCodeInput.text.ToUpperInvariant());
        roomCodeInput.text = "";
        nameInput.text = "";
        joinButton.gameObject.SetActive(false);
        attemptingConnection = true;
        StartCoroutine(DoLoadingWheel());
    }

    IEnumerator DoLoadingWheel()
    {
        while (attemptingConnection)
        {
            for (int i = 0; i < wheelSpokes.Length; i++)
            {
                foreach (GameObject g in wheelSpokes)
                    g.gameObject.SetActive(true);
                wheelSpokes[i].gameObject.SetActive(false);
                yield return new WaitForSeconds(0.075f);
                if (!attemptingConnection)
                    break;
            }
            if (!attemptingConnection)
            {
                foreach (GameObject g in wheelSpokes)
                    g.gameObject.SetActive(false);
                break;
            }
        }
    }

    public void OnCouldNotConnectToRoom()
    {
        attemptingConnection = false;
        joinButton.gameObject.SetActive(true);
        nameInput.ActivateInputField();
    }

    public void OnValidateAccount(string otp)
    {
        attemptingConnection = false;
        joinButton.gameObject.SetActive(false);
        nameInput.gameObject.SetActive(false);
        roomCodeInput.gameObject.SetActive(false);

        if (File.Exists(Application.persistentDataPath + @"\persist.txt"))
        {
            try
            {
                string attempt = Encryption.DecryptData(File.ReadAllText(Application.persistentDataPath + @"\persist.txt"));
                if(!string.IsNullOrEmpty(attempt))
                {
                    ValidateFromStoredName(attempt, otp);
                    return;
                }
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }            
        }

        otpMesh.text = otpMessage.Replace("[ABCD]", otp);
        otpAlert.SetActive(true);
    }

    public void ValidateFromStoredName(string name, string otp)
    {
        otpMesh.text = "LOADING SAVED USER";
        otpAlert.SetActive(true);
        ClientManager.Get.SendPayloadToHost($"{name}|{otp}", EventLibrary.ClientEventType.StoredValidation);
    }

    public void OnValidated(string[] otpArr)
    {
        //Default [0] is player name
        //Default [1] is player points
        //Default [2] is twitch name

        File.WriteAllText(Application.persistentDataPath + @"\persist.txt", Encryption.EncryptData(otpArr[2]));

        otpAlert.SetActive(false);
        ClientMainGame.Get.Initialise(otpArr);
        this.gameObject.SetActive(false);
    }
}
