using System;
using UnityEngine;
using Random = UnityEngine.Random;

public struct AgentData
{
    public int _steps;
    public int _rayRadius;
    public float Sight;
    public float MovingSpeed;
    public Vector2 RandomDirectionValue;
    public float BoxWeight;
    public float DistanceFactor;
    public float BoatWeight;
    public float BoatDistanceFactor;
    public float EnemyWeight;
    public float EnemyDistanceFactor;

    public AgentData(int steps, int rayRadius, float sight, float movingSpeed, Vector2 randomDirectionValue, float boxWeight, float distanceFactor, float boatWeight, float boatDistanceFactor, float enemyWeight, float enemyDistanceFactor)
    {
        _steps = steps;
        _rayRadius = rayRadius;
        Sight = sight;
        MovingSpeed = movingSpeed;
        RandomDirectionValue = randomDirectionValue;
        BoxWeight = boxWeight;
        DistanceFactor = distanceFactor;
        BoatWeight = boatWeight;
        BoatDistanceFactor = boatDistanceFactor;
        EnemyWeight = enemyWeight;
        EnemyDistanceFactor = enemyDistanceFactor;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class AgentLogic : MonoBehaviour, IComparable
{
    private Vector3 _movingDirection;
    private int _steps;
    private Rigidbody _rigidbody;
    
    protected float points;

    private bool _isAwake;

    [Header("Genes")]
    [SerializeField, Range(0.0f, 360.0f)]
    private int _rayRadius = 16;
    [SerializeField]
    private float Sight = 10.0f;
    [SerializeField]
    private float MovingSpeed;
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
        Initiate();
    }

    public void Initiate()
    {
        _steps = 360 / _rayRadius;
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void Birth(AgentData parent)
    {
        _steps = parent._steps;
        _rayRadius = parent._rayRadius;
        Sight = parent.Sight;
        MovingSpeed = parent.MovingSpeed;
        RandomDirectionValue = parent.RandomDirectionValue;
        BoxWeight = parent.BoxWeight;
        DistanceFactor = parent.DistanceFactor;
        BoatWeight = parent.BoatWeight;
        BoatDistanceFactor = parent.BoatDistanceFactor;
        EnemyWeight = parent.EnemyWeight;
        EnemyDistanceFactor = parent.EnemyDistanceFactor;
    }

    public void Mutate(float mutationFactor, float mutationChance)
    {
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            _steps += (int) Random.Range(-mutationFactor, +mutationFactor);
            _steps = (int) Mathf.Max(_steps, 1.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            _rayRadius += (int) Random.Range(-mutationFactor, +mutationFactor);
            _rayRadius = (int) Mathf.Max(_rayRadius, 1.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            Sight += Random.Range(-mutationFactor, +mutationFactor);
            Sight = Mathf.Max(Sight, 1.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            MovingSpeed += Random.Range(-mutationFactor, +mutationFactor);
            MovingSpeed = Mathf.Max(MovingSpeed, 0.1f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            RandomDirectionValue.x += Random.Range(-mutationFactor, +mutationFactor);
            RandomDirectionValue.y += Random.Range(-mutationFactor, +mutationFactor);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            BoxWeight += Random.Range(-mutationFactor, +mutationFactor);
            BoxWeight = Mathf.Max(Mathf.Min(BoxWeight, 10.0f), -10.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            DistanceFactor += Random.Range(-mutationFactor, +mutationFactor);
            DistanceFactor = Mathf.Max(Mathf.Min(DistanceFactor, 10.0f), -10.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            BoatWeight += Random.Range(-mutationFactor, +mutationFactor);
            BoatWeight = Mathf.Max(Mathf.Min(BoatWeight, 10.0f), -10.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            BoatDistanceFactor +=  Random.Range(-mutationFactor, +mutationFactor);
            BoatDistanceFactor = Mathf.Max(Mathf.Min(BoatDistanceFactor, 10.0f), -10.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            EnemyWeight += Random.Range(-mutationFactor, +mutationFactor);
            EnemyWeight = Mathf.Max(Mathf.Min(EnemyWeight, 10.0f), -10.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            EnemyDistanceFactor += Random.Range(-mutationFactor, +mutationFactor);
            EnemyDistanceFactor = Mathf.Max(Mathf.Min(EnemyDistanceFactor, 10.0f), -10.0f);
        }
    }

    private void Update()
    {
        if (_isAwake)
        {
            Act();    
        }
    }

    private void Act()
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
                    case "Box":
                        value = distanceIndex * DistanceFactor + BoxWeight;
                        break;
                    case "Boat":
                        value = distance * BoatDistanceFactor + BoatWeight;
                        break;
                    case "Enemy":
                        value = distance * EnemyDistanceFactor + EnemyWeight;
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

            if (debug)
            {
                Debug.DrawRay(selfPosition, highestDirection * (Sight * 1.5f), directionColor);
            }
        }
    }

    public void AwakeUp()
    {
        _isAwake = true;
    }

    public void Sleep()
    {
        _isAwake = false;
        _rigidbody.velocity = Vector3.zero;
    }

    public float GetPoints()
    {
        return points;
    }
    
    public int CompareTo(object obj) {
        if (obj == null) return 1;
        
        AgentLogic otherAgent = obj as AgentLogic;
        if (otherAgent != null)
        {
            return otherAgent.GetPoints().CompareTo(GetPoints());
        } 
        else
        {
            throw new ArgumentException("Object is not an AgentLogic");
        }
    }

    public AgentData GetData()
    {
        return new AgentData(_steps, _rayRadius, Sight, MovingSpeed, RandomDirectionValue, BoxWeight, DistanceFactor, BoatWeight, BoatDistanceFactor, EnemyWeight,  EnemyDistanceFactor);
    }
}
