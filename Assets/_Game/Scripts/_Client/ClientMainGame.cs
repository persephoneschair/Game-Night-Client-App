using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClientMainGame : SingletonMonoBehaviour<ClientMainGame>
{
    public enum QuestionType { None, SimpleQuestion, NumericalQuestion, MultipleChoice, MultiSelect, DZQuestion };

    [Header("Settings")]
    public QuestionType loadedQuestion = QuestionType.None;
    public bool appInitialised;

    [Header("Key Details")]
    public GameObject keyDetailsObj;
    public TextMeshProUGUI playerNameMesh;
    public TextMeshProUGUI pointsMesh;
    public TextMeshProUGUI clockMesh;
    public GameObject optionsButton;

    [Header("Fixed Message")]
    public GameObject fixedMessageObj;
    public TextMeshProUGUI fixedMessageMesh;
    public GameObject otpAlert;

    [Header("Feedback Box")]
    public GameObject feedbackBoxObj;
    public TextMeshProUGUI feedbackBoxMesh;
    public enum FeedbackBoxColorStyle { Default, Correct, Incorrect };
    public Color[] feedbackBoxBackgroundColors;
    public Color[] feedbackBoxBorderColors;
    public Image feedbackBoxBackground;
    public Image feedbackBoxBorder;

    [Header("Simple Question Group")]
    public GameObject simpleQuestionGroup;
    public TextMeshProUGUI simpleQuestionMesh;
    public TMP_InputField simpleQuestionInput;
    public Button simpleQuestionSubmitButton;

    [Header("Simple Question Group Mobile")]
    public GameObject simpleQuestionGroupMobile;
    public TextMeshProUGUI simpleQuestionMeshMobile;
    public TMP_InputField simpleQuestionInputMobile;
    public SoftKeyboard simpleQuestionSoftKeyboard;

    [Header("Numerical Question Group Desktop")]
    public GameObject numericalQuestionGroup;
    public TextMeshProUGUI numericalQuestionMesh;
    public TMP_InputField numericalQuestionInput;
    public Button numericalQuestionSubmitButton;

    [Header("Numerical Question Group Mobile")]
    public GameObject numericalQuestionGroupMobile;
    public TextMeshProUGUI numericalQuestionMeshMobile;
    public TMP_InputField numericalQuestionInputMobile;
    public SoftKeyboard numericalQuestionSoftKeyboard;

    [Header("Multiple Choice Question Group")]
    public GameObject multipleChoiceQuestionGroup;
    public TextMeshProUGUI multipleChoiceQuestionMesh;

    public List<MultipleChoiceButton> multipleChoiceButtons = new List<MultipleChoiceButton>();

    public Transform answerButtonsTransformTarget;
    public GameObject mcButtonToInstance;

    [Header("Multi Select Question Group")]
    public GameObject multiSelectQuestionGroup;
    public TextMeshProUGUI multiSelectQuestionMesh;
    public MultipleChoiceButton multiSelectSubmitButton;

    public List<MultiSelectButton> multiSelectButtons = new List<MultiSelectButton>();

    //public Transform multiSelectSubmitTransformTarget;
    public Transform multiSelectButtonsTransformTarget;
    public GameObject msButtonToInstance;

    [Header("DangerZone Question Group")]
    public GameObject dangerZoneQuestionGroup;
    public TextMeshProUGUI dangerZoneQuestionMesh;

    public List<MultipleChoiceButton> dzMultipleChoiceButtons = new List<MultipleChoiceButton>();
    public List<MultipleChoiceButton> dzMultiplyButtons = new List<MultipleChoiceButton>();

    public MultipleChoiceButton dzNerfButton;
    public bool dzMultiply;

    private void Update()
    {
        simpleQuestionSubmitButton.interactable = simpleQuestionInput.text.Length > 0 && loadedQuestion == QuestionType.SimpleQuestion ? true : false;
        numericalQuestionSubmitButton.interactable = numericalQuestionInput.text.Length > 0 && loadedQuestion == QuestionType.NumericalQuestion ? true : false;

        if (loadedQuestion == QuestionType.SimpleQuestion && simpleQuestionInput.text.Length > 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            SubmitSimpleQuestion();

        if (loadedQuestion == QuestionType.NumericalQuestion && numericalQuestionInput.text.Length > 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            SubmitNumericalQuestion();

        if (loadedQuestion == QuestionType.MultiSelect && multiSelectSubmitButton != null)
            multiSelectSubmitButton.button.interactable = multiSelectButtons.Where(x => x.isSelected).Count() == 0 ? false : true;
    }

    #region Paste Detection

    private string recordedInput;
    private int speedCap = 0;

    public void OnInputFieldUpdate(string s)
    {
        recordedInput = s;
        speedCap++;
    }

    IEnumerator ResetSpeedCap()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f);
            if (speedCap > 3)
                ClientManager.Get.SendPayloadToHost($"''{recordedInput}''", EventLibrary.ClientEventType.PasteAlert);
            speedCap = 0;
            recordedInput = "";
        }            
    }

    #endregion

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void Initialise(string[] nameAndPoints)
    {
        KillAllGroups();
        this.gameObject.SetActive(true);
        ClientManager.Get.storedTwitchName = nameAndPoints[2];
        playerNameMesh.text = nameAndPoints[0];
        pointsMesh.text = nameAndPoints[1];
        clockMesh.text = "-";
        feedbackBoxObj.SetActive(true);
        feedbackBoxMesh.text = "The game will begin shortly";
        keyDetailsObj.SetActive(true);
        feedbackBoxBorder.color = feedbackBoxBorderColors[(int)FeedbackBoxColorStyle.Default];
        feedbackBoxBackground.color = feedbackBoxBackgroundColors[(int)FeedbackBoxColorStyle.Default];
        StartCoroutine(ResetSpeedCap());

#if UNITY_EDITOR || UNITY_STANDALONE
        optionsButton.SetActive(true);
#endif
    }

    public void UpdateCurrentScore(string data)
    {
        pointsMesh.text = data;
    }

    private IEnumerator TriggerCountdown(int timeInSeconds)
    {
        for(int i = timeInSeconds; i > 0; i--)
        {
            clockMesh.text = i.ToString();
            yield return new WaitForSeconds(1f);
            if (loadedQuestion == QuestionType.None)
                break;
        }
        clockMesh.text = "-";
    }

    public void DisplayInformation(string data)
    {
        loadedQuestion = QuestionType.None;
        KillAllGroups();
        feedbackBoxObj.SetActive(true);
        feedbackBoxMesh.text = $"{data}";
        feedbackBoxBorder.color = feedbackBoxBorderColors[(int)FeedbackBoxColorStyle.Default];
        feedbackBoxBackground.color = feedbackBoxBackgroundColors[(int)FeedbackBoxColorStyle.Default];
    }

    public void DisplaySimpleQuestion(string[] dataArr)
    {
        //[0] = question
        //[1] = (int)time in seconds

        KillAllGroups();

        loadedQuestion = QuestionType.SimpleQuestion;

        if (int.TryParse(dataArr[1], out int time))
            StartCoroutine(TriggerCountdown(time));

        if (Application.isMobilePlatform)
        {
            simpleQuestionGroupMobile.SetActive(true);
            simpleQuestionInputMobile.text = "";
            simpleQuestionMeshMobile.text = dataArr[0];
            simpleQuestionInput.ActivateInputField();
            simpleQuestionSoftKeyboard.ChangeMode(SoftKeyboard.CurrentView.FrontUpper);
        }
        else
        {
            simpleQuestionGroup.SetActive(true);
            simpleQuestionInput.text = "";
            simpleQuestionMesh.text = dataArr[0];
            simpleQuestionInput.ActivateInputField();
        }
    }

    public void SubmitSimpleQuestion()
    {
        if(Application.isMobilePlatform)
        {
            ClientManager.Get.SendPayloadToHost(simpleQuestionInputMobile.text, EventLibrary.ClientEventType.SimpleQuestion);
            simpleQuestionInputMobile.text = "";
        }
        else
        {
            ClientManager.Get.SendPayloadToHost(simpleQuestionInput.text, EventLibrary.ClientEventType.SimpleQuestion);
            simpleQuestionInput.text = "";
        }
        loadedQuestion = QuestionType.None;
        KillAllGroups();
    }


    public void DisplayNumericalQuestion(string[] dataArr)
    {
        //[0] = question
        //[1] = (int)time in seconds

        KillAllGroups();

        loadedQuestion = QuestionType.NumericalQuestion;

        if (int.TryParse(dataArr[1], out int time))
            StartCoroutine(TriggerCountdown(time));

        if (Application.isMobilePlatform)
        {
            numericalQuestionGroupMobile.SetActive(true);
            numericalQuestionInputMobile.text = "";
            numericalQuestionMeshMobile.text = dataArr[0];
            numericalQuestionInput.ActivateInputField();
            numericalQuestionSoftKeyboard.ChangeMode(SoftKeyboard.CurrentView.Back);
        }
        else
        {
            numericalQuestionGroup.SetActive(true);
            numericalQuestionInput.text = "";
            numericalQuestionMesh.text = dataArr[0];
            numericalQuestionInput.ActivateInputField();
        }
    }

    public void SubmitNumericalQuestion()
    {
        if (Application.isMobilePlatform)
        {
            ClientManager.Get.SendPayloadToHost(numericalQuestionInputMobile.text, EventLibrary.ClientEventType.NumericalQuestion);
            simpleQuestionInputMobile.text = "";
        }
        else
        {
            ClientManager.Get.SendPayloadToHost(numericalQuestionInput.text, EventLibrary.ClientEventType.NumericalQuestion);
            numericalQuestionInput.text = "";
        }
        loadedQuestion = QuestionType.None;
        KillAllGroups();
    }

    public void DisplayMultipleChoiceQuestion(string[] dataArr)
    {
        //[0] = question
        //[1] = (int)time in seconds
        //[2]-[n] = options

        KillAllGroups();
        loadedQuestion = QuestionType.MultipleChoice;

        if (int.TryParse(dataArr[1], out int time))
            StartCoroutine(TriggerCountdown(time));

        multipleChoiceQuestionGroup.SetActive(true);
        multipleChoiceQuestionMesh.text = dataArr[0];

        foreach (MultipleChoiceButton b in multipleChoiceButtons)
            Destroy(b.gameObject);
        multipleChoiceButtons.Clear();

        for(int i = 2; i < dataArr.Length; i++)
        {
            var x = Instantiate(mcButtonToInstance, answerButtonsTransformTarget);
            MultipleChoiceButton b = x.GetComponent<MultipleChoiceButton>();
            b.containedData = dataArr[i];
            b.mesh.text = b.containedData;
            multipleChoiceButtons.Add(b);
        }
    }

    public void OnPressMultipleChoiceButton(string data)
    {
        KillAllGroups();
        loadedQuestion = QuestionType.None;
        ClientManager.Get.SendPayloadToHost(data, EventLibrary.ClientEventType.MultipleChoiceQuestion);
    }

    public void DisplayMultiSelectQuestion(string[] dataArr)
    {
        //[0] = question
        //[1] = (int)time in seconds
        //[2]-[n] = options

        KillAllGroups();
        loadedQuestion = QuestionType.MultiSelect;

        if (int.TryParse(dataArr[1], out int time))
            StartCoroutine(TriggerCountdown(time));

        multiSelectQuestionGroup.SetActive(true);
        multiSelectQuestionMesh.text = dataArr[0];

        //If no options are included in the packet
        //Previously spawned buttons persist
        //And no new buttons are created
        if (dataArr.Length > 2)
        {
            foreach (MultiSelectButton bz in multiSelectButtons)
                Destroy(bz.gameObject);
            multiSelectButtons.Clear();

            for (int i = 2; i < dataArr.Length; i++)
            {
                var x = Instantiate(msButtonToInstance, multiSelectButtonsTransformTarget);
                ApplyNewButton(x, dataArr[i]);
            }
        }
    }

    public void KillSingleMultiSelectButton(string data)
    {
        if(multiSelectButtons.Count > 0)
        {
            MultiSelectButton msb = multiSelectButtons.FirstOrDefault(x => x.mesh.text == data);
            if(msb != null)
            {
                msb.isSelected = false;
                msb.checkmark.enabled = false;
                msb.button.interactable = false;
            }
        }
    }

    void ApplyNewButton(GameObject x, string data)
    {
        MultiSelectButton bt = x.GetComponent<MultiSelectButton>();
        bt.containedData = data;
        bt.mesh.text = bt.containedData;
        multiSelectButtons.Add(bt);
    }

    public void OnPressMultiSelectSubmit()
    {
        KillAllGroups();
        loadedQuestion = QuestionType.None;
        string dataPackage = string.Join("|", multiSelectButtons.Where(x => x.isSelected).Select(x => x.containedData));
        ClientManager.Get.SendPayloadToHost(dataPackage, EventLibrary.ClientEventType.MultiSelectQuestion);
    }

    public void DisplayDZQuestion(string[] dataArr)
    {
        //[0] = question
        //[1] = (int)time in seconds
        //[2]-[5] = options
        //[6] = helpAvailable?
        //[7] = nerfPoints

        KillAllGroups();
        dzMultiply = false;
        loadedQuestion = QuestionType.DZQuestion;

        if (int.TryParse(dataArr[1], out int time))
            StartCoroutine(TriggerCountdown(time));

        dangerZoneQuestionGroup.SetActive(true);
        dangerZoneQuestionMesh.text = dataArr[0];

        for (int i = 0; i < 4; i++)
        {
            dzMultipleChoiceButtons[i].containedData = dataArr[i + 2];
            dzMultipleChoiceButtons[i].mesh.text = dzMultipleChoiceButtons[i].containedData;

            dzMultiplyButtons[i].containedData = dataArr[i + 2];
            dzMultiplyButtons[i].button.interactable = dataArr[6] == "TRUE" ? true : false;
        }

        dzNerfButton.button.interactable = dataArr[6] == "TRUE" ? true : false;
        dzNerfButton.mesh.text = $"NERF ({dataArr[7]} POINTS)";
    }

    public void OnPressDZButton(string data)
    {
        KillAllGroups();
        if (dzMultiply && data != "***NERF***")
            data += $"|***MULTIPLY***";
        loadedQuestion = QuestionType.None;
        ClientManager.Get.SendPayloadToHost(data, EventLibrary.ClientEventType.DangerZoneQuestion);
        dzMultiply = false;
    }

    public void DisplayAnswerSimpleOrMulti(string[] data)
    {
        //[0] = answer message
        //[1] = feedbackBox colorstyle enum

        KillAllGroups();
        loadedQuestion = QuestionType.None;
        loadedQuestion = QuestionType.None;
        feedbackBoxObj.SetActive(true);
        feedbackBoxMesh.text = $"{data[0]}";
        feedbackBoxBorder.color = feedbackBoxBorderColors[(int)(FeedbackBoxColorStyle)Enum.Parse(typeof(FeedbackBoxColorStyle), data[1], true)];
        feedbackBoxBackground.color = feedbackBoxBackgroundColors[(int)(FeedbackBoxColorStyle)Enum.Parse(typeof(FeedbackBoxColorStyle), data[1], true)];
    }

    public void NewInstanceOpened()
    {
        KillAllGroups();
        fixedMessageObj.SetActive(true);
        fixedMessageMesh.text = "THE TWITCH ACCOUNT ASSOCIATED WITH THIS CONTROLLER HAS BEEN ASSOCIATED WITH ANOTHER CONTROLLER.\n\n" +
            "THIS CONTROLLER WILL NOT RECEIVE FURTHER DATA FROM THE GAME AND CAN NOW BE CLOSED.\n\n" +
            "IF YOU DID  NOT VALIDATE A NEW CONTROLLER, PLEASE CONTACT THE HOST.";
    }

    public void EndOfGameAlert(string pennyValue)
    {
        KillAllGroups();
        fixedMessageObj.SetActive(true);
        fixedMessageMesh.text = "THE GAME HAS NOW CONCLUDED AND THIS CONTROLLER CAN BE CLOSED.";
    }

    public void WrongApp()
    {
        KillAllGroups();
        fixedMessageObj.SetActive(true);
        fixedMessageMesh.text = "YOU ARE ATTEMPTING TO PLAY THE GAME USING THE WRONG CONTROLLER APP.\n\n" +
            "PLEASE CHECK THE GAME PAGE FOR THE CORRECT LINK TO THE CURRENT GAME.";
    }

    public void KillAllGroups()
    {
        otpAlert.SetActive(false);
        if(ClientLandingPageManager.Get != null && ClientLandingPageManager.Get.gameObject.activeInHierarchy)
            ClientLandingPageManager.Get.gameObject.SetActive(false);

        /*if (multiSelectSubmitButton != null)
        {
            Destroy(multiSelectSubmitButton.gameObject);
            multiSelectSubmitButton = null;
        }*/
        simpleQuestionInput.text = "";
        feedbackBoxObj.SetActive(false);
        simpleQuestionGroup.SetActive(false);
        simpleQuestionGroupMobile.SetActive(false);
        numericalQuestionGroup.SetActive(false);
        numericalQuestionGroupMobile.SetActive(false);
        multipleChoiceQuestionGroup.SetActive(false);
        multiSelectQuestionGroup.SetActive(false);
        dangerZoneQuestionGroup.SetActive(false);
    }

}
