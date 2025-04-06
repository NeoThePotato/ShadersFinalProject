using UnityEngine;

public class SphereComparisonSystem : MonoBehaviour
{
    [Header("Compute Shader")]
    [SerializeField] private ComputeShader comparisonShader;

    [Header("Textures")]
    [SerializeField] private RenderTexture playerColor;
    [SerializeField] private RenderTexture playerHeight;
    [SerializeField] private Texture targetColor;
    [SerializeField] private Texture targetHeight;

    [Header("Settings")]
    [Range(0.1f, 10f)]
    [SerializeField] private float difficultyMultiplier = 1.0f;

    [Header("Output")]
    [SerializeField] private int computedScore;

    private ComputeBuffer _resultBuffer;
    private int _kernel;
    private int _resolution;

    private void Start()
    {
        Comparison();
    }

    public void Comparison()
    {
        _resolution = playerColor.width;

        int totalPixels = _resolution * _resolution;
        _resultBuffer = new ComputeBuffer(totalPixels, sizeof(float));

        _kernel = comparisonShader.FindKernel("CSMain");

        comparisonShader.SetTexture(_kernel, "_PlayerColorTex", playerColor);
        comparisonShader.SetTexture(_kernel, "_TargetColorTex", targetColor);
        comparisonShader.SetTexture(_kernel, "_PlayerHeightTex", playerHeight);
        comparisonShader.SetTexture(_kernel, "_TargetHeightTex", targetHeight);

        comparisonShader.SetFloat("_DifficultyMultiplier", difficultyMultiplier);
        comparisonShader.SetInt("_Resolution", _resolution);
        comparisonShader.SetBuffer(_kernel, "ResultBuffer", _resultBuffer);
    }

    public void CompareTextures()
    {
        if (comparisonShader == null || _resultBuffer == null)
        {
            Debug.LogWarning("Compute Shader / Result Buffer not set up!");
            return;
        }

        comparisonShader.Dispatch(_kernel, Mathf.CeilToInt(_resolution / 8.0f), Mathf.CeilToInt(_resolution / 8.0f), 1);

        float[] results = new float[_resolution * _resolution];
        _resultBuffer.GetData(results);

        float totalDifference = 0f;
        foreach (float diff in results)
            totalDifference += diff;

        float maxDifference = _resolution * _resolution;
        float similarity = 1f - (totalDifference / maxDifference);

        computedScore = Mathf.RoundToInt(similarity * 100f);
        GameManager.Instance.SendMessage("EvaluateAccuracy", computedScore);
    }
    
    public void CompareNow() // Call this CompareNow() method to trigger the comparison!
    {
        CompareTextures();
    }

    private void OnDestroy()
    {
        _resultBuffer?.Dispose();
    }
    
    public void SetDifficultyMultiplier(float value)
    {
        difficultyMultiplier = value;
        comparisonShader.SetFloat("_DifficultyMultiplier", difficultyMultiplier);
    }
    
    public void SetTargetTextures(Texture color, Texture height)
    {
        targetColor = color;
        targetHeight = height;
        comparisonShader.SetTexture(_kernel, "_TargetColorTex", targetColor);
        comparisonShader.SetTexture(_kernel, "_TargetHeightTex", targetHeight);
    }

    public void SetPlayerTextures(RenderTexture color, RenderTexture height)
    {
        playerColor = color;
        playerHeight = height;
        comparisonShader.SetTexture(_kernel, "_PlayerColorTex", playerColor);
        comparisonShader.SetTexture(_kernel, "_PlayerHeightTex", playerHeight);
    }

    public int GetScore() => computedScore;
}