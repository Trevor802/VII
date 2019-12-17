using System;
using UnityEngine;
using UnityEditor;

public class ClearPositionOffset : EditorWindow
{
	[MenuItem("Tools/Clear Position Offset")]
	static void CreateReplaceWithPrefab()
	{
		EditorWindow.GetWindow<ClearPositionOffset>();
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Clear Offset"))
		{
			var selection = Selection.gameObjects;

			for (var i = selection.Length - 1; i >= 1; --i)
			{
				var selected = selection[i];

                selected.transform.localPosition -= selection[0].transform.localPosition;
                Undo.RecordObject(selected.transform, "Previous transform");
			}
            selection[0].transform.localPosition = Vector3.zero;
		}

		GUI.enabled = false;
		EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
	}
}