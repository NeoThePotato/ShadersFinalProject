using System;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

/// <summary>
/// Handles the comparison of the player's textures with the reference textures.
/// </summary>
public class SphereComparisonSystem : MonoBehaviour
{
    [Header("Compute Shader")]
    [SerializeField] private ComputeShader comparisonShader;

    [Header("Textures")]
    [SerializeField] private RenderTexture playerColor;
    [SerializeField] private RenderTexture playerHeight;

    [Header("Settings")]
    [Range(0.1f, 10f)]
    [SerializeField] private float difficultyMultiplier = 1.0f;
    
    [Header("UI")]
	[SerializeField] private Material referenceMat;
	[SerializeField] private Slider accuracyBar;
    [SerializeField] private TMPro.TMP_Text scoreText;
    [SerializeField] private UnityEngine.GameObject successWindow;
    
    [Header("Target Flags")]
    [SerializeField] private Reference[] references;

    private Reference currentReference;
	private ComputeBuffer _resultBuffer;
    private int _kernel;
    private int _resolution;
	private int computedScore;

	private Texture ReferenceColor => currentReference.color;
	private Texture ReferenceHeight => currentReference.height;

	private void Awake()
    {
        _kernel = comparisonShader.FindKernel("CSMain");
        NextFlag();
	}

	private void OnDestroy()
	{
		_resultBuffer?.Dispose();
	}

	/// <summary>
	/// Uses <see cref="comparisonShader"/> to compare the player's textures with <see cref="currentReference"/>.
	/// </summary>
	public void Compare()
    {
        Comparison();
        CompareTextures();

		void Comparison()
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
			comparisonShader.SetTexture(_kernel, "TargetColor", ReferenceColor);
			comparisonShader.SetTexture(_kernel, "PlayerHeight", playerHeight);
			comparisonShader.SetTexture(_kernel, "TargetHeight", ReferenceHeight);

			comparisonShader.SetFloat("DifficultyScale", difficultyMultiplier);
			comparisonShader.SetInt("TextureResolution", _resolution);
			comparisonShader.SetBuffer(_kernel, "ResultBuffer", _resultBuffer);
		}

		void CompareTextures()
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
				successWindow.SetActive(true);
			}
		}
	}
    
    public void SetDifficultyMultiplier(float value)
    {
        difficultyMultiplier = value;
        comparisonShader.SetFloat("DifficultyScale", difficultyMultiplier);
    }
    
    private void SetReferenceTextures(Reference reference)
    {
        currentReference = reference;
		referenceMat.SetTexture("_Color", ReferenceColor);
		referenceMat.SetTexture("_Height", ReferenceHeight);
		comparisonShader.SetTexture(_kernel, "TargetColor", ReferenceColor);
        comparisonShader.SetTexture(_kernel, "TargetHeight", ReferenceHeight);
    }

	private void ClearRenderTexture(RenderTexture rt)
    {
        if (rt == null) return;
        Graphics.SetRenderTarget(rt);
        GL.Clear(true, true, Color.clear);
        Graphics.SetRenderTarget(null);
    }

	/// <summary>
	/// Resets the player textures and changes the reference.
	/// </summary>
    public void NextFlag()
    {
        SetReferenceTextures(references.GetRandom());
        ClearRenderTexture(playerColor);
        ClearRenderTexture(playerHeight);
    }

    public int GetScore() => computedScore;

    [Serializable]
    private struct Reference : IEquatable<Reference>
    {
        public Texture color;
        public Texture height;

		public readonly bool Equals(Reference other) => color.Equals(other.color) && height.Equals(other.height);
	}
}