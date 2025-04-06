using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
public class MissingScriptCleaner : MonoBehaviour
{
    [MenuItem("Tools/Clean Missing Scripts")]
    static void CleanMissingScripts()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = 0;
        
        foreach (GameObject obj in allObjects)
        {
            // Check components for missing scripts
            Component[] components = obj.GetComponents<Component>();
            List<int> missingIndices = new List<int>();
            
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    missingIndices.Add(i);
                    count++;
                }
            }
            
            // Remove missing scripts
            if (missingIndices.Count > 0)
            {
                Debug.Log($"Found {missingIndices.Count} missing scripts on {obj.name}");
                SerializedObject serializedObject = new SerializedObject(obj);
                SerializedProperty componentsProperty = serializedObject.FindProperty("m_Component");
                
                for (int i = missingIndices.Count - 1; i >= 0; i--)
                {
                    componentsProperty.DeleteArrayElementAtIndex(missingIndices[i]);
                }
                
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        Debug.Log($"Cleaned up {count} missing script references");
    }
}
#endif 