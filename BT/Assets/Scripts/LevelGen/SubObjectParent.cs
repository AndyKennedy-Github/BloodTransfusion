﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubObjectParent : MonoBehaviour
{
    public Transform[] subObjectLocs;

    void Awake()
    {
        // Remove parent object's transform from the array of child transforms
        List<Transform> tempTransforms = new List<Transform>(GetComponentsInChildren<Transform>());
        tempTransforms.Remove(this.transform);
        subObjectLocs = tempTransforms.ToArray();

        foreach (Transform t in tempTransforms)
        {
            Debug.Log("-=-==-=-=-=-=-= SUB OBJECT LOCATION: " + t.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // if the array has more than just the parent transform in it...
        if (subObjectLocs.Length > 0)
        {
            RoomGenerator.SpawnSubObjects(this);
        }
    }
}