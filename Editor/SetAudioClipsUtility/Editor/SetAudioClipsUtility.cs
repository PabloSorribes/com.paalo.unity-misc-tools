//using Paalo.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Paalo.Tools
{
	public class SetAudioClipsUtility : EditorWindow
	{
		private const int menuIndexPosition = PabloSorribesToolsConstants.defaultPaaloMenuIndexPosition;     //To make the menu be at the top of the GameObject-menu and the first option in the hierarchy.
		private const string baseMenuPath = PabloSorribesToolsConstants.defaultPaaloMenuPath;
		private const string rightClickMenuPath = "GameObject/" + baseMenuPath + toolName;
		private const string toolsMenuPath = "Window/" + baseMenuPath + toolName;
		private const string toolName = "Set AudioClips Utility";

		public string startPath = "Assets/Game/Audio/Source";
		public AudioClip[] audioClips = null;

		public string textArea = "";
		Vector2 textAreaScroller;

		[MenuItem(rightClickMenuPath, false, menuIndexPosition)]
		public static void RightClickMenu()
		{
			SetupWindow();
		}

		[MenuItem(toolsMenuPath, false, menuIndexPosition)]
		public static void ToolsMenu()
		{
			SetupWindow();
		}

		public static void SetupWindow()
		{
			var window = GetWindow<SetAudioClipsUtility>(true, toolName, true);
			window.minSize = new Vector2(300, 200);
			window.maxSize = new Vector2(window.minSize.x + 100, window.minSize.y + 100);
		}

		private void OnGUI()
		{
			GUISection_GetAudioClips();
			EditorGUILayout.Space();
			GUISection_SetAudioClips();
		}

		private void GUISection_GetAudioClips()
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.Space();
			startPath = EditorGUILayout.TextField("Starting Path: ", startPath);
			EditorGUILayout.Space();

			Color oldGuiColor = GUI.color;

			GUI.color = Color.cyan;
			if (GUILayout.Button($"Gimme them Audio Clips!"))
			{
				string directoryToBrowse = BrowseToFolder(startPath);
				if (string.IsNullOrEmpty(directoryToBrowse))
				{
					return;
				}

				audioClips = GetAllAssetsOfTypeInDirectory<AudioClip>(directoryToBrowse);
			}

			EditorGUILayout.Space();

			GUI.color = Color.red;
			if (GUILayout.Button("Clear selected AudioClips"))
			{
				audioClips = null;
			}
			GUI.color = oldGuiColor;

			EditorGUILayout.Space();

			GUISection_SelectedAudioClipsTextArea();

			GUI.color = oldGuiColor;
			EditorGUILayout.EndVertical();
		}

		private void GUISection_SelectedAudioClipsTextArea()
		{
			string selectedClips = "";
			if (audioClips != null)
			{
				foreach (var clip in audioClips)
				{
					selectedClips += $"{clip.name}\n";
				}
			}

			//Make text area showing what clips have been selected already
			EditorGUILayout.LabelField("Selected Audio Clips:", EditorStyles.boldLabel);
			textAreaScroller = EditorGUILayout.BeginScrollView(textAreaScroller);
			textArea = EditorGUILayout.TextArea(selectedClips, GUILayout.Height(position.height - 150));
			EditorGUILayout.EndScrollView();
		}

		private void GUISection_SetAudioClips()
		{
			Color oldGuiColor = GUI.color;

			var selectedObjects = Selection.gameObjects;
			EditorGUILayout.BeginVertical(GUI.skin.box);

			GUI.color = Color.cyan;
			if (GUILayout.Button($"Set AudioClips to {selectedObjects.Length} selected GameObjects!"))
			{
				SetAudioClips(audioClips, selectedObjects);
			}
			EditorGUILayout.EndVertical();

			GUI.color = oldGuiColor;
		}

		private static string BrowseToFolder(string startPath = "Assets")
		{
			string folderPath = EditorUtility.OpenFolderPanel("Browse to Folder", startPath, "");
			if (string.IsNullOrEmpty(folderPath))
			{
				//Cancelled the OpenFolderPanel window.
				return string.Empty;
			}

			if (folderPath.Contains(Application.dataPath))
			{
				folderPath = folderPath.Replace(Application.dataPath, string.Empty).Trim();
				folderPath = folderPath.TrimStart('/', '\\');
				folderPath = $"Assets/{folderPath}";
			}
			return folderPath;
		}

		public static T[] GetAllAssetsOfTypeInDirectory<T>(string path) where T : UnityEngine.Object
		{
			List<T> assetsToGet = new List<T>();

			string absolutePath = Application.dataPath + "/" + path.Remove(0, 7);
			string[] fileEntries = Directory.GetFiles(absolutePath);

			foreach (string fileName in fileEntries)
			{
				string sanitizedFileName = fileName.Replace('\\', '/');
				int index = sanitizedFileName.LastIndexOf('/');
				string localPath = path;
				if (index > 0)
				{
					localPath += sanitizedFileName.Substring(index);
				}
				//Paalo.Utils.EditorGUIHelper.DisplayProgressBar("Image Sequencer", "Discovering Assets in folder...", (float)i / count);

				T assetOfType = AssetDatabase.LoadAssetAtPath<T>(localPath);
				if (assetOfType != null)
					assetsToGet.Add(assetOfType);
			}
			//Paalo.Utils.EditorGUIHelper.ClearProgressBar();
			return assetsToGet.ToArray();
		}

		private static void SetAudioClips(AudioClip[] audioClips, GameObject[] gameObjects)
		{
			List<GameObject> gameObjectsList = new List<GameObject>(gameObjects);
			gameObjectsList.Sort(new SceneGraphOrderComparer());
			
			for (int i = 0; i < gameObjectsList.ToArray().Length; i++)
			{
				if (i > audioClips.Length - 1)
				{
					Debug.Log("You have less AudioClips than selected GameObjects.");
					return;
				}

				var currentObject = gameObjectsList[i];
				Undo.RecordObject(currentObject, $"Set AudioClip '{audioClips[i]}' to {currentObject.name}");

				currentObject.GetComponent<AudioSource>().clip = audioClips[i];
			}

			Debug.Log($"Set the AudioClips for {audioClips.Length} GameObjects");
		}
	}
}
