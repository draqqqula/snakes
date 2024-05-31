using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplicativeColorFilter : MonoBehaviour
{
    [SerializeField]
    public Color Color;

    public SpriteRenderer TargetRenderer;

    void Start()
    {
        TargetRenderer.color = TargetRenderer.color * Color;
    }
}
