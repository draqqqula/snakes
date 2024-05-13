using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAdapter : MonoBehaviour
{
    private JoystickBehaviour Joystick { get; set; }
    void Start()
    {
        Joystick = GetComponent<JoystickBehaviour>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Joystick.Touch(Input.mousePosition);
        }
        else
        {
            Joystick.Release();
        }
    }
}
