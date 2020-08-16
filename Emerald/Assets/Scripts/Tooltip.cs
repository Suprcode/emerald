﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public TMP_Text TextBox;

    private static bool camerarefresh;
    private static Camera _cam;
    public static Camera cam
    {
        get { return _cam; }
        set
        {
            _cam = value;
            camerarefresh = true;
        }

    }
    Vector3 min, max;
    RectTransform rect;
    float offset = 10f;    
    
    // Update is called once per frame
    void Update()
    {
        if (camerarefresh)
        {            
            UpdateCamera();
            camerarefresh = false;
        }

        if (gameObject.activeSelf)
        {
            //get the tooltip position with offset
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y - (rect.rect.height + offset), 0f);
            //clamp it to the screen size so it doesn't go outside
            transform.position = new Vector3(Mathf.Clamp(position.x, min.x + rect.rect.width / 2, max.x - rect.rect.width / 4), Mathf.Clamp(position.y, min.y + rect.rect.height / 2, max.y - rect.rect.height / 2), transform.position.z);
        }
    }

    void UpdateCamera()
    {
        rect = GetComponent<RectTransform>();
        min = new Vector3(0, 0, 0);
        max = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);
    }

    public void Show(string text)
    {
        TextBox.SetText(text);
        gameObject.SetActive(true);
    }
}
