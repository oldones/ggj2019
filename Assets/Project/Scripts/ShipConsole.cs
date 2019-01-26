﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipConsole : MonoBehaviour
{
    public Planet closestPlanet{get;private set;}
    private Transform m_Trf;
    private float m_NextScanTime = 0f;
    private const float SCAN_PLANET_TIMER = 10f;
    private const float MAX_DIST_INTERACT_PLANET = 500f;
    private const float HD_JUMP_TIME = 2f;

    private delegate void UpdateMethod(float dt);
    private UpdateMethod m_UpdateMethod;
    private bool m_CanInteract = true;
    private float m_JumpTime = 0f;
    private Vector3 m_JumpTarget = Vector3.zero;
    [SerializeField]
    private GameObject m_TargetPlanet;
    public void Init()
    {
        closestPlanet = ScanClosestPlanet();
        m_Trf = transform;
        m_UpdateMethod = _IdleUpdate;
    }

    public void UpdateShipConsole(float dt)
    {
        m_UpdateMethod(dt);
        closestPlanet.UpdatePlanet(this, dt);
    }

    private void _IdleUpdate(float dt)
    {
        if(m_CanInteract)
        {
            m_NextScanTime += dt;
            if(m_NextScanTime > SCAN_PLANET_TIMER)
            {
                m_NextScanTime = 0f;
                closestPlanet = ScanClosestPlanet();
            }

            float dist = Vector3.Distance(closestPlanet.trf.position, m_Trf.position);
            
            if(dist < MAX_DIST_INTERACT_PLANET)
            {
                Debug.LogFormat("Interacting with {0}", closestPlanet.planet.name);
                m_UpdateMethod = _InteractUpdate;
            }
        }
        else 
        {
            //player has to leave the vicinity of the closest planet to be able to interact with it again
            m_CanInteract = Vector3.Distance(closestPlanet.trf.position, m_Trf.position) > MAX_DIST_INTERACT_PLANET;
            if(m_CanInteract)
            {
                Debug.LogFormat("Can Interact again with {0}", closestPlanet.planet.name);
            }
        }
    }

    private void _InteractUpdate(float dt)
    {
        m_CanInteract = false;
        if(Vector3.Distance(closestPlanet.trf.position, m_Trf.position) > MAX_DIST_INTERACT_PLANET)
        {
            m_UpdateMethod = _IdleUpdate;
        }
    }

    private void _FlyToUpdate(float dt)
    {
        m_JumpTime += dt;
        float progress =  m_JumpTime / HD_JUMP_TIME;
        m_Trf.position = Vector3.Slerp(m_Trf.position, m_JumpTarget, progress);
        m_Trf.LookAt(m_JumpTarget);
        
        float dist = Vector3.Distance(closestPlanet.trf.position, m_Trf.position);
        if(dist < MAX_DIST_INTERACT_PLANET)
        {
            m_UpdateMethod = _InteractUpdate;
        }
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

    public void FlyToClosestPlanet()
    {
        m_JumpTime = 0f;

        if(m_TargetPlanet != null)
        {
            closestPlanet = m_TargetPlanet.GetComponent<Planet>();
        }

        m_JumpTarget = closestPlanet.trf.position;
        m_UpdateMethod = _FlyToUpdate;
        m_CanInteract = false;
    }
}
