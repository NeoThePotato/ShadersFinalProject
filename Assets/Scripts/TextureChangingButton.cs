using UnityEngine;
using UnityEngine.UI;

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
