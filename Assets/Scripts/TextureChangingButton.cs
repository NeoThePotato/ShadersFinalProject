using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Notifies <see cref="UIManager"/> when the user changes the brush texture.
/// </summary>
public class TextureChangingButton : MonoBehaviour
{
    private Button myButton;
	public Texture Texture { get; private set; }

    private void Awake()
    {
        myButton = GetComponent<Button>();
        Texture = myButton.image.mainTexture;
    }

    public void OnPress()
    {
        UIManager.Instance.SelectTexture(Texture);
    }
}
