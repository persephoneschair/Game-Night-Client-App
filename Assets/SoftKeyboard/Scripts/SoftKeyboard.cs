using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SoftKeyboard : MonoBehaviour
{
    public enum CurrentView
    {
        FrontUpper,
        FrontLower,
        Back
    }

    public enum FrontKeyboardUpper
    {
        Q,W,E,R,T,Y,U,I,O,P,
        A,S,D,F,G,H,J,K,L,
        Shift, Z,X,C,V,B,N,M, Backspace,
        Toggle, Space, Enter
    }

    public enum FrontKeyboardLower
    {
        q,w,e,r,t,y,u,i,o,p,
        a,s,d,f,g,h,j,k,l,
        Shift, z,x,c,v,b,n,m, Backspace,
        Toggle, Space, Enter
    }

    public enum BackKeyboard
    {
        Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9, Num0,
        Hyphen, ForwardSlash, Colon, SemiColon, OpenParent, CloseParent, Pound, Ampersand, Quotes,
        Nothing, Stop, Comma, QMark, Exclaim, Apostrophe, Dollar, Asterisk, Backspace,
        Toggle, Space, Enter
    }

    public Sprite[] frontSpecialSprites = new Sprite[] { };

    public Sprite[] backSpecialSprites = new Sprite[] { };

    public char[] backKeyboardCharConverter = new char[]
    {
        '1','2','3','4','5','6','7','8','9','0',
        '-','/',':',';','(', ')','£','&','"',
        ' ','.',',','?','!','\'','$','*',' ',
        ' ',' ',' '
    };

    public bool isLandingPage;
    public bool isNumerical;
    public Keypad[] keypads;
    public CurrentView currentView = CurrentView.FrontUpper;
    public TMP_InputField attachedInputField;
    public TextMeshProUGUI promptMesh;

    public TMP_InputField []variableInputs;
    private int currentInput = 0;

    public void Start()
    {
        if (isNumerical)
            currentView = CurrentView.Back;

        for (int i = 0; i < keypads.Length; i++)
            keypads[i].OnSetKeypad((FrontKeyboardUpper)i, (FrontKeyboardLower)i, (BackKeyboard)i);

        if (isNumerical)
        {
            keypads[10].OnSetKeypad(FrontKeyboardUpper.Backspace, FrontKeyboardLower.Backspace, BackKeyboard.Backspace);
            keypads[10].image.enabled = true;
            keypads.LastOrDefault().OnSetKeypad(FrontKeyboardUpper.Enter, FrontKeyboardLower.Enter, BackKeyboard.Enter);
            keypads.LastOrDefault().image.enabled = true;
        }

        if (isLandingPage)
        {
            attachedInputField = variableInputs[currentInput];
            keypads[28].upperInput = FrontKeyboardUpper.Toggle;
            keypads[28].image.enabled = false;
            keypads[28].label.text = "SWITCH";
            promptMesh.text = "ENTER NAME";
            keypads[30].button.interactable = false;
        }            
    }

    public void ChangeMode(CurrentView newView)
    {
        currentView = newView;
        for (int i = 0; i < keypads.Length; i++)
            keypads[i].OnModeChange();
    }

    public void OnKeyStroke(FrontKeyboardUpper stroke)
    {
        if (isLandingPage)
        {
            OnLandingPageKeyStroke(stroke);
            return;
        }

        if (stroke.ToString().Length == 1)
            attachedInputField.text += stroke.ToString();
        else
        {
            switch(stroke)
            {
                case FrontKeyboardUpper.Shift:
                    ChangeMode(CurrentView.FrontLower);
                    break;

                case FrontKeyboardUpper.Backspace:
                    if(attachedInputField.text.Length > 0)
                        attachedInputField.text = attachedInputField.text.Substring(0, attachedInputField.text.Length - 1);
                    break;

                case FrontKeyboardUpper.Toggle:
                    ChangeMode(CurrentView.Back);
                    break;

                case FrontKeyboardUpper.Space:
                    attachedInputField.text += " ";
                    break;

                case FrontKeyboardUpper.Enter:
                    ClientMainGame.Get.SubmitSimpleQuestion();
                    break;
            }
        }
    }

    public void OnKeyStroke(FrontKeyboardLower stroke)
    {            
        if (stroke.ToString().Length == 1)
            attachedInputField.text += stroke.ToString();
        else
        {
            switch (stroke)
            {
                case FrontKeyboardLower.Shift:
                    ChangeMode(CurrentView.FrontUpper);
                    break;

                case FrontKeyboardLower.Backspace:
                    if (attachedInputField.text.Length > 0)
                        attachedInputField.text = attachedInputField.text.Substring(0, attachedInputField.text.Length - 1);
                    break;

                case FrontKeyboardLower.Toggle:
                    ChangeMode(CurrentView.Back);
                    break;

                case FrontKeyboardLower.Space:
                    attachedInputField.text += " ";
                    break;

                case FrontKeyboardLower.Enter:
                    ClientMainGame.Get.SubmitSimpleQuestion();
                    break;
            }
        }
    }

    public void OnKeyStroke(BackKeyboard stroke)
    {
        if (backKeyboardCharConverter[(int)stroke] != ' ')
            attachedInputField.text += backKeyboardCharConverter[(int)stroke].ToString();

        else
        {
            switch(stroke)
            {
                case BackKeyboard.Nothing:
                    break;

                case BackKeyboard.Backspace:
                    if (attachedInputField.text.Length > 0)
                        attachedInputField.text = attachedInputField.text.Substring(0, attachedInputField.text.Length - 1);
                    break;

                case BackKeyboard.Toggle:
                    ChangeMode(CurrentView.FrontUpper);
                    break;

                case BackKeyboard.Space:
                    attachedInputField.text += " ";
                    break;

                case BackKeyboard.Enter:
                    if(isNumerical)
                        ClientMainGame.Get.SubmitNumericalQuestion();
                    else
                        ClientMainGame.Get.SubmitSimpleQuestion();
                    break;
            }
        }
    }

    public void OnLandingPageKeyStroke(FrontKeyboardUpper stroke)
    {
        if (stroke.ToString().Length == 1)
        {
            if ((currentInput == 1 && attachedInputField.text.Length > 3) || (currentInput == 0 && attachedInputField.text.Length > 14))
                return;
            attachedInputField.text += stroke.ToString();
            keypads[30].button.interactable = (variableInputs[0].text.Length > 0 && variableInputs[1].text.Length == 4) ? true : false;
        }            

        else
        {
            switch (stroke)
            {
                case FrontKeyboardUpper.Toggle:
                    currentInput = (currentInput + 1) % variableInputs.Length;
                    attachedInputField = variableInputs[currentInput];
                    promptMesh.text = currentInput == 0 ? "ENTER NAME" : "ENTER ROOM CODE";
                    break;

                case FrontKeyboardUpper.Backspace:
                    if (attachedInputField.text.Length > 0)
                        attachedInputField.text = attachedInputField.text.Substring(0, attachedInputField.text.Length - 1);
                    break;

                case FrontKeyboardUpper.Enter:
                    ClientLandingPageManager.Get.OnPressJoinRoomButton();
                    currentInput = 0;
                    attachedInputField = variableInputs[currentInput];
                    promptMesh.text = currentInput == 0 ? "ENTER NAME" : "ENTER ROOM CODE";
                    break;
            }
        }
    }
}
