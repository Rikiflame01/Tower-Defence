using UnityEditor;
using UnityEngine;

public class PrefabSpawner : EditorWindow
{
    private GameObject prefab;
    private GameObject referenceObject;
    private Vector3 spawnPosition;
    private float offset = 1.0f; // Default offset for relative positioning
    private GameObject spawnedObject;

    [MenuItem("Tools/Prefab Spawner")]
    public static void ShowWindow()
    {
        GetWindow<PrefabSpawner>("Prefab Spawner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Spawner", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true);
        referenceObject = (GameObject)EditorGUILayout.ObjectField("Reference Object (Optional)", referenceObject, typeof(GameObject), true);

        GUILayout.Space(10);
        GUILayout.Label("Spawn Options", EditorStyles.boldLabel);

        spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", spawnPosition);
        offset = EditorGUILayout.FloatField("Offset", offset);

        if (GUILayout.Button("Spawn Prefab"))
        {
            SpawnPrefabAtPosition(spawnPosition);
        }

        GUILayout.Space(10);
        GUILayout.Label("Relative Spawn Options", EditorStyles.boldLabel);

        if (referenceObject != null)
        {
            if (GUILayout.Button("Spawn Above"))
            {
                SpawnPrefabRelativeToReference(Vector3.up);
            }

            if (GUILayout.Button("Spawn Below"))
            {
                SpawnPrefabRelativeToReference(Vector3.down);
            }

            if (GUILayout.Button("Spawn to the Left"))
            {
                SpawnPrefabRelativeToReference(Vector3.left);
            }

            if (GUILayout.Button("Spawn to the Right"))
            {
                SpawnPrefabRelativeToReference(Vector3.right);
            }

            if (GUILayout.Button("Spawn in Front"))
            {
                SpawnPrefabRelativeToReference(Vector3.forward);
            }

            if (GUILayout.Button("Spawn Behind"))
            {
                SpawnPrefabRelativeToReference(Vector3.back);
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("Adjust Spawned Object Position", EditorStyles.boldLabel);

        if (spawnedObject != null)
        {
            spawnPosition = EditorGUILayout.Vector3Field("New Position", spawnedObject.transform.position);

            if (GUILayout.Button("Update Position"))
            {
                UpdateSpawnedObjectPosition(spawnPosition);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No spawned object to adjust.");
        }
    }

    private void SpawnPrefabAtPosition(Vector3 position)
    {
        if (prefab != null)
        {
            spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            spawnedObject.transform.position = position;
            Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawn Prefab");
        }
        else
        {
            Debug.LogWarning("No prefab selected to spawn.");
        }
    }

    private void SpawnPrefabRelativeToReference(Vector3 direction)
    {
        if (prefab != null && referenceObject != null)
        {
            spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            spawnedObject.transform.position = referenceObject.transform.position + direction * offset;
            Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawn Prefab Relative");
        }
        else
        {
            if (prefab == null)
            {
                Debug.LogWarning("No prefab selected to spawn.");
            }

            if (referenceObject == null)
            {
                Debug.LogWarning("No reference object selected.");
            }
        }
    }

    private void UpdateSpawnedObjectPosition(Vector3 position)
    {
        if (spawnedObject != null)
        {
            Undo.RecordObject(spawnedObject.transform, "Update Spawned Object Position");
            spawnedObject.transform.position = position;
        }
        else
        {
            Debug.LogWarning("No spawned object to update.");
        }
    }
}
