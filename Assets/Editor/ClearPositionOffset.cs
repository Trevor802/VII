using System;
using UnityEngine;
using UnityEditor;

public class ClearPositionOffset : EditorWindow
{
    [SerializeField] private float m_SnapValue = 0.5f;

    [MenuItem("Tools/Clear Position Offset")]
	static void CreateClearPositionOffset()
	{
		EditorWindow.GetWindow<ClearPositionOffset>();
	}

	private void OnGUI()
	{
        m_SnapValue = EditorGUILayout.FloatField("Snap Value", m_SnapValue);
        if (GUILayout.Button("Snap to Value"))
		{
            foreach (var gameObject in Selection.gameObjects)
            {
                var selection = gameObject.GetComponentsInChildren<Transform>();
                foreach (var item in selection)
                {
                    Undo.RecordObject(item.transform, "Previous transform");
                    Vector3 itemPosition = item.transform.position;
                    itemPosition.x = Mathf.Round(itemPosition.x / m_SnapValue) * m_SnapValue;
                    itemPosition.y = Mathf.Round(itemPosition.y / m_SnapValue) * m_SnapValue;
                    itemPosition.z = Mathf.Round(itemPosition.z / m_SnapValue) * m_SnapValue;
                    item.transform.position = itemPosition;
                }
            }
		}
        if (GUILayout.Button("Snap to Grid"))
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                var selection = gameObject.GetComponentsInChildren<Transform>();
                Vector3 offset = new Vector3(
                    selection[1].transform.position.x - Mathf.Floor(selection[1].transform.position.x),
                    selection[1].transform.position.y - Mathf.Floor(selection[1].transform.position.y),
                    selection[1].transform.position.z - Mathf.Floor(selection[1].transform.position.z));
                foreach (var item in selection)
                {
                    Undo.RecordObject(item.transform, "Previous transform");
                    if (item.gameObject == gameObject) continue;
                    item.transform.position += offset;
                }
            }
        }
        GUI.enabled = false;
		EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
	}
}