using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    [SerializeField] private Texture2D paintBrushCursorSprite, vertexBrushCursorSprite;
    [SerializeField] private Vector2 cursorHotSpot = new Vector2(50, 50);
    [SerializeField] private Image currentColorImage;
    [SerializeField] private RotateCamera _rotateCamera;

	private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UIManager.Instance.OnPaintBrushSelected += (Color color) =>
        {
            currentColorImage.color = color;
            currentColorImage.gameObject.SetActive(true);
            Cursor.SetCursor(paintBrushCursorSprite, cursorHotSpot, CursorMode.Auto);
        };
        UIManager.Instance.OnVertexBrushSelected += () =>
        {
            currentColorImage.gameObject.SetActive(false);
            Cursor.SetCursor(vertexBrushCursorSprite, cursorHotSpot, CursorMode.Auto);
        };
        
        currentColorImage.gameObject.SetActive(false);
        Cursor.SetCursor(vertexBrushCursorSprite, cursorHotSpot, CursorMode.Auto);
    }

	private void Update()
	{
        _rotateCamera.enabled = Mouse.current.middleButton.IsActuated();
	}
}