using UnityEditor;
using UnityEngine;

namespace Paalo.Utils
{
	/// <summary>
	/// Contains helper methods for common (and tedious) operations when writing IMGUI-code.
	/// </summary>
	public static class EditorGUIHelper
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
    }
}
