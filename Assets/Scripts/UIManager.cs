using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private Color _defaultColor;
    [SerializeField] private Texture _defaultTexture;

    /// <summary>
    /// You can read this value at any time to know the currently selected brush color
    /// </summary>
    public Color CurrentlySelectedColor { get; private set; }

    public Texture CurrentlySelectedTexture { get; private set; }
    /// <summary>
    /// Subscribe to this in order to select the coloring brush and change the brush's color
    /// </summary>
    public event UnityAction<Color> OnPaintBrushSelected;
    /// <summary>
    /// Subscribe to this in order to select the vertex brush
    /// </summary>
    public event UnityAction OnVertexBrushSelected;

    public event UnityAction<Texture> OnTextureChanged;

    private void Awake()
    {
        Instance = this;
        CurrentlySelectedColor = GetComponentInChildren<ColorChangingButton>().Color;
		CurrentlySelectedTexture = GetComponentInChildren<TextureChangingButton>().Texture;
    }

    public void SelectColor(Color color)
    {
        OnPaintBrushSelected?.Invoke(CurrentlySelectedColor = color);
	}

	public void SelectTexture(Texture texture)
	{
        OnTextureChanged?.Invoke(CurrentlySelectedTexture = texture);
	}

	public void SelectVertexBrush()
    {
        OnVertexBrushSelected?.Invoke();
    }
}