using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

	#region EVENTS
	public event UnityAction<Color> OnPaintBrushSelected;
    public event UnityAction OnVertexBrushSelected;
    public event UnityAction<Texture> OnTextureChanged;
    public event UnityAction<float> OnIntensityChanged;
    public event UnityAction<float> OnSizeChanged;
    #endregion

    public Color DefaultColor => GetComponentInChildren<ColorChangingButton>().Color;

    public Texture DefaultTexture => GetComponentInChildren<TextureChangingButton>().Texture;

	private void Awake() => Instance = this;

	public void SelectColor(Color color) => OnPaintBrushSelected?.Invoke(color);

	public void SelectTexture(Texture texture) => OnTextureChanged?.Invoke(texture);

	public void SelectVertexBrush() => OnVertexBrushSelected?.Invoke();

	public void ChangeSize(float value) => OnSizeChanged?.Invoke(value);

	public void ChangeIntensity(float value) => OnIntensityChanged?.Invoke(value);
}