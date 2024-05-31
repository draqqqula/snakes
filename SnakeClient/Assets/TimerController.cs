using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public TimeSpan TimeElapsed = TimeSpan.Zero;
    private TextMeshPro Text;

    void Start()
    {
        Text = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        TimeElapsed -= TimeSpan.FromSeconds(Time.deltaTime);
        Text.text = TimeElapsed.ToString("mm\\:ss");
    }
}
