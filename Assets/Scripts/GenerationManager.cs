using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerationManager : MonoBehaviour
{
    [SerializeField]
    private GenerateObjectsInArea[] boxGenerators;
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
    private int boatParentSize;
    [SerializeField] 
    private int pirateParentSize;

    private BoatLogic[] _boatParents;
    private PirateLogic[] _pirateParents;

    [Space(10)] 
    [SerializeField]
    private float simulationTimer;
    [SerializeField]
    private float simulationCount;
    [SerializeField]
    private bool runOnStart;
    
    private bool _runningSimulation;

    [SerializeField]
    private int generationCount;
    [SerializeField]
    private string savePrefabsAt;
    
    [SerializeField]
    private AgentData lastBoatWinnerData;
    [SerializeField]
    private AgentData lastPirateWinnerData;

    private void Start()
    {
        if (runOnStart)
        {
            generationCount = 1;
            StartSimulation();
        }
    }
    
    private void Update()
    {
        if (_runningSimulation)
        {
            if (simulationCount >= simulationTimer)
            {
                ++generationCount;
                MakeNewGeneration();
                simulationCount = -Time.deltaTime;
            } 
            simulationCount += Time.deltaTime;
        }
    }

    public void GenerateBoxes()
    {
        foreach (GenerateObjectsInArea generateObjectsInArea in boxGenerators)
        {
            generateObjectsInArea.RegenerateObjects();
        }
    }
    
    public void GenerateObjects(BoatLogic[] boatParents = null, PirateLogic[] pirateParents = null)
    {
        GenerateBoats(boatParents);
        GeneratePirates(pirateParents);
    }

    private void GeneratePirates(PirateLogic[] pirateParents)
    {
        List<GameObject> objects;
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

    private void GenerateBoats(BoatLogic[] boatParents)
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
    }

    public void MakeNewGeneration()
    {
        GenerateBoxes();
        
        //Fetch parents
        _activeBoats.RemoveAll(item => item == null);
        _activeBoats.Sort();
        if (_activeBoats.Count == 0)
        {
            GenerateBoats(_boatParents);
        }
        _boatParents = new BoatLogic[boatParentSize];
        for (int i = 0; i < boatParentSize; i++)
        {
            _boatParents[i] = _activeBoats[i];
        }

        BoatLogic lastBoatWinner = _activeBoats[0];
        lastBoatWinner.name += "Gen-" + generationCount; 
        lastBoatWinnerData = lastBoatWinner.GetData();
        PrefabUtility.SaveAsPrefabAsset(lastBoatWinner.gameObject, savePrefabsAt + lastBoatWinner.name + ".prefab");
        
        _activePirates.RemoveAll(item => item == null);
        _activePirates.Sort();
        _pirateParents = new PirateLogic[pirateParentSize];
        for (int i = 0; i < pirateParentSize; i++)
        {
            _pirateParents[i] = _activePirates[i];
        }

        PirateLogic lastPirateWinner = _activePirates[0];
        lastPirateWinner.name += "Gen-" + generationCount; 
        lastPirateWinnerData = lastPirateWinner.GetData();
        PrefabUtility.SaveAsPrefabAsset(lastPirateWinner.gameObject, savePrefabsAt + lastPirateWinner.name + ".prefab");
        
        //Winners:
        Debug.Log("Last winner boat had: " + lastBoatWinner.GetPoints() + " points!" + " Last winner pirate had: " + lastPirateWinner.GetPoints() + " points!");
        
        GenerateObjects(_boatParents, _pirateParents);
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
        _activeBoats.RemoveAll(item => item == null);
        _activeBoats.ForEach(boat => boat.Sleep());
        _activePirates.ForEach(pirate => pirate.Sleep());
    }
}
