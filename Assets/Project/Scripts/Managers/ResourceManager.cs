using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    [SerializeField] float FoodPercentage;
    [SerializeField] float O2Percent;
    [SerializeField] float FuelPercent;
    [SerializeField] float H2OPercent;
    float HappyLevel;

    float FoodConsume;
    float OxygenConsume;
    float FuelConsume;


    [SerializeField] float EngineerFoodProduction;
    [SerializeField] float EngineerFuelProduction;
    [SerializeField] float EngineerOxygenProduction;


    [SerializeField] float WaterRequiredForConversion = 5;

    [SerializeField] float HighSpeedFuelConsumptionFactor = 8;
    [SerializeField] float NormalSpeedFuelConsumptionFactor = 4;
    [SerializeField] float LowSpeedFuelConsumptionFactor = 1;

    [SerializeField] float EngineFuelConsumption = 3;
    [SerializeField] float SectorFuelConsumption = 3;

    private static ResourceManager _instance;



    public static ResourceManager getInstance()
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

    // Use this for initialization
    void Start()
    {
        CalculateFuelConsumption();
        WaterRequiredForConversion = 5;
        CalculateSectorsConsumptions();
    }

    public void DecrementWater(float requiredWater)
    {
        H2OPercent -= requiredWater;
    }


    public float getEngineerFoodProduction()
    {
        return EngineerFoodProduction;
    }
    public float getEngineerFuelProduction()
    {
        return EngineerFuelProduction;
    }
    public float getEngineerOxygenProduction()
    {
        return EngineerOxygenProduction;
    }

    public void IncrementFood(float totalFoodProduction)
    {
        FoodPercentage += totalFoodProduction;
        FoodPercentage = Mathf.Clamp(FoodPercentage, 0.0f, 100.0f);
    }

    public void IncrementFuel(float totalFuelProduction)
    {
        FuelPercent += totalFuelProduction;
        FuelPercent = Mathf.Clamp(FuelPercent, 0.0f, 100.0f);
    }
    public void IncrementOxygen(float totalOxygenProduction)
    {
        O2Percent += totalOxygenProduction;
        O2Percent = Mathf.Clamp(O2Percent, 0.0f, 100.0f);
    }

    public void DecrementFood(float totalFoodProduction)
    {
        FoodPercentage -= totalFoodProduction;
        FoodPercentage = Mathf.Clamp(FoodPercentage, 0.0f, 100.0f);
    }

    public void DecrementFuel(float totalFuelProduction)
    {
        FuelPercent -= totalFuelProduction;
        FuelPercent = Mathf.Clamp(FuelPercent, 0.0f, 100.0f);

    }
    public void DecrementOxygen(float totalOxygenProduction)
    {
        O2Percent -= totalOxygenProduction;
        O2Percent = Mathf.Clamp(O2Percent, 0.0f, 100.0f);
    }

    public void IncrementWater(float totalWaterGathered)
    {
        H2OPercent += totalWaterGathered;
        H2OPercent = Mathf.Clamp(H2OPercent, 0.0f, 100.0f);
    }



    public float getFoodPercentage()
    {
        return FoodPercentage;
    }

    public float getOxygenPercent()
    {
        return O2Percent;
    }

    public float getFuelPercent()
    {
        return FuelPercent;
    }

    

    public float getWaterPercent()
    {
        return H2OPercent;
    }

    public bool checkWaterQuantity()
    {
        if (H2OPercent < WaterRequiredForConversion)
        {
            return false;
        }
        
        return true;
    }

    public void CalculateTotalHappy()
    {
        HappyLevel = SectorManager.getInstance().getHappyInAllSectors();

    }
    
    private float GetFuelPolicyConsumption()
    {
        switch (GameCoordinator.getInstance().getCurrentSpeedPolicy())
        {
            case shipSpeed.HighSpeed:
                return HighSpeedFuelConsumptionFactor;
            case shipSpeed.NormalSpeed:
                return NormalSpeedFuelConsumptionFactor;
            default:
                return LowSpeedFuelConsumptionFactor;
        }   
    }


    public float CalculateFuelConsumption()
    {
        float FuelPower = GetFuelPolicyConsumption();
        FuelConsume = (EngineFuelConsumption * FuelPower) + (SectorFuelConsumption * SectorManager.getInstance().getActiveSectorsCount());
         

        return FuelConsume;
    }

    public float GetFuelConsumption()
    {
        return FuelConsume;
    }

    public void CalculateSectorsConsumptions()
    {
        SectorManager.getInstance().RecalculateTotalDecayLevels(out FoodConsume,out  OxygenConsume);
    }

    public float GetFoodConsumption()
    {
        return FoodConsume;
    }


    public float GetOxygenConsumption()
    {
        return OxygenConsume;
    }

    public bool CheckIfThereIsFuel()
    {
        return FuelPercent <= 0 ? false : true;
    }


}

