using Paalo.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Paalo.Tools
{
	public class SetAudioClipsUtility : EditorWindow
	{
		#region ToolName and SetupWindow
		private const int menuIndexPosition = PabloSorribesToolsConstants.defaultPaaloMenuIndexPosition;     //To make the menu be at the top of the GameObject-menu and the first option in the hierarchy.
		private const string baseMenuPath = PabloSorribesToolsConstants.defaultPaaloMenuPath;
		private const string rightClickMenuPath = "GameObject/" + baseMenuPath + toolName;
		private const string toolsMenuPath = "Window/" + baseMenuPath + toolName;
		private const string toolName = "Set AudioClips Utility";

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
		#endregion ToolName and SetupWindow

		public string startPath = "Assets/Game/Audio/Source";
		public AudioClip[] audioClips = new AudioClip[0];

		public string textArea = "";
		Vector2 textAreaScroller;

		private void OnGUI()
		{
			GUISection_GetAudioClips();
			EditorGUILayout.Space();
			GUISection_SetAudioClips();
		}

		private void UpdateAudioClips<T>(T[] draggedObjects) where T : Object
		{
			audioClips = draggedObjects as AudioClip[];

			Debug.Log("Dragged Object Array Length: " + audioClips.Length);
			Debug.Log($"Dragged Obj Array Type: {draggedObjects.GetType().FullName}");
			foreach (var draggedObj in draggedObjects)
			{
				Debug.Log($"Dragged Obj Type: {draggedObj.GetType().FullName}");
			}
		}

		private void GUISection_GetAudioClips()
		{
			Color oldGuiColor = GUI.color;
			EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.Space();
			var dragAndDropInfo = new PaaloEditorHelper.DragAndDropAreaInfo("Audio Clips", Color.black, Color.cyan);
			PaaloEditorHelper.DrawDragAndDropArea<AudioClip>(dragAndDropInfo, UpdateAudioClips);
			EditorGUILayout.Space();

			GUI.color = Color.red;
			if (GUILayout.Button("Clear selected AudioClips"))
			{
				//audioClips = null;
				audioClips = new AudioClip[0];
			}
			GUI.color = oldGuiColor;

			EditorGUILayout.Space();

			GUISection_ShowSelectedAudioClipsTextArea();

			GUI.color = oldGuiColor;
			EditorGUILayout.EndVertical();
		}

		private void GUISection_ShowSelectedAudioClipsTextArea()
		{
			string selectedClipName = "";
			if (audioClips != null)
			{
				foreach (var clip in audioClips)
				{
					selectedClipName += $"{clip.name}\n";
				}
			}

			//Make text area showing what clips have been selected already
			EditorGUILayout.LabelField($"{audioClips?.Length} Selected Audio Clips:", EditorStyles.boldLabel);

			if (audioClips != null && audioClips.Length > 0)
			{
				textAreaScroller = EditorGUILayout.BeginScrollView(textAreaScroller);
				textArea = EditorGUILayout.TextArea(selectedClipName, GUILayout.Height(position.height - 150));
				EditorGUILayout.EndScrollView();
			}
		}

		private void GUISection_SetAudioClips()
		{
			//Disable button if no clips are selected.
			GUI.enabled = audioClips.Length > 0 ? true : false;

			Color oldGuiColor = GUI.color;

			var selectedObjects = Selection.gameObjects;
			EditorGUILayout.BeginVertical(GUI.skin.box);

			GUI.color = Color.cyan;
			if (GUILayout.Button($"Apply AudioClips to {selectedObjects.Length} selected GameObjects!"))
			{
				SetAudioClips(audioClips, selectedObjects);
			}
			EditorGUILayout.EndVertical();

			GUI.color = oldGuiColor;
		}

		private static void SetAudioClips(AudioClip[] audioClips, GameObject[] gameObjects)
		{
			List<GameObject> gameObjectsList = new List<GameObject>(gameObjects);
			gameObjectsList.Sort(new SceneGraphOrderComparer());
			
			for (int i = 0; i < gameObjectsList.ToArray().Length; i++)
			{
				int clipsIndex = i;

				if (i > audioClips.Length - 1)
				{
					clipsIndex -= audioClips.Length;
					Debug.Log("You have less AudioClips than selected GameObjects - Starting the audio clip iteration again.");
				}

				var currentObject = gameObjectsList[i];
				Undo.RecordObject(currentObject, $"Set AudioClip '{audioClips[clipsIndex]}' to {currentObject.name}");

				currentObject.GetComponent<AudioSource>().clip = audioClips[clipsIndex];
			}

			Debug.Log($"Applied AudioClips to {gameObjectsList.ToArray().Length} GameObjects with AudioSources.");
		}




		private void GUISection_OldGetClipsButton()
		{
			EditorGUILayout.Space();
			startPath = EditorGUILayout.TextField("Starting Path: ", startPath);
			EditorGUILayout.Space();

			GUI.color = Color.cyan;
			if (GUILayout.Button($"Gimme them Audio Clips!"))
			{
				string directoryToBrowse = PaaloEditorHelper.BrowseToFolder(startPath);
				if (string.IsNullOrEmpty(directoryToBrowse))
				{
					return;
				}

				audioClips = PaaloEditorHelper.GetAllAssetsOfTypeInDirectory<AudioClip>(directoryToBrowse);
			}
		}
	}
}
