using UnityEngine;
using UnityEngine.InputSystem;

public class PaintController : MonoBehaviour
{
    [SerializeField] private Camera _paintCamera;
    [SerializeField] private RenderTexture _colorTex, _heightTex;
    [SerializeField] private Renderer _grassRenderer;
    [SerializeField] private GameObject _brush;

	public bool IsColorMode
	{
		get => _paintCamera.targetTexture == _colorTex;
		set => _paintCamera.targetTexture = value ? _colorTex : _heightTex;
	}

	public bool IsPainting
    {
        get => _brush.activeSelf;
        set => _brush.SetActive(value);
    }

	private void Awake()
	{
		IsColorMode = true;
		IsPainting = false;
	}

	void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
            SwitchMode();
		if (IsPainting = Mouse.current.leftButton.isPressed)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hit, 100f))
			{
				_brush.transform.position = hit.point;
			}
		}
	}

    private void SwitchMode()
    {
		IsColorMode = !IsColorMode;
	}
}
