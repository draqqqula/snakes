using MessageSchemes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public class FrameDisplay : MonoBehaviour
{
    public GameObject[] FramePrefabs;

    private Dictionary<string, GameObject> _prefabDictionary;

    private Dictionary<int, GameObject> _instances = new ();

    private void Start()
    {
        _prefabDictionary = FramePrefabs.ToDictionary(it => it.name, it => it);
    }

    public void Synchronize(Message message)
    {
        for (int i = 0; i < message.GroupsLength; i++)
        {
            var group = message.Groups(i).Value;
            if (_prefabDictionary.TryGetValue(group.Asset, out var prefab))
            {
                for (int j = 0; j < group.FramesLength; j++)
                {
                    var frame = group.Frames(j).Value;
                    var position = new Vector3(frame.Position.X/100, frame.Position.Y/100, 0);
                    var rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * frame.Angle);
                    if (_instances.TryGetValue(frame.Id, out var instance))
                    {
                        instance.transform.position = position;
                        instance.transform.rotation = rotation;
                        continue;
                    }
                    var obj = Instantiate(prefab, position, rotation);
                    _instances.Add(frame.Id, obj);
                }
            }
        }
    }
}
