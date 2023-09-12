using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI mesh;

    public string containedData;

    public void OnPressButton()
    {
        if (containedData == "SUBMIT!")
            ClientMainGame.Get.OnPressMultiSelectSubmit();
        else
            ClientMainGame.Get.OnPressMultipleChoiceButton(containedData);
    }
}
