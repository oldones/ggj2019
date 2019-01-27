using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorManager : MonoBehaviour {
    private List<Sector> AllSectors = new List<Sector>();
    private List<FoodProductionSector> FoodProductionSectors = new List<FoodProductionSector>();
    private List<FuelProductionSector> FuelProductionSectors = new List<FuelProductionSector>();



    private List<OxygenProductionSector> OxygenProductionSectors = new List<OxygenProductionSector>();

    [SerializeField] float NormalRationsFoodFactor = 0.0015f;
    [SerializeField] float HalfRationsFoodFactor = 0.0008f;
    [SerializeField] float SurvivalRationsFoodFactor = 0.0002f;

    [SerializeField] float NormalRationsHappyFactor = 1;
    [SerializeField] float HalfRationsHappyFactor = 0.5f;
    [SerializeField] float SurvivalRationsHappyFactor = 0;

    [SerializeField] float MaximumResourceProdution = 1.5f;
    [SerializeField] float MinimumResourceProdution = 0.2f;


    [SerializeField] float minimumFoodSectorConsumption = 0.2f;
    [SerializeField] float maximumFoodSectorConsumption = 2f;

<<<<<<< HEAD
    [SerializeField] float personOxygenConsumptionFactor = 0.0016f;
=======
    [SerializeField] float personOxygenConsumptionFactor = 0.0008f;
>>>>>>> 2739a2a49ba221585e88047b5269d74582a3e986

    [SerializeField] float minimumOxygenSectorConsumption = 0.2f;
    [SerializeField] float maximumOxygenSectorConsumption = 2f;

    [SerializeField] float minimumFuelSectorConsumption = 0.2f;

    float currentTotalFoodDecay;
    float currentTotalFuelDecay;
    float currentTotalOxygenDecay;


    private int totalPopulation;
    private int currentPopulation;

    private int numActiveSectors;

    private ResourceManager resourceManagerRef;


    private static SectorManager _instance;

    public static SectorManager getInstance()
    {
        return _instance;
    }

    

    void Awake()
    {
        //Check if instance already exists
        if (_instance == null)

            //if not, set instance to this
            _instance = this;

        //If instance already exists and it's not this:
        else if (_instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }

    public int getCurrentDeaths()
    {
        return totalPopulation - currentPopulation;
    }

    public int getCurrentPopulation()
    {
        return currentPopulation;
    }

    public bool CheckIfPeopleAreAlive()
    {
        return currentPopulation > 0 ?  true : false; 
    }


    // Use this for initialization
    void Start () {
        resourceManagerRef = GameObject.FindObjectOfType<ResourceManager>();
        AllSectors = new List<Sector>(FindObjectsOfType<Sector>());
        FoodProductionSectors = new List<FoodProductionSector>(FindObjectsOfType<FoodProductionSector>());
        FuelProductionSectors = new List<FuelProductionSector>(FindObjectsOfType<FuelProductionSector>());
        OxygenProductionSectors = new List<OxygenProductionSector>(FindObjectsOfType<OxygenProductionSector>());

        foreach(Sector currentSector in AllSectors)
        {
            currentSector.RecalculateAlivePopulation();
            totalPopulation += currentSector.GetAlivePopulation();
        }
        currentPopulation = totalPopulation;
        foreach (Sector currentSector in AllSectors)
        {
            currentSector.CalculateInitialValues();
        }
        

    }

    public float getMaximumResourceProduction()
    {
        return MaximumResourceProdution;
    }

    public float getMinimumResourceProduction()
    {
        return MinimumResourceProdution;
    }


    public void RecalculateTotalDecayLevels(out float currentTotalFoodDecay, out float currentTotalOxygenDecay)
    {
        currentTotalFoodDecay = 0;
        currentTotalOxygenDecay = 0;
        foreach (Sector currentSector in AllSectors)
        {
            currentTotalFoodDecay +=  currentSector.getSectorFoodConsumption();
            currentTotalOxygenDecay += currentSector.getSectorOxygenConsumption();
        }
    }


    public float getMinimumFoodSectorConsumption()
    {
        return minimumFoodSectorConsumption;
    }

    public float getMaximumFoodSectorConsumption()
    {
        return maximumFoodSectorConsumption;
    }

    public float getMinimumOxygenSectorConsumption()
    {
        return minimumOxygenSectorConsumption;
    }

    public float getMaximumOxygenSectorConsumption()
    {
        return maximumOxygenSectorConsumption;
    }

    public float getPersonOxygenConsumptionFactor()
    {
        return personOxygenConsumptionFactor;
    }

    public void produceFoodInSectors()
    {
        float totalFoodProduction = 0;
        foreach(FoodProductionSector sector in FoodProductionSectors){
            totalFoodProduction+= sector.CalculateResourceProduced();
        }
        resourceManagerRef.IncrementFood(totalFoodProduction);
    }

    public void produceFuelInSectors()
    {
        float totalFuelProduction = 0;
        foreach (FuelProductionSector sector in FuelProductionSectors)
        {
            totalFuelProduction += sector.CalculateResourceProduced();
        }
        resourceManagerRef.IncrementFuel(totalFuelProduction);
    }

    public void produceOxygenInSectors()
    {
        float totalOxygenSectors = 0;
        foreach (FuelProductionSector sector in FuelProductionSectors)
        {
            totalOxygenSectors += sector.CalculateResourceProduced();
        }
        resourceManagerRef.IncrementOxygen(totalOxygenSectors);
    }

    public int getActiveSectorsCount()
    {
        return AllSectors.Count;
    }

    public void SectorActivated(Sector newActiveSector)
    {
        AllSectors.Add(newActiveSector);

        if(newActiveSector.GetType() == typeof(FoodProductionSector))
        {
            FoodProductionSectors.Add((FoodProductionSector)newActiveSector);
        }else if (newActiveSector.GetType() == typeof(FuelProductionSector))
        {
            FuelProductionSectors.Add(((FuelProductionSector)newActiveSector));
        }else if (newActiveSector.GetType() == typeof(OxygenProductionSector))
        {
            OxygenProductionSectors.Add((OxygenProductionSector)newActiveSector);
        }


    }

    public void SectorDeactivated(Sector newActiveSector)
    {
        AllSectors.Remove(newActiveSector);

        if (newActiveSector.GetType() == typeof(FoodProductionSector))
        {
            FoodProductionSectors.Remove((FoodProductionSector)newActiveSector);
        }
        else if (newActiveSector.GetType() == typeof(FuelProductionSector))
        {
            FuelProductionSectors.Remove(((FuelProductionSector)newActiveSector));
        }
        else if (newActiveSector.GetType() == typeof(OxygenProductionSector))
        {
            OxygenProductionSectors.Remove((OxygenProductionSector)newActiveSector);
        }
    }




    public void DecayCycle()
    {

        List<Sector> sectorsToCalculate = new List<Sector>(AllSectors);

        List<Sector> calculatedSectors = new List<Sector>();

       
        foreach(Sector currentSector in sectorsToCalculate)
        {
            ResourceManager.getInstance().DecrementOxygen(currentSector.calculateSectorOxygenConsumption());
            if (currentSector.getActiveFoodPolicy() == FoodPolicy.NormalRations)
            {
                ResourceManager.getInstance().DecrementFood(currentSector.calculateSectorFoodConsumption());
                calculatedSectors.Add(currentSector);
            }
            
        }
        foreach(Sector currentSector in calculatedSectors)
        {
            sectorsToCalculate.Remove(currentSector);
        }
        calculatedSectors.Clear();
      
        foreach (Sector currentSector in sectorsToCalculate)
        {
            ResourceManager.getInstance().DecrementOxygen(currentSector.calculateSectorOxygenConsumption());
            if (currentSector.getActiveFoodPolicy() == FoodPolicy.HalfRations)
            {
                ResourceManager.getInstance().DecrementFood(currentSector.calculateSectorFoodConsumption());
                calculatedSectors.Add(currentSector);
            }
           
        }
        foreach (Sector currentSector in calculatedSectors)
        {
            sectorsToCalculate.Remove(currentSector);
        }
        calculatedSectors.Clear();
       


        foreach (Sector currentSector in sectorsToCalculate)
        {
            ResourceManager.getInstance().DecrementOxygen(currentSector.calculateSectorOxygenConsumption());
            ResourceManager.getInstance().DecrementFood(currentSector.calculateSectorFoodConsumption());
  
        }
       
    }

    public void DeathCycle()
    {
        foreach(Sector currentSector in AllSectors)
        {
            currentPopulation -= currentSector.calculateDeaths();
        }
    }

    public void HappinessCycle()
    {
        foreach(Sector currentSector in AllSectors)
        {
            currentSector.CalculateHappyLevelInSector();
        }
    }

    public float getHappyInAllSectors()
    {
        float toReturn = 0.0f;   
        foreach (Sector currentSector in AllSectors)
        {
            toReturn += currentSector.CalculateHappyLevelInSector();
        }

        return toReturn/AllSectors.Count;

    }

    public float calculateDeathImpact()
    {
        return ((float)totalPopulation) / ((float)currentPopulation);
    }

    public float getFoodPolicyConsumption(FoodPolicy activeFoodPolicy )
    {
        switch (activeFoodPolicy)
        {
            case FoodPolicy.NormalRations:
                return NormalRationsFoodFactor;
            case FoodPolicy.HalfRations:
                return HalfRationsFoodFactor;
            default:
                return SurvivalRationsFoodFactor;
        }
    }    

    public float getFoodPolicyHappyImpact(FoodPolicy activeFoodPolicy)
    {
        switch (activeFoodPolicy)
        {
            case FoodPolicy.NormalRations:
                return NormalRationsHappyFactor;
            case FoodPolicy.HalfRations:
                return HalfRationsHappyFactor;
            default:
                return SurvivalRationsHappyFactor;
        }
    }


    
}

public enum FoodPolicy{
    NormalRations = 3,
    HalfRations = 2,
    SurvivalRations = 1
}




public enum shipSpeed
{
    HighSpeed = 3,
    NormalSpeed = 2,
    LowSpeed = 1
}