using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UITextShower : MonoBehaviour
{
    public GameObject scoreTracker;
    public Text scoreText1, scoreText2;
    public Text missedText1, missedText2;
    public int totalMissed;
    void Start()
    {
        
    }

    void Update()
    {
        scoreText1.text = "ACCURACY: " + (int)scoreTracker.GetComponent<ObjectMessageHandler>().percentComplete + "%";
        scoreText2.text = "ACCURACY: " + (int)scoreTracker.GetComponent<ObjectMessageHandler>().percentComplete + "%";
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
        missedText1.text += "You missed " + totalMissed + " possible check(s)!";
        missedText2.text += "You missed " + totalMissed + " possible check(s)!";
    }    
}
