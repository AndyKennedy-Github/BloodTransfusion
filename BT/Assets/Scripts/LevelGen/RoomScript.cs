/** RoomScript.cs
 * <summary>
 * Place on any room prefab. Specify Transforms to use for spawn positions in the editor. When
 * the room spawns in, this script runs SetTheScene() on the RoomGenerator, causing objects
 * within the room to spawn in.
 * </summary>
 */

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public Transform[] bedLocs;
    public Transform[] wallLocs;
    public Transform[] bedsideHeadLocs;
    public Transform[] bedsideMidLocs;
    public Transform[] bedsideFootLocs;
    public Transform[] cartLocs;
    public Transform[] screenLocs;
    public Transform[] deskLocs;
    public Transform[] chairLocs;

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<RoomGenerator>())
        {
            RoomGenerator.SetTheScene(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
