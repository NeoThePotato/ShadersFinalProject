using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace Extensions
{
	static class DOTween
	{
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveInTargetLocalSpace(this UnityEngine.Transform transform, UnityEngine.Transform target, Vector3 targetLocalEndPosition, float duration)
		{
			var t = DG.Tweening.DOTween.To(
				() => transform.position - target.position,
				x => transform.position = x + target.position,
				targetLocalEndPosition,
				duration);
			t.SetTarget(transform);
			return t;
		}
	}
}