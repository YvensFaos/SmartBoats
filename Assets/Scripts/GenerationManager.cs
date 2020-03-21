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
    // [SerializeField]
    private List<BoatLogic> activeBoats;
    // [SerializeField]
    private List<PirateLogic> activePirates;

    [Space(10)] 
    [SerializeField]
    private float MutationFactor;
    [SerializeField] 
    private float MutationChance;
    [SerializeField] 
    private int ParentSize;

    [Space(10)] 
    [SerializeField]
    private float SimulationTimer;
    [SerializeField]
    private float SimulationCount;
    [SerializeField]
    private bool RunOnStart;
    
    private bool _runningSimulation;

    private void Start()
    {
        if (RunOnStart)
        {
            StartSimulation();
        }
    }
    
    private void Update()
    {
        if (_runningSimulation)
        {
            if (SimulationCount >= SimulationTimer)
            {
                MakeNewGeneration();
                SimulationCount = -Time.deltaTime;
            } 
            SimulationCount += Time.deltaTime;
        }
    }

    public void GenerateBoxes()
    {
        boxGenerator.RegenerateObjects();
    }
    
    public void GenerateObjects(BoatLogic[] boatParents = null, PirateLogic[] pirateParents = null)
    {
        activeBoats = new List<BoatLogic>();
        List<GameObject> objects = boatGenerator.RegenerateObjects();
        foreach (GameObject obj in objects)
        {
            BoatLogic boat = obj.GetComponent<BoatLogic>();
            if (boat != null)
            {
                activeBoats.Add(boat);
                if (boatParents != null)
                {
                    BoatLogic boatParent = boatParents[Random.Range(0, boatParents.Length - 1)];
                    boat.Birth(boatParent.GetData());
                }
                boat.Mutate(MutationFactor, MutationChance);
            }
        }

        activePirates = new List<PirateLogic>();
        objects = pirateGenerator.RegenerateObjects();
        foreach (GameObject obj in objects)
        {
            PirateLogic pirate = obj.GetComponent<PirateLogic>();
            if (pirate != null)
            {
                activePirates.Add(pirate);
                if (pirateParents != null)
                {
                    PirateLogic pirateParent = pirateParents[Random.Range(0, pirateParents.Length - 1)];
                    pirate.Birth(pirateParent.GetData());
                }
                pirate.Mutate(MutationFactor, MutationChance);
            }
        }
    }

    public void MakeNewGeneration()
    {
        GenerateBoxes();
        
        //Fetch parents
        activeBoats.RemoveAll(item => item == null);
        activeBoats.Sort();
        BoatLogic[] boatParents = new BoatLogic[ParentSize];
        for (int i = 0; i < ParentSize; i++)
        {
            boatParents[i] = activeBoats[i];
        }
        
        activePirates.RemoveAll(item => item == null);
        activePirates.Sort();
        PirateLogic[] pirateParents = new PirateLogic[ParentSize];
        for (int i = 0; i < ParentSize; i++)
        {
            pirateParents[i] = activePirates[i];
        }
        
        //Winners:
        Debug.Log("Last winner boat had: " + activeBoats[0].GetPoints() + " points!" + " Last winner pirate had: " + activePirates[0].GetPoints() + " points!");
        
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
    }
}
