using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class BrushInput : MonoBehaviour
{
	[SerializeField] private BrushController _brushController;

	private ref BrushController.BrushData Brush => ref _brushController.Brush;

	private static ButtonControl MainButton => Mouse.current.leftButton;

	private static ButtonControl SecondaryButton => Mouse.current.rightButton;

	private void Update()
	{
		if (SecondaryButton.wasPressedThisFrame)
			Brush.SwitchMode();
		if (Brush.Painting = PaintActive(out var hit))
			Brush.uv = hit.textureCoord;

		static bool PaintActive(out RaycastHit hit)
		{
			if (MainButton.isPressed)
				return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f);
			hit = new();
			return false;
		}
	}
}
