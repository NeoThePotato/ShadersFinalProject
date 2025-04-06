using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    /// <summary>
    /// You can read this value at any time to know the currently selected brush color
    /// </summary>
    public Color currentlySelectedColor { get; private set; }
    /// <summary>
    /// Subscribe to this in order to select the coloring brush and change the brush's color
    /// </summary>
    public event UnityAction<Color> OnPaintBrushSelected;
    /// <summary>
    /// Subscribe to this in order to select the vertex brush
    /// </summary>
    public event UnityAction OnVertexBrushSelected;
    
    /// <summary>
    /// Read this (on Update, presumably) to know if/where to rotate the model
    /// </summary>
    public int currentRotationValue { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SelectColor(Color color)
    {
        currentlySelectedColor = color;
        OnPaintBrushSelected?.Invoke(color);
    }

    public void SelectVertexBrush()
    {
        OnVertexBrushSelected?.Invoke();
    }

    public void RotateRight()
    {
        currentRotationValue = -1;
    }
    
    public void RotateLeft()
    {
        currentRotationValue = 1;
    }

    public void StopRotation()
    {
        currentRotationValue = 0;
    }
}