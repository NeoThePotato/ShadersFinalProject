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
	private static readonly int
		BLIT_TEXTURE = Shader.PropertyToID("_BlitTexture"),
		BLIT_SCALE_BIAS = Shader.PropertyToID("_BlitScaleBiasRt"),
		BLIT_INTENSITY = Shader.PropertyToID("_Intensity"),
		BLIT_COLOR = Shader.PropertyToID("_Color");

	[SerializeField] private Material _brushMaterial;
	[SerializeField] private RenderTexture _colorTex, _heightTex;
	private BrushData _brush;
	private CommandBuffer _cmd;

	public ref BrushData Brush => ref _brush;

	private RenderTexture Target => _brush.ColorMode ? _colorTex : _heightTex;

	private void Awake()
	{
		_brush.Scale = 0.1f;
		_brush.Intensity = 1f;
		_cmd = new();
	}

	private void OnDestroy() => _cmd.Dispose();

	private void Update()
    {
		if (!_brush.PaintingMode)
			return;
		_cmd.Clear();
		_cmd.SetRenderTarget(Target);
		BlitBrush(_cmd, _brush);
		Graphics.ExecuteCommandBuffer(_cmd);
	}

	private void BlitBrush(CommandBuffer cmd, in BrushData brushData)
	{
		_brushMaterial.SetTexture(BLIT_TEXTURE, brushData.texture);
		_brushMaterial.SetVector(BLIT_SCALE_BIAS, brushData.GetScaleBias());
		_brushMaterial.SetFloat(BLIT_INTENSITY, brushData.Intensity);
		_brushMaterial.SetColor(BLIT_COLOR, brushData.Color);
		cmd.Blit(null, Target, _brushMaterial);
	}

	/// <summary>
	/// Contains data on how the brush should render unto the model.
	/// </summary>
	public struct BrushData
	{
		private const uint PAINT = 0, COLOR = 1;
		public static readonly Vector4 INPUT_SCALE_BIAS = new(1f, 1f, 0f, 0f);

		private Color _color;
		private float2 _uv;
		private float _scale;
		private float _intensity;
		public Texture texture;
		private BitArray8 _state;

		/// <summary>
		/// <see cref="UnityEngine.Color"/> to tint <see cref="texture"/> in. Only applicable when in <see cref="ColorMode"/>.
		/// </summary>
		public Color Color { readonly get => ColorMode ? _color : Color.white; set => _color = value; }

		/// <summary>
		/// UV coordinates to render <see cref="texture"/> on.
		/// </summary>
		public float2 UV { readonly get => _uv; set => _uv = value; }

		/// <summary>
		/// How big should <see cref="texture"/> render.
		/// </summary>
		public float Scale { readonly get => _scale; set => _scale = value; }

		/// <summary>
		/// How strong should <see cref="texture"/>'s effect be.
		/// </summary>
		public float Intensity { readonly get => _intensity; set => _intensity = value; }

		/// <summary>
		/// Whether to paint or not.
		/// </summary>
		public bool PaintingMode { readonly get => _state[PAINT]; set => _state[PAINT] = value; }

		/// <summary>
		/// Whether to paint color (<see langword="true"/>) or height (<see langword="false"/>).
		/// </summary>
		public bool ColorMode { readonly get => !_state[COLOR]; set => _state[COLOR] = !value; }
	}
}

public static class BrushExtensions
{
	/// <param name="source">Source <see cref="BrushData"/>.</param>
	/// <returns>ScaleBiasTex that can be fed into <see cref="Blitter.BlitQuad"/>.</returns>
	[BurstCompile]
	public static float4 GetScaleBias(this BrushData source) => new(xy: source.Scale, zw: source.UV - (source.Scale * 0.5f));

	/// <param name="source">Source <see cref="BrushData"/>.</param>
	/// <returns>Transformation matrix that can be fed into <see cref="CommandBuffer.DrawProcedural"/>.</returns>
	public static Matrix4x4 GetMatrix(this BrushData source) => Matrix4x4.TRS(new float3(xy: source.UV, z: 0), Quaternion.identity, new(source.Scale, source.Scale, source.Scale));
}
