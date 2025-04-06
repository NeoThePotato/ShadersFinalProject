using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.Burst;
using Unity.Mathematics;

public class BrushInput : MonoBehaviour
{
	[SerializeField] private BrushController _brushController;
	[SerializeField] private float _scrollSensitivity = 2e-4f;
	[SerializeField] private float _minSize = 0.02f, _maxSize = 1f;

	private ref BrushController.BrushData Brush => ref _brushController.Brush;

	private static ButtonControl MainButton => Mouse.current.leftButton;

	private static ButtonControl SecondaryButton => Mouse.current.rightButton;

	private static DeltaControl ScaleInput => Mouse.current.scroll;

	private void OnEnable()
	{
		if (!UIManager.Instance)
			return;
		UIManager.Instance.OnPaintBrushSelected += OnPaintBrushSelected;
		UIManager.Instance.OnVertexBrushSelected += OnVertexBrushSelected;
	}

	private void OnDisable()
	{
		if (!UIManager.Instance)
			return;
		UIManager.Instance.OnPaintBrushSelected -= OnPaintBrushSelected;
		UIManager.Instance.OnVertexBrushSelected -= OnVertexBrushSelected;
	}

	private void Update()
	{
		if (Brush.Painting = PaintActive(out var hit, out var intensity))
		{
			Brush.UV = hit.textureCoord;
			Brush.Intensity = math.abs(Brush.Intensity) * intensity;
		}
		if (ScaleInput.IsActuated())
			Brush.Scale = math.clamp(Brush.Scale + ScaleInput.ReadValue().y * _scrollSensitivity, _minSize, _maxSize);

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

	private void OnVertexBrushSelected()
	{
		Brush.Color = false;
	}

	private void OnPaintBrushSelected(Color color)
	{
		Brush.Color = true;
	}
}
