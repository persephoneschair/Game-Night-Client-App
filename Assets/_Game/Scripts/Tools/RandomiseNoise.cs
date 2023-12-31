﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomiseNoise : MonoBehaviour
{
    private string[] propertyNames = new string[6] { "_ScaleA", "_ScaleB", "_OffsetA", "_OffsetB", "_TimeScaleA", "_TimeScaleB" };

    public void Start()
    {
        Material mat = GetComponent<Renderer>().material;
        Material newMat = new Material(mat);
        for (int i = 0; i < 6; i++)
        {
            Color newColor;
            if (i < 4)
            {
                newColor = new Color(Random.Range(-2f, 2f), Random.Range(-1.5f, 1.5f), Random.Range(-1f, 1f), Random.Range(-2f, 2f));
            }
            else
            {
                newColor = new Color(Random.Range(-0.1f, 0.1f), Random.Range(-0.2f, 0.2f), Random.Range(-0.3f, 0.3f), Random.Range(-1f, 1f));
            }
            
            newMat.SetColor(propertyNames[i], newColor);
        }
        GetComponent<Renderer>().material = newMat;
    }
}
