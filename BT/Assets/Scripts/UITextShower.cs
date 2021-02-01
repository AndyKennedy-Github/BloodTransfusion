using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextShower : MonoBehaviour
{
    public GameObject scoreTracker;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        
    }

    void Update()
    {
        scoreText.text = "Total Points " + scoreTracker.GetComponent<ObjectMessageHandler>().pointTotal + "/9";
        // Putting this here until somebody adds the pointTotal back to the ObjectMessageHandler
        //scoreText.text = "Total Points 0/9";
    }
}
