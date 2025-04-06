using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Burst;
using static BrushController;

public class BrushController : MonoBehaviour
{
	[SerializeField] private Texture _defaultTexture;
	[SerializeField] private RenderTexture _colorTex, _heightTex;
	private BrushData _brush;
	private CommandBuffer _cmd;

	public bool IsColorMode => _brush.Color;

	public bool IsPainting { get => _brush.Painting; private set => _brush.Painting = value; }

	public ref BrushData Brush => ref _brush;

	private RenderTexture Target => IsColorMode ? _colorTex : _heightTex;

	private void Awake()
	{
		_brush.texture = _defaultTexture;
		_brush.scale = 0.1f;
		_brush.intensity = 1f;
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

	public struct BrushData
	{
		public static readonly Vector4 INPUT_SCALE_BIAS = new(1f, 1f, 0f, 0f);

		public Texture texture;
		public float2 uv;
		public float scale;
		public float intensity;
		private BitArray8 _state;

		public bool Painting { readonly get => _state[0]; set => _state[0] = value; }

		public bool Color { readonly get => !_state[1]; set => _state[1] = !value; }
	}
}

public static class BrushExtensions
{
	[BurstCompile]
	public static float4 GetScaleBias(this BrushData brushData) => new(xy: brushData.scale, zw: brushData.uv - (brushData.scale * 0.5f));
}
