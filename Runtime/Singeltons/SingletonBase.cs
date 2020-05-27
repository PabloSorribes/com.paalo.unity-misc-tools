using UnityEngine;

namespace Paalo.UnityMiscTools
{
	public abstract class SingletonBase : MonoBehaviour
	{
		public static bool ApplicationIsQuitting { get; private set; } = false;

		private void OnApplicationQuit()
		{
			ApplicationIsQuitting = true;
		}
	}
}
