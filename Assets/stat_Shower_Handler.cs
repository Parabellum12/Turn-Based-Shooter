using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class stat_Shower_Handler : MonoBehaviour
{
    [SerializeField] TMP_Text Text;
    [SerializeField] Slider CurrentValueSlider;
    [SerializeField] Slider TempValueSlider;
    [SerializeField] float currentValue = 0;
    [SerializeField] float temValue = 0;
    float maxValue = 100;
    float moveBy;

    public void setValues(string text, float currentValue, float maxValue)
    {
        this.maxValue = maxValue;
        this.currentValue = currentValue;
        this.Text.text = text;

        moveBy = maxValue / 300;
    }

    public void setTempValue(float temp)
    {
        temValue = temp;
    }

    public void setCurrentValue(float curVal)
    {
        currentValue = curVal;
        temValue = currentValue;
    }

    public void resetTemp()
    {
        temValue = currentValue;
    }

    float flashTimer = 0;
    //true = white, false = color
    bool FlashOrColor = false;
    [SerializeField] Color Startcolor;
    private void Update()
    {
        flashTimer += Time.deltaTime;
        if (flashTimer > 1)
        {
            flashTimer -= 1;
            FlashOrColor = !FlashOrColor;
        }
        /*
        if (FlashOrColor)
        {
            //white
            CurrentValueSlider.image.color = Color.Lerp(CurrentValueSlider.image.color, Color.white, flashTimer);
        }
        else
        {
            //color
            CurrentValueSlider.image.color = Color.Lerp(Color.white, CurrentValueSlider.image.color, flashTimer);
        }
        */
        TempValueSlider.value = Mathf.RoundToInt(temValue);
        CurrentValueSlider.value = Mathf.RoundToInt(currentValue);
    }
}
