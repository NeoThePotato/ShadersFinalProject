using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Unity.Burst;
using Unity.Mathematics;

public class BrushInput : MonoBehaviour
{
	[SerializeField] private BrushController _brushController;
	[SerializeField] private float _scrollSensitivity = 2e-4f;
	[SerializeField] private float _minSize = 0.02f, _maxSize = 1f;
	[SerializeField] private Slider _intensitySlider, _scaleSlider;

	private ref BrushController.BrushData Brush => ref _brushController.Brush;

	private static ButtonControl MainButton => Mouse.current.leftButton;

	private static ButtonControl SecondaryButton => Mouse.current.rightButton;

	private static DeltaControl ScaleInput => Mouse.current.scroll;

	private void Start()
	{
		var ui = UIManager.Instance;
		Brush.Color = ui.DefaultColor;
		Brush.texture = ui.DefaultTexture;
		if (_scaleSlider)
		{
			_scaleSlider.minValue = _minSize;
			_scaleSlider.maxValue = _maxSize;
			_scaleSlider.value = Brush.Scale;
		}
		if (_intensitySlider)
			_intensitySlider.value = Brush.Intensity;
	}

	private void OnEnable()
	{
		var ui = UIManager.Instance;
		if (!ui)
			return;
		ui.OnPaintBrushSelected += OnPaintBrushSelected;
		ui.OnVertexBrushSelected += OnVertexBrushSelected;
		ui.OnTextureChanged += OnTextureChanged;
		if (_intensitySlider)
			_intensitySlider.onValueChanged.AddListener(OnIntensityChanged);
		if (_scaleSlider)
			_scaleSlider.onValueChanged.AddListener(OnSizeChanged);
	}

	private void OnDisable()
	{
		var ui = UIManager.Instance;
		if (!ui)
			return;
		ui.OnPaintBrushSelected -= OnPaintBrushSelected;
		ui.OnVertexBrushSelected -= OnVertexBrushSelected;
		ui.OnTextureChanged -= OnTextureChanged;
		if (_intensitySlider)
			_intensitySlider.onValueChanged.RemoveListener(OnIntensityChanged);
		if (_scaleSlider)
			_scaleSlider.onValueChanged.RemoveListener(OnSizeChanged);
	}

	private void Update()
	{
		if (Brush.PaintingMode = PaintActive(out var hit, out var intensity))
		{
			Brush.UV = hit.textureCoord;
			var absIntensity = math.abs(Brush.Intensity);
			Brush.Intensity = absIntensity * intensity;
		}
		if (ScaleInput.IsActuated())
		{
			Brush.Scale = math.clamp(Brush.Scale + ScaleInput.ReadValue().y * _scrollSensitivity, _minSize, _maxSize);
			if (_scaleSlider)
				_scaleSlider.value = Brush.Scale;
		}

		static bool PaintActive(out RaycastHit hit, out float intensity)
		{
			if (TryGetIntensity(out intensity))
				return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f);
			hit = new();
			return false;
		}

		[BurstCompile]
		static bool TryGetIntensity(out float intensity)
		{
			intensity = (MainButton.isPressed, SecondaryButton.isPressed) switch
			{
				(true, false) => 1f,
				(false, true) => -1f,
				_ => 0f
			};
			return intensity != 0f;
		}
	}

	private void OnVertexBrushSelected() => Brush.ColorMode = false;

	private void OnPaintBrushSelected(Color color)
	{
		Brush.ColorMode = true;
		Brush.Color = color;
	}

	private void OnTextureChanged(Texture texture) => Brush.texture = texture;

	private void OnSizeChanged(float size) => Brush.Scale = size;

	private void OnIntensityChanged(float intensity) => Brush.Intensity = intensity;
}
