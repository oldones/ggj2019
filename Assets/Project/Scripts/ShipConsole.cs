using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipConsole : MonoBehaviour
{
    public Planet closestPlanet{get;private set;}
    private Transform m_Trf;
    private float m_NextScanTime = 0f;
    private const float SCAN_PLANET_TIMER = 10f;


    private void Awake()
    {
        closestPlanet = ScanClosestPlanet();
        m_Trf = transform;
    }

    public void UpdateShipConsole(float dt)
    {
        m_NextScanTime += dt;
        if(m_NextScanTime > SCAN_PLANET_TIMER)
        {
            m_NextScanTime = 0f;
            closestPlanet = ScanClosestPlanet();
        }
        closestPlanet.UpdatePlanet(this, dt);
    }

    public Planet ScanClosestPlanet(bool lookAt = false)
    {
        List<Planet> ps = WorldSpace.Instance.planets;
        Vector3 pos = transform.position;
        int count = ps.Count;
        float closest = float.MaxValue;
        for(int i = 0 ; i < count; ++i)
        {
            float dist = Vector3.Distance(pos, ps[i].planet.transform.position);
            if(dist < closest)
            {
                closest = dist;
                closestPlanet = ps[i];
            }
        }

        if(lookAt)
        {
            transform.LookAt(closestPlanet.transform);
            Debug.LogWarningFormat("Closest planet is: {0} at distance: {1}", closestPlanet.planet.name, closest);
        }
        return closestPlanet;
    }
}
