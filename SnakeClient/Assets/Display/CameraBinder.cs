using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraBinder : MonoBehaviour
{
    [SerializeField]
    public float InterpolationFactor = 0.07f;
    [SerializeField]
    public int TargetFrameRate = 100;
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
            var destination = new Vector3(target.transform.position.x, target.transform.position.y, -10);
            var position = Vector3.Lerp(
                transform.position, 
                destination,
                1 - Mathf.Pow(1 - InterpolationFactor, Time.deltaTime * TargetFrameRate)
                );
            _camera.transform.SetParent(target.transform, false);
            target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -10);
        }
    }
}
