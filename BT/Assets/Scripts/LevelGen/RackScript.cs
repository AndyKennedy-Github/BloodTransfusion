using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RackScript : MonoBehaviour
{
    public Transform[] infusionBagLocs;

    // Start is called before the first frame update
    void Start()
    {
        RoomGenerationTest.SetTheRack(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
