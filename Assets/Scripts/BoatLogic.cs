using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), 
    typeof(ConstantForce))]
public class BoatLogic : MonoBehaviour
{
    private Vector3 _movingDirection;
    private int _points;
    private uint _steps;
    private Rigidbody _rigidbody;
    private ConstantForce _constantForce;
    private Vector3 _previousDirection;

    [SerializeField, Range(0.0f, 360.0f)]
    private uint _rayRadius = 16;

    [SerializeField]
    private float Sight = 10.0f;

    [SerializeField, Range(0.0f, 5.0f)]
    private float BoxWeight;
    [SerializeField, Range(0.0f, 5.0f)]
    private float DistanceFactor;
    [SerializeField, Range(0.0f, 5.0f)]
    private float EnemyWeight;
    [SerializeField, Range(0.0f, 5.0f)]
    private float EnemyDistanceFactor;

    [SerializeField]
    private float MovingSpeed;
    [SerializeField]
    private float SearchingSpeed;

    private void Awake()
    {
        _previousDirection = transform.forward;
        _steps = 360 / _rayRadius;
        _rigidbody = GetComponent<Rigidbody>();
        _constantForce = GetComponent<ConstantForce>();
    }

    private void Update()
    {
        Transform selfTransform = transform;
        Vector3 forward = selfTransform.forward;
        Vector3 rayDirection = forward;
        Vector3 selfPosition = selfTransform.position;
        
        Vector3 highestDirection = forward;
        float highestValue = float.MinValue;
        for (int i = 0; i <= _rayRadius; i++)
        {
            Debug.DrawRay(selfPosition, rayDirection * Sight, Color.red);
        
            RaycastHit raycastHit;
            if (Physics.Raycast(selfPosition, rayDirection, out raycastHit, Sight))
            {
                Debug.DrawLine(selfPosition, raycastHit.point, Color.blue);
                float distance = raycastHit.distance;
                float distanceIndex = 1.0f - distance / Sight;
                string type = raycastHit.collider.gameObject.tag;
        
                float value = float.MinValue;
                switch (type)
                {
                    case "Box": value = distanceIndex * DistanceFactor + BoxWeight;
                        break;
                    case "Enemy": value = distance * EnemyDistanceFactor + EnemyWeight;
                        break;
                }
                
                if (value > highestValue)
                {
                    highestValue = value;
                    highestDirection = new Vector3(rayDirection.x, 0.0f, rayDirection.z);
                }
            }
            
            rayDirection = Quaternion.Euler(0, _steps, 0) * rayDirection;
        }
        
        if (Math.Abs(highestValue - float.MinValue) > 0.0005f)
        {
            transform.rotation = Quaternion.LookRotation(highestDirection);
            _rigidbody.velocity = highestDirection * MovingSpeed;
            _previousDirection = highestDirection;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(_previousDirection);
            _rigidbody.velocity = _previousDirection * SearchingSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Box"))
        {
            ++_points;
            Destroy(other.gameObject);
        }
    }
}
