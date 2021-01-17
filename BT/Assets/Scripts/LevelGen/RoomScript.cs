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

    // Start is called before the first frame update
    void Start()
    {
        RoomGenerationTest.SetTheScene(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
