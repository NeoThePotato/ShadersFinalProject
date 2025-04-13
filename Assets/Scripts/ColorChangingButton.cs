using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Notifies <see cref="UIManager"/> when the user changes the brush color.
/// </summary>
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
