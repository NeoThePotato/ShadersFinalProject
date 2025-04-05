using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChangingButton : MonoBehaviour
{
    private Button myButton;
    private Color buttonColor;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        buttonColor = myButton.image.color;
    }

    public void OnPress()
    {
        UIManager.Instance.SelectColor(buttonColor);
    }
}
