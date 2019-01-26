using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpace : MonoBehaviour
{
    [SerializeField]
    private float m_SpaceSize = 1000f;
    [SerializeField]
    private int m_NumPlanets = 10;
    [SerializeField]
    private float m_MinPlanetSize = 10f;
    [SerializeField]
    private float m_MaxPlanetSize = 1000f;
    [SerializeField]
    private Texture[] m_PlanetTextures = null;
    [SerializeField]
    private Material m_PlanetMaterial = null;
    [SerializeField]
    private GameObject m_PlanetPrefab = null;
    public GameObject planetPrefab{ get {return m_PlanetPrefab;}}
    public List<Planet> planets{get;private set;}
    [SerializeField]
    private Dictionary<int, Material> m_PlanetMaterials;
    [SerializeField]
    private ShipConsole m_ShipConsole = null;
    [SerializeField]
    private float m_MinDistanceHomePlanet = 0f;
    private Transform m_Trf;

    public Planet homePlanet{get;private set;}


    public void Init()
    {
        m_Trf = transform;
        planets = new List<Planet>();
        for(int i = 0 ; i < m_NumPlanets; ++i)
        {
            planets.Add(m_Trf.GetChild(i).GetComponent<Planet>());
        }
        homePlanet = _AssignHomePlanet();
        m_ShipConsole.Init(this);
    }

    public void UpdateWorldSpace(float dt)
    {   
        m_ShipConsole.UpdateShipConsole(dt);

        // if(Input.GetKeyUp(KeyCode.Alpha1))
        // {
        //     m_ShipConsole.ScanClosestPlanet(true);
        // }
        // else if(Input.GetKeyUp(KeyCode.Alpha2))
        // {
        //     m_ShipConsole.FlyToClosestPlanet();
        // }
        // 
    }
    
    public void BuildSpace()
    {
        m_Trf = transform;
        m_PlanetMaterials = new Dictionary<int, Material>();
        planets = new List<Planet>();
        for(int i = 0 ; i < m_NumPlanets; ++i)
        {
            //m_Planets.Add(CreatePlanet(i));
            planets.Add(CreatePlanet(i));
        }
    }

    private Planet _AssignHomePlanet()
    {
        int retries = 100;
        Vector3 shipPos = m_ShipConsole.transform.position;
        Planet p = null;
        while(retries > 0)
        {
            p = planets[UnityEngine.Random.Range(0, planets.Count)];
            if(Vector3.Distance(shipPos, p.trf.position) > m_MinDistanceHomePlanet)
            {
                Debug.LogFormat("Chose {0} to be home planet.", p.planet.name);
                return p;
            }
            --retries;
        }
        p = planets[UnityEngine.Random.Range(0, planets.Count)];
        Debug.LogFormat("Failed to assign home planet, {0} was chosen randomly.", p.planet.name);
        return p;
    }

    private Planet CreatePlanet(int i)
    {
        Planet.SPlanetConfig cfg = new Planet.SPlanetConfig();
        Vector3 pos = _ValidSpacePos(UnityEngine.Random.insideUnitSphere * m_SpaceSize);
        float scaleRand = UnityEngine.Random.Range(m_MinPlanetSize, m_MaxPlanetSize);
        cfg.scale = Vector3.one * scaleRand;
        return Planet.CreatePlanet(pos, _GetPlanetName(), i, this, cfg);
    }

    private string _GetPlanetName()
    {
        int retries = 100;
        bool validName = true;
        int count = planets.Count;
        string name = "";
        do
        {
            name = _GenName();
            validName = true;

            for(int i = 0 ; i < count; ++i)
            {
                if(name == planets[i].planet.name)
                {
                    validName = false;
                    break;
                }
            }
            --retries;
        } while(!validName && retries > 0);
        return name;
    }

    private string _GenName()
    {
        string[] prefixes = new string[]{"HD ","HIP ", "GJ ", "Kepler-", "Brahe-", "Galilei-", "Halley-", "Herschel-", "Messier-", "Cannon-", "Leavitt-",
        "Samos-","Mitchell-","Laplace-","Lowel-", "Banneker-", "Galle-","Ptolemy-", "Copernicus-", "Newton-","Huygens-","Cassini-", "Sagan-","Hawking-"};
        string[] sufixes = new string[]{"", "A", "b", "Ab"};
        string prf = prefixes[UnityEngine.Random.Range(0, prefixes.Length)];
        int mid = UnityEngine.Random.Range(100000, 1000000);
        string suf = sufixes[UnityEngine.Random.Range(0, sufixes.Length)];
        return prf + mid + " " + suf;
    }

    public void WipeSpace()
    {
        m_Trf = transform;
        while(m_Trf.childCount > 0)
            GameObject.DestroyImmediate(m_Trf.GetChild(0).gameObject);
        planets = null;
    }

    private Vector3 _ValidSpacePos(Vector3 candidate, int retries = 100)
    {
        for(int i = 0 ; i < planets.Count; ++i)
        {
            if(retries > 0 && Vector3.Distance(planets[i].planet.transform.position, candidate) < m_MaxPlanetSize * 3)
            {
                return _ValidSpacePos(UnityEngine.Random.insideUnitSphere * m_SpaceSize, retries - 1);
            }
        }
        return candidate;
    }

    private Texture _GetPlanetTexture(int idx)
    {
        if(idx == -1)
        {
            idx = UnityEngine.Random.Range(0, m_PlanetTextures.Length);
        }
        return m_PlanetTextures[idx];
    }

    public Material GetPlanetMaterial(int idx)
    {
        if(idx == -1)
        {
            idx = UnityEngine.Random.Range(0, m_PlanetTextures.Length);
        }

        if(!m_PlanetMaterials.ContainsKey(idx))
        {
            Texture t = _GetPlanetTexture(idx);
            Material m = new Material(m_PlanetMaterial);
            m.mainTexture = t;
            m_PlanetMaterials.Add(idx, m);
        }
        return m_PlanetMaterials[idx];
    }
}
