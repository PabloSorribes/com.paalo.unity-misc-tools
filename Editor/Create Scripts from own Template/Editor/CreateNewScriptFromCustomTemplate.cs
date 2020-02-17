//Source 1: https://forum.unity.com/threads/how-to-create-your-own-c-script-template.459977/#post-3533591 //Original code
//Source 2: https://forum.unity.com/threads/how-to-create-your-own-c-script-template.459977/#post-3539041 //Micro Optimization
//Source 3: https://forum.unity.com/threads/how-to-create-your-own-c-script-template.459977/#post-3569909 //What path to send in when the script is part of a Package

using UnityEditor;
using UnityEngine;

public class CreateNewScriptFromCustomTemplate
{
	private static string pathToYourScriptTemplate = "com.paalo.unity-misc-tools/Editor/Create Scripts from own Template/ScriptTemplates/PaaloScriptTemplate.cs.txt";

	[MenuItem(itemName: "Assets/Create/Paalo/Create New Script from Custom Template", isValidateFunction: false, priority: 51)]
	public static void CreateScriptFromTemplate()
	{
		int dialogResult = EditorUtility.DisplayDialogComplex(
			"Create Script from Template",
			"Choose the default template or specify another template?",
			"Use Default",
			"Cancel",
			"Choose another file as template");

		//Check if we're in the Package Development-project or in a project that is using this script as a Package.
		Object scriptTemplateAsset = AssetDatabase.LoadAssetAtPath($"Packages/{pathToYourScriptTemplate}", typeof(Object));
		if (scriptTemplateAsset == null)
		{
			scriptTemplateAsset = AssetDatabase.LoadAssetAtPath($"Assets/{pathToYourScriptTemplate}", typeof(Object));
		}
		pathToYourScriptTemplate = AssetDatabase.GetAssetPath(scriptTemplateAsset);


		switch (dialogResult)
		{
			// Default template (ok)
			case 0:
				ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToYourScriptTemplate, "PaaloBehaviour.cs");
				break;

			// Cancel.
			case 1:
				break;

			// Choose another template (alternative)
			case 2:
				string templateAlternative = SelectScriptTemplateAsset();

				if (string.IsNullOrEmpty(templateAlternative))
				{
					break;
				}
				ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templateAlternative, "PaaloBehaviour.cs");
				break;

			default:
				Debug.LogError("Unrecognized option.");
				break;
		}
	}

	public static string SelectScriptTemplateAsset()
	{
		string path = EditorUtility.OpenFilePanel("SelectScriptTemplateAsset", "Assets", "");

		if (path.Contains(Application.dataPath))
		{
			path = path.Replace(Application.dataPath, string.Empty).Trim();
			path = path.TrimStart('/', '\\');
			path = $"Assets/{path}";
		}

		return path;
	}
}