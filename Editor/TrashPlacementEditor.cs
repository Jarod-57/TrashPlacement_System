using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrashPlacement))]
public class TrashPlacementEditor : Editor
{
    private void OnSceneGUI()
    {
        TrashPlacement trashPlacement = (TrashPlacement)target;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;
        if (e.shift && e.type == EventType.MouseDown && e.button == 0)
        {
            PlaceTrash(trashPlacement);
        }
    }

    private void PlaceTrash(TrashPlacement trashPlacement)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, trashPlacement.terrainLayerMask))
        {
            GameObject trashPrefab = trashPlacement.trashPrefabs[Random.Range(0, trashPlacement.trashPrefabs.Count)];
            GameObject trashInstance = (GameObject)PrefabUtility.InstantiatePrefab(trashPrefab);
            trashInstance.transform.position = hit.point + Vector3.up * (trashInstance.GetComponent<Collider>().bounds.extents.y);
            trashInstance.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            trashInstance.transform.SetParent(trashPlacement.transform);

            Undo.RegisterCreatedObjectUndo(trashInstance, "Place Trash");
            EditorUtility.SetDirty(trashPlacement);
            
            if (IsTrashStuck(trashInstance))
            {
                RepositionTrash(trashInstance, hit.point);
            }
        }
    }

    private bool IsTrashStuck(GameObject trashInstance)
    {
        Collider collider = trashInstance.GetComponent<Collider>();
        Vector3 boundsCenter = collider.bounds.center;
        Vector3 boundsExtents = collider.bounds.extents;
        float checkDistance = boundsExtents.magnitude;

        RaycastHit hitInfo;
        if (Physics.Raycast(boundsCenter, Vector3.down, out hitInfo, checkDistance) || 
            Physics.Raycast(boundsCenter, Vector3.up, out hitInfo, checkDistance) ||
            Physics.Raycast(boundsCenter, Vector3.left, out hitInfo, checkDistance) ||
            Physics.Raycast(boundsCenter, Vector3.right, out hitInfo, checkDistance) ||
            Physics.Raycast(boundsCenter, Vector3.forward, out hitInfo, checkDistance) ||
            Physics.Raycast(boundsCenter, Vector3.back, out hitInfo, checkDistance))
        {
            return true; 
        }

        return false; 
    }

    private void RepositionTrash(GameObject trashInstance, Vector3 originalPosition)
    {
        Collider collider = trashInstance.GetComponent<Collider>();
        Vector3 boundsExtents = collider.bounds.extents;
        Vector3 newPosition = originalPosition + Vector3.up * (boundsExtents.y + 0.1f);

        trashInstance.transform.position = newPosition;

        if (IsTrashStuck(trashInstance))
        {
            Undo.DestroyObjectImmediate(trashInstance);
            Debug.LogWarning("Bag was stuck so we deleted it .");
        }
    }
}
