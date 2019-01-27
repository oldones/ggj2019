using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenProductionSector : Sector
{
    ResourceManager resourceManagerRef;

    [SerializeField] float RequiredWater = 5;

    [SerializeField] float defaultOxygenProductionSector = 0.3f;

    private void Start()
    {
        base.Start();
        resourceManagerRef = GameObject.FindObjectOfType<ResourceManager>();
    }


    public float CalculateResourceProduced()
    {
        if (resourceManagerRef.checkWaterQuantity() && !(ResourceManager.getInstance().getOxygenPercent() >= 100 ))
        {
            resourceManagerRef.DecrementWater(RequiredWater);
            float EngDecay = (getEngineerCount() / maximumCapacity) * 2;
            float EngPower = Mathf.Clamp(EngDecay, SectorManager.getInstance().getMinimumResourceProduction(), SectorManager.getInstance().getMaximumResourceProduction());

            return RequiredWater * EngPower;
        }
        return 0.0f;
        
    }
}
