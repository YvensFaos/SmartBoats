using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

/// <summary>
/// Script to generate objects in an given area.
/// </summary>
[ExecuteInEditMode]
public class GenerateObjectsInArea : MonoBehaviour
{
    private Bounds _bounds;

    [Header("Objects")]
    [SerializeField, Tooltip("Possible objecst to be created in the area.")]
    private GameObject[] gameObjectToBeCreated;
    [SerializeField, Tooltip("Number of objects to be created.")]
    private uint count;

    [Space(10)]
    [Header("Variation")]
    [SerializeField]
    private Vector3 randomRotationMinimal;
    [SerializeField]
    private Vector3 randomRotationMaximal;

    private void Awake()
    {
        _bounds = GetComponent<Renderer>().bounds;
    }

    /// <summary>
    /// Remove all children objects. Uses DestroyImmediate.
    /// </summary>
    public void RemoveChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    
    /// <summary>
    /// Destroy all objects in the area (that belongs to this script) and creates them again.
    /// The list of newly created objects is returned.
    /// </summary>
    /// <returns></returns>
    public List<GameObject> RegenerateObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        
        List<GameObject> newObjects = new List<GameObject>();
        for (uint i = 0; i < count; i++)
        {
            GameObject created = Instantiate(gameObjectToBeCreated[Random.Range(0, gameObjectToBeCreated.Length)], GetRandomPositionInWorldBounds(), GetRandomRotation());
            created.transform.parent = transform;
            newObjects.Add(created);
        }

        return newObjects;
    }
    
    /// <summary>
    /// Gets a random position delimited by the bounds, using its extends and center.
    /// </summary>
    /// <returns>Returns a random position in the bounds of the area.</returns>
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
    
    /// <summary>
    /// Gets a random rotation (Quaternion) using the randomRotationMinimal and randomRotationMaximal.
    /// </summary>
    /// <returns>Returns a random rotation.</returns>
    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(randomRotationMinimal.x, randomRotationMaximal.x),
            Random.Range(randomRotationMinimal.y, randomRotationMaximal.y),
            Random.Range(randomRotationMinimal.z, randomRotationMaximal.z));
    }
}
