using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextMutationRate : MonoBehaviour
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
        string sliderMessage = "Mutation Rate :  " + sliderUI.value;
        textSliderValue.text = sliderMessage;
    }
}
