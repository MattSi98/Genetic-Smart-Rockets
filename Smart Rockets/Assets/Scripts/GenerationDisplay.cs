using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class GenerationDisplay : MonoBehaviour
{

    public int currentGeneration;
    public Text genText;
    private MainEasy mainEasy;
    // Start is called before the first frame update

    private void Start() {
        foreach (Transform child in transform) {
            Debug.Log("tag: " +child.tag);
            if (child.tag == "Main")
                mainEasy = child.gameObject.GetComponent<MainEasy>();
        }

    }

    void Update() {
        genText.text = "Generation: " + mainEasy.currentGen;
        
    }
}
