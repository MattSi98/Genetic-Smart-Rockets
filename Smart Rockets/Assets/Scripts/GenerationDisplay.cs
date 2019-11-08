using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class GenerationDisplay : MonoBehaviour
{

    public int currentGeneration;
    public Text genText;
    private MainEasy mainEasy;
    public int currentGen;
    // Start is called before the first frame update

    private void Start() {
        currentGen = 0;

    }

    void Update() {
        genText.text = "Generation: " + currentGen;
    }
}
