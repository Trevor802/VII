using System;
using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefab : EditorWindow
{
	[SerializeField] private GameObject prefab;
	[SerializeField] private GameObject m_RespawnTilePrefab;

    [MenuItem("Tools/Replace With Prefab")]
	static void CreateReplaceWithPrefab()
	{
		EditorWindow.GetWindow<ReplaceWithPrefab>();
	}

	private void OnGUI()
	{
		prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        m_RespawnTilePrefab = (GameObject)EditorGUILayout.ObjectField("RespawnTilePrefabs",
            m_RespawnTilePrefab, typeof(GameObject), false);

        if (GUILayout.Button("Replace"))
		{
			var selection = Selection.gameObjects;

			for (var i = selection.Length - 1; i >= 0; --i)
			{
				var selected = selection[i];
				var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
				GameObject newObject;

				if (prefabType == PrefabAssetType.Regular)
				{
					newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				}
				else
				{
					newObject = Instantiate(prefab);
					newObject.name = prefab.name;
				}

				if (newObject == null)
				{
					Debug.LogError("Error instantiating prefab");
					break;
				}

				Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
				newObject.transform.parent = selected.transform.parent;
				newObject.transform.localPosition = selected.transform.localPosition;
				newObject.transform.localRotation = selected.transform.localRotation;
				newObject.transform.localScale = selected.transform.localScale;
				newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
				Undo.DestroyObjectImmediate(selected);
			}
		}

        if (GUILayout.Button("Replace Checkpoint"))
        {
            var selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i)
            {
                var selected = selection[i];
                var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
                var respawnTilePrefabType = PrefabUtility.GetPrefabAssetType(m_RespawnTilePrefab);
                GameObject newObject;
                GameObject newRespawnTileObject;

                if (prefabType == PrefabAssetType.Regular)
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name;
                }

                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                if (respawnTilePrefabType == PrefabAssetType.Regular)
                {
                    newRespawnTileObject = (GameObject)PrefabUtility.InstantiatePrefab(m_RespawnTilePrefab);
                }
                else
                {
                    newRespawnTileObject = Instantiate(m_RespawnTilePrefab);
                    newRespawnTileObject.name = m_RespawnTilePrefab.name;
                }

                if (newRespawnTileObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                Undo.RegisterCreatedObjectUndo(newRespawnTileObject, "Replace With Prefabs");
                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = selected.transform.localPosition;
                newObject.transform.localRotation = selected.transform.localRotation;
                newObject.transform.localScale = selected.transform.localScale;
                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());

                newRespawnTileObject.transform.parent = selected.transform.parent;
                newRespawnTileObject.transform.position = selected.GetComponent<Checkpoint>().respawnObject.transform.position - VII.GameData.PLAYER_RESPAWN_POSITION_OFFSET;
                newRespawnTileObject.transform.localRotation = selected.GetComponent<Checkpoint>().respawnObject.transform.localRotation;
                newRespawnTileObject.transform.localScale = selected.GetComponent<Checkpoint>().respawnObject.transform.localScale;
                newRespawnTileObject.transform.SetSiblingIndex(selected.GetComponent<Checkpoint>().respawnObject.transform.GetSiblingIndex());
                newObject.GetComponent<Checkpoint>().respawnObject = newRespawnTileObject;
                Undo.DestroyObjectImmediate(selected);
            }
        }

        GUI.enabled = false;
		EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
	}
}