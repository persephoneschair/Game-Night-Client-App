using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiSelectButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI mesh;

    public string containedData;
    public Image checkmark;
    public bool isSelected;

    public void OnPressButton()
    {
        isSelected = !isSelected;
        checkmark.enabled = isSelected;
    }
}
