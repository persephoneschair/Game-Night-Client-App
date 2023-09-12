using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoftKeyboard : SingletonMonoBehaviour<SoftKeyboard>
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

    public Keypad[] keypads;
    public CurrentView currentView = CurrentView.FrontUpper;
    public TMP_InputField attachedInputField;

    public void Start()
    {
        for(int i = 0; i < keypads.Length; i++)
            keypads[i].OnSetKeypad((FrontKeyboardUpper)i, (FrontKeyboardLower)i, (BackKeyboard)i);
    }

    public void ChangeMode(CurrentView newView)
    {
        currentView = newView;
        for (int i = 0; i < keypads.Length; i++)
            keypads[i].OnModeChange();
    }

    public void OnKeyStroke(FrontKeyboardUpper stroke)
    {
        if(stroke.ToString().Length == 1)
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
                    ClientMainGame.Get.SubmitSimpleQuestion();
                    break;
            }
        }
    }
}
