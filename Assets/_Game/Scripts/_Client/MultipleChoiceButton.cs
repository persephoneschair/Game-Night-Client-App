using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI mesh;
    public bool isDZ;
    public bool isDZMulti;

    public string containedData;

    public void OnPressButton()
    {
        if (containedData == "SUBMIT!")
            ClientMainGame.Get.OnPressMultiSelectSubmit();
        else if(isDZMulti)
        {
            ClientMainGame.Get.dzMultiply = true;
            ClientMainGame.Get.OnPressDZButton(containedData);
        }            
        else if (isDZ)
            ClientMainGame.Get.OnPressDZButton(containedData);
        else
            ClientMainGame.Get.OnPressMultipleChoiceButton(containedData);
    }
}
