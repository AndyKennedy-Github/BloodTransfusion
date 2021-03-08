/** SubObjectParent.cs
 * <summary>
 * Place on any room object that may be spawned in with dynamic sub-objects parented under it. Once
 * spawned in, this object will run SpawnSubObjects() from the RoomGenerator.
 * </summary>
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubObjectParent : MonoBehaviour
{
    public Transform[] subObjectLocs;

    void Awake()
    {
        // Remove parent object's transform from the array of child transforms
        //List<Transform> tempTransforms = new List<Transform>(GetComponentsInChildren<Transform>());
        //tempTransforms.Remove(this.transform);
        //subObjectLocs = tempTransforms.ToArray();

        foreach (Transform t in subObjectLocs)
        {
            Debug.Log("SUB OBJECT PARENT: " + gameObject.name + "-=-=-=-=-=-=-= SUB OBJECT LOCATION: " + t.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // if the array has more than just the parent transform in it...
        if (subObjectLocs.Length > 0)
        {
            if (FindObjectOfType<RoomGenerator>())
            {
                RoomGenerator.SpawnSubObjects(this);
            }
        }
    }
}