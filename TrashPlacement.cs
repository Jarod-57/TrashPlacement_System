using System.Collections.Generic;
using UnityEngine;

public class TrashPlacement : MonoBehaviour
{
    private GameObject player;

    public List<GameObject> trashPrefabs;
    public float renderDistance = 50f;
    public LayerMask terrainLayerMask;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        ManageTrashRendering();
    }

    private void ManageTrashRendering()
    {
        foreach (Transform trash in transform)
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, trash.position);
            bool shouldBeActive = distanceToPlayer <= renderDistance;
            if (trash.gameObject.activeSelf != shouldBeActive) trash.gameObject.SetActive(shouldBeActive);
        }
    }
}
