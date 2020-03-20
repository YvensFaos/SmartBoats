using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class AgentLogic : MonoBehaviour
{
    private Vector3 _movingDirection;
    private uint _steps;
    private Rigidbody _rigidbody;
    private Vector3 _previousDirection;
    
    protected float points;

    [Header("Genes")]
    [SerializeField, Range(0.0f, 360.0f)]
    private uint _rayRadius = 16;
    [SerializeField]
    private float Sight = 10.0f;
    [SerializeField]
    private float MovingSpeed;
    [SerializeField]
    private float SearchingSpeed;
    [SerializeField]
    private Vector2 RandomDirectionValue;

    [Space(20)]
    [Header("Weights")]
    [SerializeField, Range(-10.0f, 10.0f)]
    private float BoxWeight;
    [SerializeField, Range(-10.0f, 10.0f)]
    private float DistanceFactor;
    [SerializeField, Range(-10.0f, 10.0f)]
    private float BoatWeight;
    [SerializeField, Range(-10.0f, 10.0f)]
    private float BoatDistanceFactor;
    [SerializeField, Range(-10.0f, 10.0f)]
    private float EnemyWeight;
    [SerializeField, Range(-10.0f, 10.0f)]
    private float EnemyDistanceFactor;

    [Header("Debug & Help")] 
    [SerializeField]
    private Color visionColor;
    [SerializeField]
    private Color foundColor;
    [SerializeField]
    private Color directionColor;
    [SerializeField] 
    private bool debug;
    
    private void Awake()
    {
        _previousDirection = transform.forward;
        _steps = 360 / _rayRadius;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Birth()
    {
        
    }

    private void Update()
    {
        Transform selfTransform = transform;
        Vector3 forward = Vector3.forward;
        Vector3 rayDirection = forward;
        Vector3 selfPosition = selfTransform.position;
        
        Vector3 highestDirection = forward;
        float highestValue = float.MinValue;
        for (int i = 0; i <= _rayRadius; i++)
        {
            if (debug)
            {
                Debug.DrawRay(selfPosition, rayDirection * Sight, visionColor);
            }
    
            float value = Random.Range(RandomDirectionValue.x, RandomDirectionValue.y);
            if (Physics.Raycast(selfPosition, rayDirection, out var raycastHit, Sight))
            {
                if (debug)
                {
                    Debug.DrawLine(selfPosition, raycastHit.point, foundColor);
                }
                    
                float distance = raycastHit.distance;
                float distanceIndex = 1.0f - distance / Sight;
                string type = raycastHit.collider.gameObject.tag;
        
                switch (type)
                {
                    case "Box": value = distanceIndex * DistanceFactor + BoxWeight;
                        break;
                    case "Boat": value = distance * BoatDistanceFactor + BoatWeight;
                        break;
                    case "Enemy": value = distance * EnemyDistanceFactor + EnemyWeight;
                        break;
                }
            }
            if (value > highestValue)
            {
                highestValue = value;
                highestDirection = new Vector3(rayDirection.x, 0.0f, rayDirection.z);
            }
            
            rayDirection = Quaternion.Euler(0, _steps, 0) * rayDirection;
        }
        
        if (Mathf.Abs(highestValue - float.MinValue) > 0.0005f)
        {
            transform.rotation = Quaternion.LookRotation(highestDirection);
            _rigidbody.velocity = highestDirection * MovingSpeed;
            _previousDirection = highestDirection;
            
            if (debug)
            {
                Debug.DrawRay(selfPosition, highestDirection * (Sight * 1.5f), directionColor);
            }
        }
    }
    
    public float GetPoints()
    {
        return points;
    }
}
