using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
using TMPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class ClientManager : SingletonMonoBehaviour<ClientManager>
{
    public Client client;
    public string storedDisplayName;
    public string storedTwitchName;
    public string storedRoomCode;

    private bool attemptingReconnection;
    public TextMeshProUGUI connectionStatusMesh;
    public TextMeshProUGUI debugMesh;

    private void Start()
    {
        Application.targetFrameRate = 30;
    }

    #region Connection

    public void AttemptToConnectToRoom(string name, string roomCode)
    {
        storedDisplayName = name;
        client.Connect(name, roomCode);
    }

    public void OnConnected(string roomCode)
    {
        storedRoomCode = roomCode.ToUpperInvariant();
        Invoke("CheckConnection", 3f);
    }

    void CheckConnection()
    {
        if (!client.Connected)
            ClientLandingPageManager.Get.OnCouldNotConnectToRoom();
    }
    public void AttemptRefresh()
    {
        Invoke("DisableAttempt", 3f);
        attemptingReconnection = true;
        if(!string.IsNullOrEmpty(storedTwitchName))
            AttemptToConnectToRoom(ClientMainGame.Get.playerNameMesh.text + "|" + storedTwitchName, storedRoomCode);
        else if(!string.IsNullOrEmpty(storedDisplayName))
            AttemptToConnectToRoom(storedDisplayName, storedRoomCode);
    }

    public void DisableAttempt()
    {
        attemptingReconnection = false;
    }

    public void Update()
    {
        if (ClientMainGame.Get != null)
            debugMesh.text = string.IsNullOrEmpty(ClientMainGame.Get.simpleQuestionInput.text) ? "NO DATA IN INPUT FIELD" : ClientMainGame.Get.simpleQuestionInput.text;

        if (!attemptingReconnection && !client.Connected && !string.IsNullOrEmpty(storedRoomCode)/* && !string.IsNullOrEmpty(storedTwitchName)*/)
            AttemptRefresh();

        connectionStatusMesh.text = !client.Connected ? "<color=red>OFFLINE" : "<color=green>ONLINE WITH HOST";
    }

    #endregion

    #region Payload Management

    public void OnPayloadReceived(DataMessage dm)
    {
        string data = (string)dm.Data;
        EventLibrary.HostEventType ev = EventLibrary.GetHostEventType(dm.Key);        

        switch (ev)
        {
            case EventLibrary.HostEventType.Validate:
                ClientLandingPageManager.Get.OnValidateAccount(data);
                break;

            case EventLibrary.HostEventType.Validated:
                string[] dataArr = data.Split('|');
                ClientLandingPageManager.Get.OnValidated(dataArr);
                break;

            case EventLibrary.HostEventType.SecondInstance:
                ClientMainGame.Get.NewInstanceOpened();
                break;

            case EventLibrary.HostEventType.Information:
                ClientMainGame.Get.DisplayInformation(data);
                break;

            case EventLibrary.HostEventType.UpdateScore:
                ClientMainGame.Get.UpdateCurrentScore(data);
                break;

            case EventLibrary.HostEventType.SimpleQuestion:
                dataArr = data.Split('|');
                ClientMainGame.Get.DisplaySimpleQuestion(dataArr);
                break;

            case EventLibrary.HostEventType.NumericalQuestion:
                dataArr = data.Split('|');
                ClientMainGame.Get.DisplayNumericalQuestion(dataArr);
                break;

            case EventLibrary.HostEventType.MultipleChoiceQuestion:
                dataArr = data.Split('|');
                ClientMainGame.Get.DisplayMultipleChoiceQuestion(dataArr);
                break;

            case EventLibrary.HostEventType.MultiSelectQuestion:
                dataArr = data.Split('|');
                ClientMainGame.Get.DisplayMultiSelectQuestion(dataArr);
                break;

            case EventLibrary.HostEventType.KillSingleMultiSelectButton:
                ClientMainGame.Get.KillSingleMultiSelectButton(data);
                break;

            case EventLibrary.HostEventType.DangerZoneQuestion:
                dataArr = data.Split('|');
                ClientMainGame.Get.DisplayDZQuestion(dataArr);
                break;

            case EventLibrary.HostEventType.SingleAndMultiResult:
                dataArr = data.Split('|');
                ClientMainGame.Get.DisplayAnswerSimpleOrMulti(dataArr);
                break;

            case EventLibrary.HostEventType.WrongApp:
            case EventLibrary.HostEventType.WRONGAPP:
                ClientMainGame.Get.WrongApp();
                break;

            default:
                break;
        }
    }

    public void SendPayloadToHost(string payload, EventLibrary.ClientEventType eventType)
    {
        var js = JsonConvert.SerializeObject(payload);
        JObject j = new JObject
        {
            { EventLibrary.GetClientEventTypeString(eventType), js }
        };
        client.SendEvent(EventLibrary.GetClientEventTypeString(eventType), j);
    }

    public void TestButton()
    {
        ClientLandingPageManager.Get.gameObject.SetActive(true);
    }

    #endregion
}
