using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBinder : MonoBehaviour
{
    public int TargetId { get; set; }
    public FrameDisplay Display;
    private Camera _camera;
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Display.Instances.TryGetValue(TargetId, out var target))
        {
            var position = new Vector3(target.transform.position.x, target.transform.position.y, -10);
            _camera.transform.position = position;
        }
    }
}
