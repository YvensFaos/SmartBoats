using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

/// <summary>
/// Script to generate objects in an given area.
/// </summary>
[ExecuteInEditMode]
public class GenerateObjectsInArea : MonoBehaviour
{

    [Header("Objects")]
    [SerializeField, Tooltip("Area to used where the objects will be created.")]
    private BoxCollider area;
    [SerializeField, Tooltip("Possible objects to be created in the area.")]
    private GameObject[] gameObjectToBeCreated;

    [SerializeField, Tooltip("Number of objects to be created.")]
    private uint count;

    [Space(10)] 
    [Header("Variation")] 
    [SerializeField]
    private Vector3 randomRotationMinimal;
    [SerializeField] 
    private Vector3 randomRotationMaximal;

    /// <summary>
    /// Remove all children objects. Uses DestroyImmediate.
    /// </summary>
    public void RemoveChildren()
    {
        for (var i = transform.childCount - 1; i >= 0; --i)
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
        for (var i = transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        var newObjects = new List<GameObject>();
        for (uint i = 0; i < count; i++)
        {
            var created = Instantiate(gameObjectToBeCreated[Random.Range(0, gameObjectToBeCreated.Length)],
                GetRandomPositionInWorldBounds(), GetRandomRotation());
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
        var randomPoint = new Vector3(
            Random.Range(area.bounds.min.x, area.bounds.max.x),
            transform.position.y,
            Random.Range(area.bounds.min.z, area.bounds.max.z)
        );
        return randomPoint;
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