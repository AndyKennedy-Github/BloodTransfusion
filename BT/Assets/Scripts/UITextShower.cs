using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UITextShower : MonoBehaviour
{
    public GameObject scoreTracker;
    public Text scoreText/*, scoreText2*/;
    public Text missedText/*, missedText2*/;
    public Text CompleteorFail;
    public int totalMissed;
    public bool perfect, good, bad;
   
    void Start()
    {
        
    }

    void Update()
    {
        if(perfect)
        {
            PerfectEnd();
        }
        else if(good)
        {
            GoodEnd();
        }
        else if(bad)
        {
            BadEnd();
        }
        else
        {
            return;
        }
    }

    public void DisplayText()
    {
        for (int i = 0; i < scoreTracker.GetComponent<ObjectMessageHandler>().stages.Count; i++)
        {
            if (scoreTracker.GetComponent<ObjectMessageHandler>().stages[i] == false)
            {
                totalMissed++;
            }
        }
        missedText.text += "You missed " + totalMissed + " possible check(s)!";
    }  
    
    public void PerfectEnd()
    {  
        scoreText.text = "ACCURACY: " + (int)scoreTracker.GetComponent<ObjectMessageHandler>().percentComplete + "%";
        missedText.text = "You missed 0 possible check(s)!";
        CompleteorFail.text = "SUCCESS";
        CompleteorFail.color = Color.green;
    }

    public void GoodEnd()
    {
        scoreText.text = "ACCURACY: " + (int)scoreTracker.GetComponent<ObjectMessageHandler>().percentComplete + "%";
        missedText.text = "You missed " + totalMissed + " possible check(s)!";
        CompleteorFail.text = "SUCCESS";
        CompleteorFail.color = Color.green;
    }

    public void BadEnd()
    {
        scoreText.text = "ACCURACY: " + (int)scoreTracker.GetComponent<ObjectMessageHandler>().percentComplete + "%";
        missedText.text = "You missed " + totalMissed + " possible check(s)!";
        CompleteorFail.text = "FAILURE";
        CompleteorFail.color = Color.red;
    }
}
