using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

struct AgentDirection : IComparable
{
    public Vector3 Direction { get; }
    public float weight;

    public AgentDirection(Vector3 direction, float weight)
    {
        Direction = direction;
        this.weight = weight;
    }
    
    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        
        AgentDirection otherAgent = (AgentDirection)obj;
        return otherAgent.weight.CompareTo(weight);
    }
}

[Serializable]
public struct AgentData
{
    public int steps;
    public int rayRadius;
    public float sight;
    public float movingSpeed;
    public Vector2 randomDirectionValue;
    public float boxWeight;
    public float distanceFactor;
    public float boatWeight;
    public float boatDistanceFactor;
    public float enemyWeight;
    public float enemyDistanceFactor;

    public AgentData(int steps, int rayRadius, float sight, float movingSpeed, Vector2 randomDirectionValue, float boxWeight, float distanceFactor, float boatWeight, float boatDistanceFactor, float enemyWeight, float enemyDistanceFactor)
    {
        this.steps = steps;
        this.rayRadius = rayRadius;
        this.sight = sight;
        this.movingSpeed = movingSpeed;
        this.randomDirectionValue = randomDirectionValue;
        this.boxWeight = boxWeight;
        this.distanceFactor = distanceFactor;
        this.boatWeight = boatWeight;
        this.boatDistanceFactor = boatDistanceFactor;
        this.enemyWeight = enemyWeight;
        this.enemyDistanceFactor = enemyDistanceFactor;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class AgentLogic : MonoBehaviour, IComparable
{
    private Vector3 _movingDirection;
    private int _steps;
    private Rigidbody _rigidbody;
    
    [SerializeField]
    protected float points;

    private bool _isAwake;

    [Header("Genes")]
    [SerializeField, Range(0.0f, 360.0f)]
    private int rayRadius = 16;
    [SerializeField]
    private float sight = 10.0f;
    [SerializeField]
    private float movingSpeed;
    [SerializeField]
    private Vector2 randomDirectionValue;

    [Space(20)]
    [Header("Weights")]
    [SerializeField]
    private float boxWeight;
    [SerializeField]
    private float distanceFactor;
    [SerializeField]
    private float boatWeight;
    [SerializeField]
    private float boatDistanceFactor;
    [SerializeField]
    private float enemyWeight;
    [SerializeField]
    private float enemyDistanceFactor;

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

    private void Initiate()
    {
        points = 0;
        _steps = 360 / rayRadius;
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void Birth(AgentData parent)
    {
        _steps = parent.steps;
        rayRadius = parent.rayRadius;
        sight = parent.sight;
        movingSpeed = parent.movingSpeed;
        randomDirectionValue = parent.randomDirectionValue;
        boxWeight = parent.boxWeight;
        distanceFactor = parent.distanceFactor;
        boatWeight = parent.boatWeight;
        boatDistanceFactor = parent.boatDistanceFactor;
        enemyWeight = parent.enemyWeight;
        enemyDistanceFactor = parent.enemyDistanceFactor;
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
            rayRadius += (int) Random.Range(-mutationFactor * 0.75f, +mutationFactor * 2.0f);
            rayRadius = (int) Mathf.Max(rayRadius, 1.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            sight += Random.Range(-mutationFactor, +mutationFactor);
            sight = Mathf.Max(sight, 0.1f);
            movingSpeed -= sight * 0.0125f;
            movingSpeed = Mathf.Max(movingSpeed, 1.0f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            movingSpeed += Random.Range(-mutationFactor * 0.75f, +mutationFactor * 2.0f);
            movingSpeed = Mathf.Max(movingSpeed, 1.0f);
            sight -= movingSpeed * 0.0125f;
            sight = Mathf.Max(sight, 0.1f);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            randomDirectionValue.x += Random.Range(-mutationFactor, +mutationFactor);
            randomDirectionValue.y += Random.Range(-mutationFactor, +mutationFactor);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            boxWeight += Random.Range(-mutationFactor, +mutationFactor);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            distanceFactor += Random.Range(-mutationFactor, +mutationFactor);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            boatWeight += Random.Range(-mutationFactor, +mutationFactor);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            boatDistanceFactor +=  Random.Range(-mutationFactor, +mutationFactor);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            enemyWeight += Random.Range(-mutationFactor, +mutationFactor);
        }
        if (Random.Range(0.0f, 100.0f) <= mutationChance)
        {
            enemyDistanceFactor += Random.Range(-mutationFactor, +mutationFactor);
        }
    }

    private void Update()
    {
        if (_isAwake)
        {
            Act();    
        }
    }

    /// <summary>
    /// Calculate the best direction to move using the Agent properties.
    /// The agent shoots a ray in a area on front of 
    /// </summary>
    private void Act()
    {
        Transform selfTransform = transform;
        Vector3 forward = selfTransform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        Vector3 rayDirection = forward;
        Vector3 selfPosition = selfTransform.position;

        //Initiate the rayDirection on the opposite side of the spectrum
        rayDirection = Quaternion.Euler(0, -1.0f * _steps * (rayRadius / 2.0f), 0) * rayDirection;
        
        List<AgentDirection> directions = new List<AgentDirection>();
        for (int i = 0; i <= rayRadius; i++)
        {
            //Add the new calculatedAgentDirection looking at the rayDirection
            directions.Add(CalculateAgentDirection(selfPosition, rayDirection));
            
            //Rotate the rayDirection by _steps every iteration through the entire rayRadius
            rayDirection = Quaternion.Euler(0, _steps, 0) * rayDirection;
        }
        directions.Add(CalculateAgentDirection(selfPosition, forward, 1.5f));

        directions.Sort();
        //There is a 15% chance of using the second best option instead of the highest one. Should help into ambiguous situation.
        AgentDirection highestAgentDirection = directions[Random.Range(0.0f, 100.0f) <= 85.0f ? 0 : 1];
        
        //Rotate towards to direction
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(highestAgentDirection.Direction), 0.1f);
        
        //Sets the velocity using the direction
        _rigidbody.velocity = highestAgentDirection.Direction * movingSpeed;
        if (debug)
        {
            Debug.DrawRay(selfPosition, highestAgentDirection.Direction * (sight * 1.5f), directionColor);
        }
    }

    private AgentDirection CalculateAgentDirection(Vector3 selfPosition, Vector3 rayDirection, float sightFactor = 1.0f)
    {
        if (debug)
        {
            Debug.DrawRay(selfPosition, rayDirection * sight, visionColor);
        }

        //Calculate a random weight even though nothing might be found in that direction
        float weight = Random.Range(Mathf.Min(randomDirectionValue.x, randomDirectionValue.y), Mathf.Max(randomDirectionValue.x, randomDirectionValue.y));

        //Create an agentDirection
        AgentDirection direction = new AgentDirection(new Vector3(rayDirection.x, 0.0f, rayDirection.z), weight);
        
        //Raycast into the rayDirection to check if something can be seen in that direction.
        //The sightFactor is a variable that prolongs the ray for the ray in front of the agent. 
        if (Physics.Raycast(selfPosition, rayDirection, out var raycastHit, sight * sightFactor))
        {
            if (debug)
            {
                Debug.DrawLine(selfPosition, raycastHit.point, foundColor);
            }

            float distance = raycastHit.distance;
            float distanceIndex = 1.0f - distance / (sight * sightFactor);

            //Calculate the weight of the found object according to its type
            switch (raycastHit.collider.gameObject.tag)
            {
                case "Box":
                    weight = distanceIndex * distanceFactor + boxWeight;
                    break;
                case "Boat":
                    weight = distanceIndex * boatDistanceFactor + boatWeight;
                    break;
                case "Enemy":
                    weight = distanceIndex * enemyDistanceFactor + enemyWeight;
                    break;
            }
        }
        
        direction.weight = weight;
        return direction;
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
        return new AgentData(_steps, rayRadius, sight, movingSpeed, randomDirectionValue, boxWeight, distanceFactor, boatWeight, boatDistanceFactor, enemyWeight,  enemyDistanceFactor);
    }
}
