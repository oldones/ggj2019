using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour {

    float HappyLevel;
    [SerializeField] float SardineLevel;
    int PopulationCount;

    [SerializeField] protected int militaryCount;
    [SerializeField] protected int scienceCount;
    [SerializeField] protected int engineerCount;
    [SerializeField] protected int normalCount;

    [SerializeField] protected int maximumCapacity;

    public float currentFoodConsumption;
    public float currentOxygenConsumption;

    private float DeathProbability;

    public float getDeathProbability()
    {
        return DeathProbability;
    }

    private FoodPolicy currentFoodPolicy = FoodPolicy.NormalRations;

    public void Start()
    {
        currentFoodPolicy = FoodPolicy.NormalRations;
    }

    public void CalculateInitialValues()
    {
        HappyLevel = CalculateHappyLevelInSector();
        currentFoodConsumption = calculateSectorFoodConsumption();
        currentOxygenConsumption = calculateSectorOxygenConsumption();
        calculateDeathProbability();


    }


    public void changeFoodPolicy(FoodPolicy newFoodPolicy)
    {
        currentFoodPolicy = newFoodPolicy;
        calculateSectorFoodConsumption();

    }

    public FoodPolicy getActiveFoodPolicy()
    {
        return currentFoodPolicy;
    }


    public int getMilitaryPopulation()
    {
        return militaryCount;
    }

    public int getScientistCount()
    {
        return scienceCount;
    }

    public int getEngineerCount()
    {
        return engineerCount;
    }

    public int getNormalCount()
    {
        return normalCount;
    }


    public float getActiveFoodPolicyConsumptionFactor()
    {
        return SectorManager.getInstance().getFoodPolicyConsumption(currentFoodPolicy);
    }


    public float calculateSectorFoodConsumption()
    {
        
        float popCount = scienceCount + engineerCount + normalCount + militaryCount;

        // varia entre 0.2 e 2
        float militaryPopRatio = 1 - (militaryCount / popCount);
        militaryPopRatio *= 2;
        //se nao houver militares nao fica = 0
        militaryPopRatio = Mathf.Clamp(militaryPopRatio, SectorManager.getInstance().getMinimumFoodSectorConsumption(),SectorManager.getInstance().getMaximumFoodSectorConsumption());

        float convertedFoodPolicy = getActiveFoodPolicyConsumptionFactor();

        float consumptionFactor = ((float)convertedFoodPolicy) * militaryPopRatio;

        this.currentFoodConsumption = ((consumptionFactor * popCount)); 

        return this.currentFoodConsumption;
    }

    public float getSectorFoodConsumption()
    {
        return this.currentFoodConsumption;
    }

    public float getSectorOxygenConsumption()
    {
        return this.currentOxygenConsumption;
    }

    public float calculateSectorOxygenConsumption()
    {
     
        

        float HappyDecay = (1 - HappyLevel) * 2;

        float HappyPower = Mathf.Clamp(HappyDecay, SectorManager.getInstance().getMinimumOxygenSectorConsumption(), SectorManager.getInstance().getMaximumOxygenSectorConsumption());

        float personOxygenUse = SectorManager.getInstance().getPersonOxygenConsumptionFactor() * HappyPower;


        this.currentOxygenConsumption = ((personOxygenUse * PopulationCount));

        return this.currentOxygenConsumption;
    }

    public void calculateDeathProbability()
    {
        float HappinessFactor = (1 - HappyLevel) * 0.05f;
        float SardineFactor = (1-SardineLevel) * 0.05f;
        float FoodFactor = ResourceManager.getInstance().getFoodPercentage() > 0 ? ( 1 - SectorManager.getInstance().getFoodPolicyHappyImpact(currentFoodPolicy)) * 0.10f : 1;

        float OxygenFactor = ResourceManager.getInstance().getOxygenPercent() > 0 ? 0 : 1;

        float death = SardineFactor + HappinessFactor + FoodFactor + OxygenFactor;

        DeathProbability = (Mathf.Clamp(death, 0.0f, 1.0f)) * 100;
    }


    public int calculateDeaths()
    {
        int totalDeaths = 0;

        calculateDeathProbability();


        float currentRandom;
        int numDeaths = 0;
        for (int i = 0; i < militaryCount; i++)
        {
            currentRandom = UnityEngine.Random.Range(1, 200);
            
            if (currentRandom <= DeathProbability)
            {
                totalDeaths++;
                numDeaths++;
            }
        }
        militaryCount -= numDeaths;
        numDeaths = 0;
        for(int i = 0; i < scienceCount; i++)
        {
            currentRandom = UnityEngine.Random.Range(1, 200);
            if (currentRandom <= DeathProbability)
            {
                totalDeaths++;
                numDeaths++;
            }
        }
        scienceCount -= numDeaths;
        numDeaths = 0;
        for (int i = 0; i< engineerCount; i++)
        {
            currentRandom = UnityEngine.Random.Range(1, 200);
            if (currentRandom <= DeathProbability)
            {
                totalDeaths++;
                numDeaths++;
            }
        }
        engineerCount -= numDeaths;
        numDeaths = 0;
        for (int i = 0; i< normalCount; i++)
        {
            currentRandom = UnityEngine.Random.Range(1, 200);
            if (currentRandom <= DeathProbability)
            {
                totalDeaths++;
                numDeaths++;
            }
        }
        normalCount -= numDeaths;
        numDeaths = 0;

        return totalDeaths;

    }

    public float CalculateHappyLevelInSector()
    {
        HappyLevel = (calculateSardineLevel() + SectorManager.getInstance().calculateDeathImpact() + SectorManager.getInstance().getFoodPolicyHappyImpact(currentFoodPolicy) + getSectorMilitarization() + GameCoordinator.getInstance().getTimeAwayFromPlanet()) / 5;
        return HappyLevel;
    }

    public float GetHappyLevelInSector()
    {
        return HappyLevel;
    }

    public float calculateSardineLevel()
    {
        
        SardineLevel = 1 - Mathf.Clamp((PopulationCount / maximumCapacity) - 1, 0, 1);
        return SardineLevel;
    }

    private float getSectorMilitarization()
    {
        float popCount = scienceCount + engineerCount + normalCount + militaryCount;
        return 1 - Mathf.Clamp((militaryCount / (popCount/3)) - 1, 0, 1);
    }



    public int GetAlivePopulation()
    {
        
        return PopulationCount;
    }

    public void RecalculateAlivePopulation()
    {
        PopulationCount = scienceCount + engineerCount + normalCount + militaryCount;
    }
    


    
    private void DeactivateSector()
    {
        SectorManager.getInstance().SectorDeactivated(this);
    }
    private void ActivateSector()
    {
        SectorManager.getInstance().SectorActivated(this);
    }

    


}
