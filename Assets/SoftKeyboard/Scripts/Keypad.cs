using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    public SoftKeyboard attachedKeyboard;

    public SoftKeyboard.FrontKeyboardUpper upperInput;
    public SoftKeyboard.FrontKeyboardLower lowerInput;
    public SoftKeyboard.BackKeyboard backInput;

    public Button button;
    public TextMeshProUGUI label;
    public Image image;

    public void OnSetKeypad(SoftKeyboard.FrontKeyboardUpper upper, SoftKeyboard.FrontKeyboardLower lower, SoftKeyboard.BackKeyboard back)
    {
        upperInput = upper;
        lowerInput = lower;
        backInput = back;
        OnModeChange();
    }

    public void OnModeChange()
    {
        switch(attachedKeyboard.currentView)
        {
            case SoftKeyboard.CurrentView.FrontUpper:
                switch(upperInput)
                {
                    case SoftKeyboard.FrontKeyboardUpper.Space:
                        label.text = "space";
                        image.enabled = false;
                        break;

                    case SoftKeyboard.FrontKeyboardUpper.Toggle:
                        label.text = "123";
                        image.enabled = false;
                        break;

                    default:
                        if(upperInput.ToString().Length == 1)
                        {
                            label.text = upperInput.ToString();
                            image.enabled = false;
                        }
                        else
                        {
                            image.sprite = attachedKeyboard.frontSpecialSprites[(int)upperInput];
                            image.enabled = true;
                            label.text = "";
                        }
                        break;
                }
                    
                break;

            case SoftKeyboard.CurrentView.FrontLower:
                switch(lowerInput)
                {
                    case SoftKeyboard.FrontKeyboardLower.Space:
                        label.text = "space";
                        image.enabled = false;
                        break;

                        case SoftKeyboard.FrontKeyboardLower.Toggle:
                        label.text = "123";
                        image.enabled = false;
                        break;

                        default:
                        {
                            if(lowerInput.ToString().Length == 1)
                            {
                                label.text = lowerInput.ToString();
                                image.enabled = false;
                            }
                            else
                            {
                                image.sprite = attachedKeyboard.frontSpecialSprites[(int)lowerInput];
                                image.enabled = true;
                                label.text = "";
                            }
                        }
                        break;
                }
                break;

            case SoftKeyboard.CurrentView.Back:
                switch(backInput)
                {
                    case SoftKeyboard.BackKeyboard.Space:
                        label.text = "space";
                        image.enabled = false;
                        break;

                    case SoftKeyboard.BackKeyboard.Nothing:
                        label.text = "";
                        image.enabled = false;
                        break;

                    case SoftKeyboard.BackKeyboard.Toggle:
                        label.text = "ABC";
                        image.enabled = false;
                        break;

                    case SoftKeyboard.BackKeyboard.Backspace:
                    case SoftKeyboard.BackKeyboard.Enter:
                        image.sprite = attachedKeyboard.backSpecialSprites[(int)backInput];
                        label.text = "";
                        break;

                    default:
                        label.text = attachedKeyboard.backKeyboardCharConverter[(int)backInput].ToString();
                        image.enabled = false;
                        break;

                }
                break;
        }
    }

    public void OnPressButton()
    {
        switch(attachedKeyboard.currentView)
        {
            case SoftKeyboard.CurrentView.FrontUpper:
                attachedKeyboard.OnKeyStroke(upperInput);
                break;

            case SoftKeyboard.CurrentView.FrontLower:
                attachedKeyboard.OnKeyStroke(lowerInput);
                break;

            case SoftKeyboard.CurrentView.Back:
                attachedKeyboard.OnKeyStroke(backInput);
                break;
        }
    }
}
