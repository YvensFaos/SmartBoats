using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GenerateObjectsInArea : MonoBehaviour
{
    private Bounds _bounds;

    [SerializeField]
    private uint count;
    [SerializeField]
    private GameObject gameObjectToBeCreated;

    [Space(10)]
    [SerializeField]
    private Vector3 randomRotationMinimal;
    [SerializeField]
    private Vector3 randomRotationMaximal;

    private void Awake()
    {
        _bounds = GetComponent<Renderer>().bounds;
    }

    public void RemoveChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    
    public List<GameObject> RegenerateObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        
        List<GameObject> newObjects = new List<GameObject>();
        for (uint i = 0; i < count; i++)
        {
            GameObject created = Instantiate(gameObjectToBeCreated, GetRandomPositionInWorldBounds(), GetRandomRotation());
            created.transform.parent = transform;
            newObjects.Add(created);
        }

        return newObjects;
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

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(randomRotationMinimal.x, randomRotationMaximal.x),
            Random.Range(randomRotationMinimal.y, randomRotationMaximal.y),
            Random.Range(randomRotationMinimal.z, randomRotationMaximal.z));
    }
}
