using UnityEngine;
using UnityEngine.InputSystem;

public class PaintController : MonoBehaviour
{
    [SerializeField] private Camera _paintCamera;
    [SerializeField] private RenderTexture _colorTex, _heightTex;
    [SerializeField] private Renderer _grassRenderer;
    [SerializeField] private GameObject _brush;

    public bool IsColorMode => _paintCamera.targetTexture == _colorTex;
    public bool IsPainting
    {
        get => _brush.activeSelf;
        private set => _brush.SetActive(value);
    }

	private void Awake()
	{
		_paintCamera.targetTexture = _colorTex;
		IsPainting = false;
	}

	void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            SwitchMode();
            var mode = IsColorMode ? "Paint Mode" : "Trim Mode";
			print($"Switched to {mode} mode.");
		}
		if (IsPainting = Mouse.current.leftButton.isPressed)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hit, 100f))
			{
				print("Hit something! " + hit.collider.name);
				_brush.transform.position = hit.point;
			}
		}
	}

    private void SwitchMode()
    {
        if (IsColorMode)
            _paintCamera.targetTexture = _heightTex;
		else
			_paintCamera.targetTexture = _colorTex;
	}
}
