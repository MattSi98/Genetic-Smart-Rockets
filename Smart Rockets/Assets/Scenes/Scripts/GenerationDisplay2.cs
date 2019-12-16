using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerationDisplay2 : MonoBehaviour
{
    public int currentGeneration;
    public Text genText;
    private MainMed mainMed;
    // Start is called before the first frame update

    private void Start()
    {
        foreach (Transform child in transform)
        {
            Debug.Log("tag: " + child.tag);
            if (child.tag == "Main")
                mainMed = child.gameObject.GetComponent<MainMed>();
        }

    }

    void Update()
    {
        genText.text = "Generation: " + mainMed.currentGen;

    }
}




