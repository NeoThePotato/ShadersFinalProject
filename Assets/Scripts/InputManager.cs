using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    [SerializeField] private Texture2D paintBrushCursorSprite, vertexBrushCursorSprite;
    [SerializeField] private Vector2 cursorHotSpot = new Vector2(50, 50);
    [SerializeField] private Image currentColorImage;
    
    private bool isColoring = false;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UIManager.Instance.OnPaintBrushSelected += (Color color) =>
        {
            isColoring = true;
            currentColorImage.color = color;
            currentColorImage.gameObject.SetActive(true);
            Cursor.SetCursor(paintBrushCursorSprite, cursorHotSpot, CursorMode.Auto);
        };
        UIManager.Instance.OnVertexBrushSelected += () =>
        {
            isColoring = false;
            currentColorImage.gameObject.SetActive(false);
            Cursor.SetCursor(vertexBrushCursorSprite, cursorHotSpot, CursorMode.Auto);
        };
        
        currentColorImage.gameObject.SetActive(false);
        Cursor.SetCursor(vertexBrushCursorSprite, cursorHotSpot, CursorMode.Auto);
    }

    /// <summary>
    /// Read this value on Update to get the current brush function and apply it appropriately
    /// </summary>
    /// <returns></returns>
    public ActiveBrushFunction GetCurrentBrushFunction()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            return ActiveBrushFunction.None;
        if (Mouse.current.leftButton.isPressed)
        {
            return isColoring ? ActiveBrushFunction.ColorPaint : ActiveBrushFunction.VertexPush;
        }
        if (Mouse.current.rightButton.isPressed)
        {
            return isColoring ? ActiveBrushFunction.ColorErasure : ActiveBrushFunction.VertexPull;
        }
        return ActiveBrushFunction.None;
    }
    
    public enum ActiveBrushFunction
    {
        None,
        ColorPaint,
        ColorErasure,
        VertexPush,
        VertexPull
    }
}