using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Paalo.Utils
{
	/// <summary>
	/// Contains helper methods for common (and tedious) operations when writing IMGUI-code.
	/// </summary>
	public static class PaaloEditorHelper
	{
		private static readonly float originalLabelWidth = EditorGUIUtility.labelWidth;
		private static readonly int originalIndentation = EditorGUI.indentLevel;

		/// <summary>
		/// A GUIStyle made of a box which is shaped like line.
		/// </summary>
		/// <returns></returns>
		public static GUIStyle GetSeparatorMarginStyle()
		{
			var separatorMarginStyle = new GUIStyle(GUI.skin.box) { margin = new RectOffset(0, 0, 10, 10) };
			return separatorMarginStyle;
		}

		/// <summary>
		/// Draws a box/rect which looks like a separation line. Useful for sectioning off different parts of your GUI.
		/// <para></para>
		/// Expands its width by default to match the width of the GUI/Editor Window.
		/// </summary>
		public static void MakeSeparatorLine(bool expandWidth = true)
		{
			var separatorMarginStyle = GetSeparatorMarginStyle();
			GUILayout.Box("", separatorMarginStyle, GUILayout.Height(1f), GUILayout.ExpandWidth(expandWidth));
		}

		public static void IncrementIndentation()
		{
			EditorGUI.indentLevel++;
		}
		public static void DecrementIndentation()
		{
			EditorGUI.indentLevel--;
		}

		public static void SetIndentation(int desiredIndent)
		{
			EditorGUI.indentLevel = desiredIndent;
		}

		public static void ResetIndentation()
		{
			EditorGUI.indentLevel = originalIndentation;
		}

		/// <summary>
		/// Useful for making a Bool Toggle (<see cref="EditorGUILayout.Toggle(bool, GUILayoutOption[])"/>) have its checkbox at the far right of an editor window. 
		/// </summary>
		/// <param name="containingBoxForLabel"></param>
		/// <param name="offsetFromRightEdge"></param>
		public static void SetWideLabelWidth(Rect containingBoxForLabel, float offsetFromRightEdge = 30f)
		{
			//Make all the longer folder paths visible, as well as make all checkboxes align along the right side of the box.
			var checkBoxPadding = offsetFromRightEdge;
			var alignedCheckBoxesLabelWidth = containingBoxForLabel.width - checkBoxPadding;
			EditorGUIUtility.labelWidth = alignedCheckBoxesLabelWidth;
		}

		/// <summary>
		/// Reset the <see cref="EditorGUIUtility.labelWidth"/> to the default value.
		/// </summary>
		public static void ResetLabelWidth()
		{
			EditorGUIUtility.labelWidth = originalLabelWidth;
		}

		/// <summary>
		/// Useful for setting the <see cref="EditorGUIUtility.labelWidth"/> to the width of your desired label.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static float CalculateLabelWidth(GUIContent label, float padding = 0f)
		{
			float labelWidth = GUI.skin.label.CalcSize(label).x + padding;
			return labelWidth;
		}

		/// <summary>
		/// Useful for setting the <see cref="EditorGUIUtility.labelWidth"/> to the width of your desired label.
		/// </summary>
		/// <param name="label"></param>
		/// <returns></returns>
		public static float CalculateLabelWidth(string label, float padding = 0f)
		{
			return CalculateLabelWidth(new GUIContent(label), padding);
		}


        #region ProgressBar Handling
        //Source: https://github.com/Unity-Technologies/VFXToolbox/blob/master/Editor/Utility/VFXToolboxGUIUtility.cs

        private static double s_LastProgressBarTime;

        /// <summary>
        /// Displays a progress bar with delay and optional cancel button
        /// </summary>
        /// <param name="title">title of the window</param>
        /// <param name="message">message</param>
        /// <param name="progress">progress</param>
        /// <param name="delay">minimum delay before displaying window</param>
        /// <param name="cancelable">is the window cancellable?</param>
        /// <returns>true if cancelled, false otherwise</returns>
        public static bool DisplayProgressBar(string title, string message, float progress, float delay = 0.0f, bool cancelable = false)
        {
            if (s_LastProgressBarTime < 0.0)
                s_LastProgressBarTime = EditorApplication.timeSinceStartup;

            if (EditorApplication.timeSinceStartup - s_LastProgressBarTime > delay)
            {
                if (cancelable)
                {
                    return EditorUtility.DisplayCancelableProgressBar(title, message, progress);
                }
                else
                {
                    EditorUtility.DisplayProgressBar(title, message, progress);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Clears the current progressbar
        /// </summary>
        public static void ClearProgressBar()
        {
            s_LastProgressBarTime = -1.0;
            EditorUtility.ClearProgressBar();
        }

        #endregion

        #region Other GUI Utils
        //Source: https://github.com/Unity-Technologies/VFXToolbox/blob/master/Editor/Utility/VFXToolboxGUIUtility.cs
        public static void GUIRotatedLabel(Rect position, string label, float angle, GUIStyle style)
        {
            var matrix = GUI.matrix;
            var rect = new Rect(position.x - 10f, position.y, position.width, position.height);
            GUIUtility.RotateAroundPivot(angle, rect.center);
            GUI.Label(rect, label, style);
            GUI.matrix = matrix;
        }
		#endregion

		#region Draw Drag And Drop Area

		/// <summary>
		/// Example method on how to call the '<see cref="DrawDragAndDropArea{T}(DragAndDropAreaInfo, System.Action{T[]})"/>' from OnGUI()
		/// </summary>
		private static void HowToDrawDragAndDropArea()
		{
			PaaloEditorHelper.DrawDragAndDropArea<AudioClip>(new DragAndDropAreaInfo("Audio Clips"), OnDragAndDropPerformed_CallbackExample);
		}

		/// <summary>
		/// Example method on how to handle the objects that are received through the OnPerformedDragCallback in the '<see cref="DrawDragAndDropArea{T}(DragAndDropAreaInfo, System.Action{T[]})"/>'-method.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="draggedObjects"></param>
		private static void OnDragAndDropPerformed_CallbackExample<T>(T[] draggedObjects) where T : UnityEngine.Object
		{
			var myObjects = draggedObjects as AudioClip[];

			Debug.Log("Dragged Object Array Length: " + myObjects.Length);
			Debug.Log($"Dragged Obj Array Type: {draggedObjects.GetType().FullName}");
			foreach (var draggedObj in draggedObjects)
			{
				Debug.Log($"Dragged Obj Type: {draggedObj.GetType().FullName}");
			}
		}

		/// <summary>
		/// Draws a Drag and Drop Area and allows you to send in a method which receives an array of the objects that were dragged into the area.
		/// <para></para>
		/// The caller method needs to receive a generic type "T" and then cast it to its desired type itself.
		/// <para></para>
		/// Example implementation in OnGUI: <see cref="OnDragAndDropPerformed_CallbackExample{T}(T[])"/>
		/// </summary>
		/// <seealso cref="HowToDrawDragAndDropArea"/>
		/// <typeparam name="T">The object type you want the '<paramref name="OnPerformedDragCallback"/>'-method to handle.</typeparam>
		/// <param name="dragAreaInfo"></param>
		/// <returns></returns>
		public static void DrawDragAndDropArea<T>(DragAndDropAreaInfo dragAreaInfo, System.Action<T[]> OnPerformedDragCallback = null) where T : UnityEngine.Object
		{
			//Change color and create Drag Area
			Color originalGUIColor = GUI.color;
			GUI.color = dragAreaInfo.outlineColor;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			GUI.color = dragAreaInfo.backgroundColor;
			var dragArea = GUILayoutUtility.GetRect(dragAreaInfo.dragAreaWidth, dragAreaInfo.dragAreaHeight, GUILayout.ExpandWidth(true));
			GUI.Box(dragArea, dragAreaInfo.DragAreaText);

			//See if the current Editor Event is a DragAndDrop event.
			var anEvent = Event.current;
			switch (anEvent.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!dragArea.Contains(anEvent.mousePosition))
					{
						//Early Out in case the drop is made outside the drag area.
						break;
					}

					//Change mouse cursor icon to the "Copy" icon
					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					//If mouse is released 
					if (anEvent.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						var draggedTypeObjectsArray = GetDraggedObjects<T>();
						OnPerformedDragCallback?.Invoke(draggedTypeObjectsArray);
					}

					Event.current.Use();
					break;
			}

			EditorGUILayout.EndVertical();
			GUI.color = originalGUIColor;
		}

		private static T[] GetDraggedObjects<T>() where T : UnityEngine.Object
		{
			List<T> draggedTypeObjects = new List<T>();

			foreach (var draggedObject in DragAndDrop.objectReferences)
			{
				//A "DefaultAsset" is a folder in the Unity Editor.
				if (draggedObject is DefaultAsset)
				{
					string folderPath = AssetDatabase.GetAssetPath(draggedObject);
					var assetsInDraggedFolders = GetAllAssetsOfTypeInDirectory<T>(folderPath);
					foreach (var asset in assetsInDraggedFolders)
					{
						if (draggedTypeObjects.Contains(asset as T))
						{
							//Debug.Log($"Asset in Dragged Folder exists already: '{asset.name}'");
							continue;
						}

						draggedTypeObjects.Add(asset as T);
					}
					//Go to next index in the "DragAndDrop.objectReferences"
					continue;
				}

				//Dragged asset is a "normal" asset, ie. not a Folder.
				T draggedAsset = draggedObject as T;
				if (draggedAsset == null || draggedTypeObjects.Contains(draggedAsset as T))
				{
					//Debug.Log($"Dragged Asset is not casteable to the type you wanted or already exists in the selection list: '{draggedAsset.name}'");
					continue;
				}

				//Debug.Log($"Asset of type '{draggedAsset.GetType().FullName}' dragged. Asset Name: '{draggedAsset.name}'");
				draggedTypeObjects.Add(draggedAsset as T);
			}
			return draggedTypeObjects.ToArray();
		}

		/// <summary>
		/// Use this class to create some values for the DragAndDrop-area that you want to create.
		/// </summary>
		public class DragAndDropAreaInfo
		{
			public string DragAreaText
			{
				get => $"Drag {draggedObjectTypeName} or a folder containing some {draggedObjectTypeName} here!";
				//private set => DragAreaText = value;
			}

			public string draggedObjectTypeName = "AudioClips";
			public float dragAreaWidth = 0f;
			public float dragAreaHeight = 35f;

			public Color outlineColor = Color.black;
			public Color backgroundColor = Color.yellow;

			public DragAndDropAreaInfo(string draggedObjectTypeName)
			{
				this.draggedObjectTypeName = draggedObjectTypeName;
			}

			public DragAndDropAreaInfo(string draggedObjectTypeName, Color outlineColor, Color backgroundColor, float dragAreaWidth = 0f, float dragAreaHeight = 35f)
			{
				this.draggedObjectTypeName = draggedObjectTypeName;
				this.outlineColor = outlineColor;
				this.backgroundColor = backgroundColor;
				this.dragAreaWidth = dragAreaWidth;
				this.dragAreaHeight = dragAreaHeight;
			}
		}
		#endregion

		public static T[] GetAllAssetsOfTypeInDirectory<T>(string path) where T : UnityEngine.Object
		{
			List<T> assetsToGet = new List<T>();

			string absolutePath = $"{Application.dataPath}/{path.Remove(0, 7)}";
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

				T assetOfType = AssetDatabase.LoadAssetAtPath<T>(localPath);
				if (assetOfType != null)
					assetsToGet.Add(assetOfType);
			}
			return assetsToGet.ToArray();
		}

		/// <summary>
		/// Open a folder panel which let's you browse to any folder on disk.
		/// </summary>
		/// <param name="startPath"></param>
		/// <returns>The path to the folder that the user selected</returns>
		public static string BrowseToFolder(string startPath = "Assets")
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

		/// <summary>
		/// Returns the assets that are selected in the Project Tab.
		/// </summary>
		/// <returns></returns>
		public static Object[] GetSelectedAssetsInProjectView()
		{
			Object[] selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
			return selectedAssets;
		}

		/// <summary>
		/// Returns a filtered selection of the selected assets which have the type '<typeparamref name="T"/>' on them (eg. a prefab, but not an actual script asset).
		/// </summary>
		/// <returns></returns>
		public static Object[] GetSelectedAssetsInProjectView<T>() where T : UnityEngine.Object
		{
			Object[] selectedAssets = Selection.GetFiltered(typeof(T), SelectionMode.Assets);
			return selectedAssets;
		}
	}
}
