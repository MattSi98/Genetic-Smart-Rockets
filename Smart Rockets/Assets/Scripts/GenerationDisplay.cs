using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class GenerationDisplay : MonoBehaviour
{

    public int currentGeneration;
    public Text genText;
    // Start is called before the first frame update

    private void Start()
    {
        GameObject main = GameObject.Find("MainEasy");
        
        MainEasy mainE = main.GetComponent<MainEasy>();
        currentGeneration = mainE.currentGen; 
        
    }

    void Update()
    {
        Debug.Log("Generation: " + currentGeneration);
    genText.text = "Generation: " + 5;
        
    }
}
