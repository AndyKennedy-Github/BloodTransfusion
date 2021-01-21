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
        // This should be rewritten to only update the point display when the player's score changes, not on Update()
        scoreText.text = "Total Points " + scoreTracker.GetComponent<ObjectMessageHandler>().pointTotal + "/9";
    }
}
