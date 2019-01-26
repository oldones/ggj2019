using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipConsole : MonoBehaviour
{
    public Planet closestPlanet{get;private set;}
    private Transform m_Trf;
    private float m_NextScanTime = 0f;
    private const float SCAN_PLANET_TIMER = 10f;
    private const float MAX_DIST_INTERACT_PLANET = 500f;

    private delegate void UpdateMethod(float dt);
    private UpdateMethod m_UpdateMethod;
    private bool m_CanInteract = true;

    private Canvas m_ShipCanvas;
    private Camera m_ShipCamera;

    void Awake(){
        m_ShipCamera = GetComponentInChildren<Camera>();
        m_ShipCanvas = GetComponentInChildren<Canvas>();

        targetRotation = startRotation = transform.localRotation;
        canvasRotation = Quaternion.LookRotation((m_ShipCanvas.transform.localPosition - this.transform.localPosition).normalized, transform.up);
    }

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

    Quaternion targetRotation;
    float rot = 0f;
    Quaternion startRotation;

    Quaternion defaultRotation = Quaternion.identity;
    Quaternion canvasRotation;

    public void FocusCanvas(bool b){
        startRotation = m_ShipCamera.transform.localRotation;
        if(b){

            targetRotation = defaultRotation;  
            // targetRotation = Quaternion.LookRotation( Vector3.forward, Vector3.up );
            rot = 0f;
            
            // m_ShipCamera.transform.forward = Vector3.forward;
            // m_ShipCamera.transform.localRotation = Quaternion.identity;
        }
        else {
            rot = 45f;
            // var v = Quaternion.LookRotation((m_ShipCanvas.transform.localPosition - this.transform.localPosition).normalized, transform.up );
            // targetRotation = Quaternion.Euler(rot, 0f,0f);

            // rot = Quaternion.LookRotation((m_ShipCanvas.transform.localPosition - this.transform.localPosition).normalized, transform.up).eulerAngles.x;

            // m_ShipCamera.transform.LookAt(m_ShipCanvas.transform.position, transform.up);

            targetRotation = canvasRotation;
        }
        // m_ShipCamera.transform.localRotation = targetRotation;
        c = 0f;

    }

    float c = 0f;

    void Update(){
        
        if(m_ShipCamera.transform.localRotation != targetRotation){
            // m_ShipCamera.transform.localRotation = Quaternion.Lerp(m_ShipCamera.transform.localRotation, targetRotation, Time.deltaTime * 10f);
            m_ShipCamera.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, c);
            c += Time.deltaTime * 3f;
        }
    }

}
