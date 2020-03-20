using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class GenerateObjectsInArea : MonoBehaviour
{
    private Bounds _bounds;

    [SerializeField]
    private uint count;
    [SerializeField]
    private GameObject gameObjectToBeCreated;

    private void Awake()
    {
        _bounds = GetComponent<Renderer>().bounds;
    }

    void Start()
    {
        if (!Application.isPlaying)
        {
            for (uint i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild((int)i).gameObject);
            }
            for (uint i = 0; i < count; i++)
            {
                GameObject created = Instantiate(gameObjectToBeCreated, GetRandomPositionInWorldBounds(), Quaternion.Euler(Random.Range(-20.0f, 20.0f), Random.Range(-89.0f, 89.0f), Random.Range(-10.0f, -10.0f)));
                created.transform.parent = transform;
            }
        }
    }
    
    private Vector3 GetRandomPositionInWorldBounds()
    {
        Vector3 extents = _bounds.extents;
        Vector3 center = _bounds.center;
        return new Vector3(
            Random.Range(-extents.x, extents.x) + center.x,
            Random.Range(-extents.y, extents.y) + center.y,
            Random.Range(-extents.z, extents.z) + center.z
        );
    }
}
