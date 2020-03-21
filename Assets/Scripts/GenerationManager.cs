using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationManager : MonoBehaviour
{
    [SerializeField]
    private GenerateObjectsInArea boxGenerator;
    [SerializeField]
    private GenerateObjectsInArea boatGenerator;
    [SerializeField]
    private GenerateObjectsInArea pirateGenerator;

    [Space(10)]
    private List<BoatLogic> _activeBoats;
    private List<PirateLogic> _activePirates;

    [Space(10)] 
    [SerializeField]
    private float mutationFactor;
    [SerializeField] 
    private float mutationChance;
    [SerializeField] 
    private int parentSize;

    [Space(10)] 
    [SerializeField]
    private float simulationTimer;
    [SerializeField]
    private float simulationCount;
    [SerializeField]
    private bool runOnStart;
    
    private bool _runningSimulation;

    private void Start()
    {
        if (runOnStart)
        {
            StartSimulation();
        }
    }
    
    private void Update()
    {
        if (_runningSimulation)
        {
            if (simulationCount >= simulationTimer)
            {
                MakeNewGeneration();
                simulationCount = -Time.deltaTime;
            } 
            simulationCount += Time.deltaTime;
        }
    }

    public void GenerateBoxes()
    {
        boxGenerator.RegenerateObjects();
    }
    
    public void GenerateObjects(BoatLogic[] boatParents = null, PirateLogic[] pirateParents = null)
    {
        _activeBoats = new List<BoatLogic>();
        List<GameObject> objects = boatGenerator.RegenerateObjects();
        foreach (GameObject obj in objects)
        {
            BoatLogic boat = obj.GetComponent<BoatLogic>();
            if (boat != null)
            {
                _activeBoats.Add(boat);
                if (boatParents != null)
                {
                    BoatLogic boatParent = boatParents[Random.Range(0, boatParents.Length - 1)];
                    boat.Birth(boatParent.GetData());
                }
                boat.Mutate(mutationFactor, mutationChance);
                boat.AwakeUp();
            }
        }

        _activePirates = new List<PirateLogic>();
        objects = pirateGenerator.RegenerateObjects();
        foreach (GameObject obj in objects)
        {
            PirateLogic pirate = obj.GetComponent<PirateLogic>();
            if (pirate != null)
            {
                _activePirates.Add(pirate);
                if (pirateParents != null)
                {
                    PirateLogic pirateParent = pirateParents[Random.Range(0, pirateParents.Length - 1)];
                    pirate.Birth(pirateParent.GetData());
                }
                pirate.Mutate(mutationFactor, mutationChance);
                pirate.AwakeUp();
            }
        }
    }

    public void MakeNewGeneration()
    {
        GenerateBoxes();
        
        //Fetch parents
        _activeBoats.RemoveAll(item => item == null);
        _activeBoats.Sort();
        BoatLogic[] boatParents = new BoatLogic[parentSize];
        for (int i = 0; i < parentSize; i++)
        {
            boatParents[i] = _activeBoats[i];
        }
        
        _activePirates.RemoveAll(item => item == null);
        _activePirates.Sort();
        PirateLogic[] pirateParents = new PirateLogic[parentSize];
        for (int i = 0; i < parentSize; i++)
        {
            pirateParents[i] = _activePirates[i];
        }
        
        //Winners:
        Debug.Log("Last winner boat had: " + _activeBoats[0].GetPoints() + " points!" + " Last winner pirate had: " + _activePirates[0].GetPoints() + " points!");
        
        GenerateObjects(boatParents, pirateParents);
    }

    public void StartSimulation()
    {
        GenerateBoxes();
        GenerateObjects();
        _runningSimulation = true;
    }
    
    public void StopSimulation()
    {
        _runningSimulation = false;
        _activeBoats.ForEach(boat => boat.Sleep());
        _activePirates.ForEach(pirate => pirate.Sleep());
    }
}
