using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickBehaviour : MonoBehaviour
{
    [field: SerializeField]
    public float MaxDistance { get; private set; }
    private RectTransform CanvasRectangle { get; set; }
    public RectTransform Head;
    public RectTransform Body;
    public Vector3 DefaultPosition { get; private set; }
    public bool Active { get; set; } = false;
    private Vector2 BodyPosition { get; set; } = Vector2.zero;
    public float Direction { get; set; } = 0f;

    void Start()
    {
        CanvasRectangle = GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
        DefaultPosition = Body.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Active)
        {
            Body.anchoredPosition = DefaultPosition;
            Head.anchoredPosition = DefaultPosition;
        }
        else
        {
            Body.anchoredPosition = BodyPosition;
        }
    }

    public void Touch(Vector3 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRectangle, screenPosition, Camera.main, out var rectPoint);
        var guiPoint = rectPoint + CanvasRectangle.rect.size / 2;
        if (!Active)
        {
            BodyPosition = guiPoint;
        }
        Active = true;

        var delta = guiPoint - BodyPosition;

        var length = Mathf.Min(delta.magnitude, MaxDistance);

        Head.anchoredPosition = BodyPosition + delta.normalized * length;

        if (delta != Vector2.zero)
        {
            Direction = -System.MathF.Atan2(delta.y, delta.x);
        }
    }

    public void Release()
    {
        Active = false;
    }
}
