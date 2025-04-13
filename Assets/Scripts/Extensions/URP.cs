using UnityEngine.Rendering;

namespace Extensions
{
	static class URP
	{
		public static VC GetVolumeComponent<VC>(this VolumeProfile profile) where VC : VolumeComponent
		{
			return profile.components.Find(vc => vc is VC) as VC;
		}
	}
}