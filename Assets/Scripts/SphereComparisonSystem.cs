using UnityEngine;
using UnityEngine.UI;

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
    
    [Header("UI")]
    [SerializeField] private Slider accuracyBar;
    [SerializeField] private TMPro.TMP_Text scoreText;
    
    [Header("Target Flags")]
    [SerializeField] private Texture[] targetColorTextures;
    [SerializeField] private Texture[] targetHeightTextures;
    [SerializeField] private GameObject[] targetRefFlags;
    private int currentFlagIndex = 0;

    private ComputeBuffer _resultBuffer;
    private int _kernel;
    private int _resolution;
    
    private void Awake()
    {
        _kernel = comparisonShader.FindKernel("CSMain");
    }

    public void Comparison()
    {
        if (playerColor == null || playerHeight == null)
        {
            Debug.LogError("Nothing to compare!");
            return;
        }
        
        // _kernel = comparisonShader.FindKernel("CSMain");
        
        _resolution = playerColor.width;

        int totalPixels = _resolution * _resolution;
        
        if (_resultBuffer != null)
            _resultBuffer.Dispose();
        _resultBuffer = new ComputeBuffer(totalPixels, sizeof(float));

        comparisonShader.SetTexture(_kernel, "PlayerColor", playerColor);
        comparisonShader.SetTexture(_kernel, "TargetColor", targetColor);
        comparisonShader.SetTexture(_kernel, "PlayerHeight", playerHeight);
        comparisonShader.SetTexture(_kernel, "TargetHeight", targetHeight);

        comparisonShader.SetFloat("DifficultyScale", difficultyMultiplier);
        comparisonShader.SetInt("TextureResolution", _resolution);
        comparisonShader.SetBuffer(_kernel, "ResultBuffer", _resultBuffer);
    }

    public void CompareTextures()
    {
        if (comparisonShader == null || _resultBuffer == null)
        {
            Debug.LogWarning("Compute Shader / Result Buffer not set up!");
            return;
        }

        int threadGroups = Mathf.CeilToInt(_resolution / 8.0f);
        comparisonShader.Dispatch(_kernel, threadGroups, threadGroups, 1);

        float[] results = new float[_resolution * _resolution];
        _resultBuffer.GetData(results);

        float totalDifference = 0f;
        foreach (float diff in results)
            totalDifference += diff;

        float maxDifference = _resolution * _resolution;
        float similarity = 1f - (totalDifference / maxDifference);

        computedScore = Mathf.RoundToInt(similarity * 100f);
        
        if (accuracyBar != null)
        {
            accuracyBar.value = computedScore;
        }
        
        if (scoreText != null)
        {
            string colorHex = "#000000";

            if (computedScore < 20)
                colorHex = "#FF4D4D";
            else if (computedScore < 40)
                colorHex = "#FFD93D";
            else
                colorHex = "#4CAF50";
            
            scoreText.text = $"<color={colorHex}>{computedScore}%</color>";
        }

        GameManager.Instance.SendMessage("EvaluateAccuracy", computedScore);
        
        if (computedScore >= 40) // Advance to next flag when the score is above 40%
        {
            NextFlag();
        }
    }
    
    public void CompareNow() // Call this CompareNow() method to trigger the comparison!
    {
        Comparison();
        CompareTextures();
    }

    private void OnDestroy()
    {
        _resultBuffer?.Dispose();
    }
    
    public void SetDifficultyMultiplier(float value)
    {
        difficultyMultiplier = value;
        comparisonShader.SetFloat("DifficultyScale", difficultyMultiplier);
    }
    
    public void SetTargetTextures(Texture color, Texture height)
    {
        targetColor = color;
        targetHeight = height;
        comparisonShader.SetTexture(_kernel, "TargetColor", targetColor);
        comparisonShader.SetTexture(_kernel, "TargetHeight", targetHeight);
    }

    public void SetPlayerTextures(RenderTexture color, RenderTexture height)
    {
        playerColor = color;
        playerHeight = height;
        comparisonShader.SetTexture(_kernel, "PlayerColor", playerColor);
        comparisonShader.SetTexture(_kernel, "PlayerHeight", playerHeight);
    }

    private void NextFlag()
    {
        if (targetRefFlags == null || targetRefFlags.Length == 0)
            return;

        if (currentFlagIndex < targetRefFlags.Length)
            targetRefFlags[currentFlagIndex].SetActive(false);

        currentFlagIndex++;

        if (currentFlagIndex < targetRefFlags.Length)
        {
            targetRefFlags[currentFlagIndex].SetActive(true);

            if (currentFlagIndex < targetColorTextures.Length && currentFlagIndex < targetHeightTextures.Length)
            {
                targetColor = targetColorTextures[currentFlagIndex];
                targetHeight = targetHeightTextures[currentFlagIndex];
                
                SetTargetTextures(targetColor, targetHeight);
                
                comparisonShader.SetTexture(_kernel, "TargetColor", targetColor);
                comparisonShader.SetTexture(_kernel, "TargetHeight", targetHeight);
            }
            else
            {
                Debug.LogWarning("No matching textures for the next flag!");
            }
        }
        else
        {
            Debug.Log("All flags completed! Well Done!");
        }
    }

    public int GetScore() => computedScore;
}