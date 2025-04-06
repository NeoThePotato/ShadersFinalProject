using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Burst;
using static BrushController;

/// <summary>
/// Handles the rendering of <see cref="Brush"/> unto the <see cref="_colorTex"/> or <see cref="_heightTex"/>;
/// </summary>
public class BrushController : MonoBehaviour
{
	[SerializeField] private Texture _defaultTexture;
	[SerializeField] private RenderTexture _colorTex, _heightTex;
	private BrushData _brush;
	private CommandBuffer _cmd;

	public ref BrushData Brush => ref _brush;

	private RenderTexture Target => _brush.Color ? _colorTex : _heightTex;

	private void Awake()
	{
		_brush.texture = _defaultTexture;
		_brush.Scale = 0.1f;
		_brush.Intensity = 1f;
		_cmd = new();
	}

	void LateUpdate()
    {
		if (!_brush.Painting)
			return;
		_cmd.Clear();
		_cmd.SetRenderTarget(Target);
		Blitter.BlitQuad(_cmd, _brush.texture, BrushData.INPUT_SCALE_BIAS, _brush.GetScaleBias(), 0, true);
		Graphics.ExecuteCommandBuffer(_cmd);
	}

	/// <summary>
	/// Contains data on how the brush should render unto the model.
	/// </summary>
	public struct BrushData
	{
		private const uint PAINT = 0, COLOR = 1;
		public static readonly Vector4 INPUT_SCALE_BIAS = new(1f, 1f, 0f, 0f);

		private float4 _uvScaleIntensity;
		public Texture texture;
		private BitArray8 _state;

		/// <summary>
		/// UV coordinates to render <see cref="texture"/> on.
		/// </summary>
		public float2 UV { readonly get => _uvScaleIntensity.xy; set => _uvScaleIntensity.xy = value; }

		/// <summary>
		/// How big should <see cref="texture"/> render.
		/// </summary>
		public float Scale { readonly get => _uvScaleIntensity.z; set => _uvScaleIntensity.z = value; }

		/// <summary>
		/// How strong should <see cref="texture"/>'s effect be.
		/// </summary>
		public float Intensity { readonly get => _uvScaleIntensity.w; set => _uvScaleIntensity.w = value; }

		/// <summary>
		/// Whether to paint or not.
		/// </summary>
		public bool Painting { readonly get => _state[PAINT]; set => _state[PAINT] = value; }

		/// <summary>
		/// Whether to paint color (<see langword="true"/>) or height (<see langword="false"/>).
		/// </summary>
		public bool Color { readonly get => !_state[COLOR]; set => _state[COLOR] = !value; }
	}
}

public static class BrushExtensions
{
	/// <param name="source">Source <see cref="BrushData"/>.</param>
	/// <returns>ScaleBiasTex that can be fed into <see cref="Blitter.BlitQuad"/>.</returns>
	[BurstCompile]
	public static float4 GetScaleBias(this BrushData source) => new(xy: source.Scale, zw: source.UV - (source.Scale * 0.5f));
}
