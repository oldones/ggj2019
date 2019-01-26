using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649 //serialize field bullshit

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
    private GameObject m_TargetPlanet = null;
    
    private WorldSpace m_WorldSpace = null;
    private Planet m_HomePlanet = null;
    private Vector3 m_HomePlanetCoords;
    private ShipController2 m_ShipController;

    private Camera m_ShipCamera;
    [SerializeField]
    private Canvas[] m_CanvasPanels;
    Quaternion defaultRotation = Quaternion.identity;
    Quaternion centerPanelRotation;
    Quaternion leftPanelRotation;
    Quaternion rightPanelRotation;
    public bool m_Warp = false;

    public float fuel{ get; private set;}
    public float investigation{ get; private set;}

    [Header("Effects")]
    [SerializeField]
    private Image m_VignetteEffect;


    void Awake(){
        m_ShipCamera = GetComponentInChildren<Camera>();
        m_ShipController = GetComponent<ShipController2>();
        targetRotation = startRotation = transform.localRotation;

        centerPanelRotation = Quaternion.LookRotation((m_CanvasPanels[0].transform.localPosition - transform.localPosition), transform.up);
        leftPanelRotation = Quaternion.LookRotation((m_CanvasPanels[1].transform.localPosition - transform.localPosition), transform.up);
        rightPanelRotation = Quaternion.LookRotation((m_CanvasPanels[2].transform.localPosition - transform.localPosition), transform.up);
    }

    public void Init(WorldSpace ws)
    {
        fuel = 1f;
        investigation = 0f;
        m_WorldSpace = ws;
        m_HomePlanet = ws.homePlanet;
        m_HomePlanetCoords = m_HomePlanet.trf.position;
        closestPlanet = ScanClosestPlanet();
        m_Trf = transform;
        m_UpdateMethod = _IdleUpdate;
    }

    public bool UpdateShipConsole(float dt)
    {
        m_UpdateMethod(dt);
        closestPlanet.UpdatePlanet(this, dt);
        return _ReachHomePlanet();
    }

    private bool _ReachHomePlanet()
    {
        return Vector3.Distance(m_Trf.position, m_HomePlanetCoords) < MAX_DIST_INTERACT_PLANET;
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
        SpendFuel();
        //Debug.LogFormat("speed: {2} fuel: {0} investigation: {1}", fuel, investigation, m_ShipController.trueSpeed);
    }

    public void ToggleInteract(bool v)
    {
        m_CanInteract = v;
    }

    private void _InteractUpdate(float dt)
    {
        m_CanInteract = false;
        if(Vector3.Distance(closestPlanet.trf.position, m_Trf.position) > MAX_DIST_INTERACT_PLANET)
        {
            m_UpdateMethod = _IdleUpdate;
            _GiveResource(closestPlanet.config.resource);
        }
    }

    private void _GiveResource(Planet.ERESOURCE res)
    {
        float rand = 0f;
        switch(res)
        {
            case Planet.ERESOURCE.FUEL:
                rand = UnityEngine.Random.Range(0.05f, 0.5f);
                fuel += rand;
                fuel = Mathf.Clamp(fuel,0f, 1f);
            break;
            case Planet.ERESOURCE.INVESTIGATION:
                rand = UnityEngine.Random.Range(0.01f, 0.1f);
                investigation += rand;
                investigation = Mathf.Clamp(investigation,0f, 1f);
                if(investigation > 0.99f)
                {
                    _DiscloseHomePlanet();
                }
            break;
            default:
            break;
        }
    }

    public void SpendFuel()
    {
        if(m_ShipController.trueSpeed > 5f)
        {
            float val = 0.0001f;
            if(m_ShipController.hiperDriving)
            {
                val *= 3f;
            }
            fuel -= val;
            fuel = Mathf.Clamp(fuel,0f, 1f);
            if(fuel == 0f)
            {
                _GameOver();
            }
        }
    }

    private void _DiscloseHomePlanet()
    {
        //indicate the player which is the home planet
    }

    private void _GameOver()
    {
        GameController.instance.GameOver();
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
        List<Planet> ps = m_WorldSpace.planets;
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

    public enum EPanels { None, Center, Left, Right }
    private EPanels m_CurrentPanel = EPanels.None;

    Quaternion targetRotation;
    Quaternion startRotation;

    public bool IsSteering {get {return m_CurrentPanel == EPanels.None;}}

    public void FocusPanel(EPanels panel){
        
        if(panel == m_CurrentPanel)
            return;

        startRotation = m_ShipCamera.transform.localRotation;

        m_CurrentPanel = panel;

        switch(panel){
            case EPanels.None:
                targetRotation = defaultRotation;
                break;
            case EPanels.Center:
                targetRotation = centerPanelRotation;
                break;
            case EPanels.Left:
                targetRotation = leftPanelRotation;
                break;
            case EPanels.Right:
                targetRotation = rightPanelRotation;
                break;
        }

        c = 0f;
    }

    float c = 0f;
    float zoomFoV = 35f;
    float defaultFoV = 65f;
    float hyperdriveFoV = 110f;

    private void _UpdateCamera(){
        if(m_ShipCamera.transform.localRotation != targetRotation){
            //change camera FoV
            if(m_ShipCamera.fieldOfView != defaultFoV && m_CurrentPanel == EPanels.None)
                m_ShipCamera.fieldOfView = Mathf.Lerp(m_ShipCamera.fieldOfView, defaultFoV, c);
            if(m_ShipCamera.fieldOfView != zoomFoV && m_CurrentPanel != EPanels.None)
                m_ShipCamera.fieldOfView = Mathf.Lerp(m_ShipCamera.fieldOfView, zoomFoV, c);
            //rotate towards target
            m_ShipCamera.transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, c);
            c+=Time.deltaTime * 3f;
        }
    }

    public void SetWarp(bool w){
        m_Warp = w;
    }

    private void _UpdateWarpEffect(){
        if(IsSteering){
            m_ShipCamera.fieldOfView = Mathf.Lerp(m_ShipCamera.fieldOfView, m_Warp ? hyperdriveFoV : defaultFoV, Time.deltaTime * 2f);
            m_VignetteEffect.color = Color.Lerp(m_VignetteEffect.color, m_Warp ? Color.white : new Color(1f,1f,1f,0f), Time.deltaTime);
        }
    }

    void Update(){
        _UpdateCamera();
        _UpdateWarpEffect();
    }
}
