using UnityEngine;
using UnityEngine.UI;

public class ColorChangingButton : MonoBehaviour
{
    private Button myButton;
    public Color Color { get; private set; }

    private void Awake()
    {
        myButton = GetComponent<Button>();
        Color = myButton.image.color;
    }

    public void OnPress()
    {
        UIManager.Instance.SelectColor(Color);
    }
}
