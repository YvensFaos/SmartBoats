using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField]
    private GameObject worldObject;

    private Bounds _worldBounds;

    [SerializeField]
    private GameObject boatObject;
    [SerializeField]
    private GameObject boxObject;
    [SerializeField]
    private GameObject enemyObject;

    [SerializeField, Range(0.1f, 10.0f)]
    private float timerGenerateBoxes;

    [SerializeField, Range(1, 10)]
    private uint maximumBoxesAtOnce;

    private float _accumulator;

    [SerializeField]
    private uint initialBoats;
    
    [SerializeField]
    private uint initialEnemies;

    private void Awake()
    {
        _worldBounds = worldObject.GetComponent<Renderer>().bounds;
        _accumulator = 0.0f;
    }

    private void Start()
    {
        for (uint i = 0; i < initialBoats; i++)
        {
            Instantiate(boatObject, GetRandomPositionInWorldBounds(), Quaternion.identity);
        }
        
        for (uint i = 0; i < initialEnemies; i++)
        {
            //Instantiate(enemyObject, GetRandomPositionInWorldBounds(), Quaternion.identity);
        }
    }

    private void Update()
    {
        _accumulator += Time.deltaTime;

        if (!(_accumulator >= timerGenerateBoxes)) return;
        
        _accumulator = 0.0f;
        uint boxes = (uint) Random.Range(1, maximumBoxesAtOnce);
        for (uint i = 0; i < boxes; i++)
        {
            Instantiate(boxObject, GetRandomPositionInWorldBounds(), Quaternion.identity);
        }
    }
    
    private Vector3 GetRandomPositionInWorldBounds()
    {
        Vector3 extents = _worldBounds.extents;
        Vector3 center = _worldBounds.center;
        return new Vector3(
            Random.Range(-extents.x, extents.x) + center.x,
            Random.Range(-extents.y, extents.y) + center.y,
            Random.Range(-extents.z, extents.z) + center.z
            );
    }
    
}
