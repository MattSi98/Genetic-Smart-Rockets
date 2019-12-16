using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextPopulation : MonoBehaviour
{
    public Slider sliderUI;
    private Text textSliderValue;
    // Start is called before the first frame update
    void Start()
    {
        textSliderValue = GetComponent<Text>();
        ShowSliderValue();

    }
    private void Update()
    {
        ShowSliderValue();
    }
    public void ShowSliderValue()
    {
        string sliderMessage = "Population Size :  " + sliderUI.value;
        textSliderValue.text = sliderMessage;
    }
}
