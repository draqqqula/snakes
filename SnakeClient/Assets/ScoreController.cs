using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum TeamColor
{
    Red,
    Blue,
    Green,
    Yellow,
}

public class ScoreController : MonoBehaviour
{
    public TextMeshPro[] TextObjects;

    public void ChangeValue(TeamColor team, int value)
    {
        var targetObject = TextObjects.FirstOrDefault(it => it.tag == team.ToString());
        if (targetObject is null)
        {
            return;
        }
        targetObject.text = value.ToString();
    }
}
