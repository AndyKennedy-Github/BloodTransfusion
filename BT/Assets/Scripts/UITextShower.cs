using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextShower : MonoBehaviour
{
    public GameObject scoreTracker;
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Total Points " + scoreTracker.GetComponent<ObjectMessageHandler>().pointTotal + "/9";
    }
}
