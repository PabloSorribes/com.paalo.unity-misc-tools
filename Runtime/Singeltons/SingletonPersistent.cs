using UnityEngine;

namespace Paalo.UnityMiscTools
{
	/// <summary>
	/// A singleton which is NOT destroyed on scene load, ie. it will live until the application is turned off.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SingletonPersistent<T> : SingletonBase where T : MonoBehaviour
	{
		private static T _instance;
		private static object _lock = new object();

		public static T Instance
		{
			get
			{
				if (ApplicationIsQuitting)
				{
					Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' will not be returned because the application is quitting.");
					return null;
				}

				lock (_lock)
				{
					if (_instance != null)
					{
						return _instance;
					}

					var instances = FindObjectsOfType<T>();

					if (instances.Length > 0)
					{
						if (instances.Length == 1)
						{
							return _instance = instances[0];
						}

						Debug.LogWarning($"There should never be more than one Singleton of type '{typeof(T)}' in the scene, " +
							$"but '{instances.Length}' were found. The first instance found will be used, and all others destroyed.");

						for (var i = 1; i < instances.Length; i++)
						{
							Destroy(instances[i].gameObject);
						}

						return _instance = instances[0];
					}

					GameObject singleton = new GameObject();
					_instance = singleton.AddComponent<T>();
					singleton.name = $"_{typeof(T).ToString()} (Persistent Singleton)";

					//Make persistent
					DontDestroyOnLoad(singleton);

					Debug.Log($"[Singleton] An instance of {typeof(T)} is needed in the scene, so a persistent '{singleton}' was created.");

					return _instance;
				}
			}
		}

		/// <summary>
		/// Cover edge-cases where a GameObject with a Singelton-component already exists in the scene.
		/// </summary>
		protected virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = this as T;
			}
			else
			{
				Destroy(gameObject);
			}
			DontDestroyOnLoad(this.gameObject);
		}
	}
}