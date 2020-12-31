using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public bool toMove = false;
    public bool follow = false;
    public string followTarget;
    public Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (follow)
        {
            follow = true;
            FollowObj();
        }
    }

    public virtual bool HandleMessage(string msg, string param = null)
    {
        print(this.name + ": Handle Message " + msg + " for " + this.name + " with param = " + param);
        if (msg == "follow")
        {
            if (param != "false")
            {
                followTarget = param;
                follow = true;
                print("I will follow");
            }
            else
            {
                follow = false;
            }
        }
        if (msg == "moveto" || msg == "align")
        {
            print("hello, I am moving");
            toMove = true;
            if ((param[0] == '-' || System.Char.IsDigit(param[0])))
            { //moveTo position
                pos = getVector3(param);
                transform.position = pos;
            }
            else
            {
                GameObject go = GameObject.Find(param); //moveTo object's position
                print("moving to position of game object " + go.name);
                pos = go.transform.position;
                if (msg == "align")
                    transform.rotation = go.transform.rotation;
                transform.position = pos;
            }

            print("The position is " + pos);

            //do something...
        }

        //ON
        if (msg == "on")
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
        // OFF
        if (msg == "off")
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (msg == "ison")
        {
            return this.transform.GetChild(0).gameObject.activeSelf;
        }


        return true;
    }

    public Vector3 getVector3(string rString)
    {
        print("getVector3:" + rString);
        char[] separators = new char[] { ' ', ',' };

        string[] temp = rString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        //string[] temp = rString.Split(' ');
        //        print("getVector3x:"+ temp[0]);
        //        print("getVector3y:"+ temp[1]);
        //        print("getVector3z:"+ temp[2]);
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);
        //        print("getVector3: ("+x+","+y+","+z+")");
        Vector3 rValue = new Vector3(x, y, z);
        return rValue;
    }

    private void followPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = player.transform.position - transform.position;
        transform.LookAt(player.transform);
        //rb.MovePosition((Vector3)transform.position + transform.forward *  Time.fixedDeltaTime);//(direction * movementSpeed * Time.fixedDeltaTime));
        this.transform.position = ((Vector3)transform.position + transform.forward * Time.fixedDeltaTime);//(direction * movementSpeed * Time.fixedDeltaTime));
    }

    private void FollowObj()
    {
        var player = GameObject.FindGameObjectWithTag(followTarget);
        Vector3 direction = player.transform.position - transform.position;
        transform.LookAt(player.transform);
        //rb.MovePosition((Vector3)transform.position + transform.forward *  Time.fixedDeltaTime);//(direction * movementSpeed * Time.fixedDeltaTime));
        this.transform.position = ((Vector3)transform.position + transform.forward * Time.fixedDeltaTime);//(direction * movementSpeed * Time.fixedDeltaTime));
    }

}
