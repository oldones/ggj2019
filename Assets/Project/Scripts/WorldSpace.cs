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
    private Material m_StarMaterial = null;
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

    public bool UpdateWorldSpace(float dt)
    {   
        return m_ShipConsole.UpdateShipConsole(dt);
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
            bool isPlanet = p.config.celestType == Planet.ECELESTIALTYPE.PLANET;
            if(isPlanet && Vector3.Distance(shipPos, p.trf.position) > m_MinDistanceHomePlanet)
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
        Vector3 pos = _GetValidPlanetPos(UnityEngine.Random.insideUnitSphere * m_SpaceSize);
        float rand = UnityEngine.Random.Range(0f, 1f);
        cfg.celestType = Planet.ECELESTIALTYPE.STAR;
        string pname = _GetPlanetName();
        if(rand > 0.01)
        {
            cfg.celestType = Planet.ECELESTIALTYPE.PLANET;
            float maxSize = m_MinPlanetSize * 10f;
            float scaleRand = UnityEngine.Random.Range(m_MinPlanetSize, maxSize);
            cfg.scale = Vector3.one * scaleRand;
        }
        else
        {
            Debug.LogFormat("Praise the Sun! {0}", pname);
            float scaleRand = UnityEngine.Random.Range(m_MaxPlanetSize * 0.85f, m_MaxPlanetSize);
            cfg.scale = Vector3.one * scaleRand;   
        }
        return Planet.CreatePlanet(pos, pname, i, this, cfg);
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
                if(name == planets[i].gameObject.name)
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

    private Vector3 _GetValidPlanetPos(Vector3 candidate, int retries = 100)
    {
        if(retries > 0)
        {
            Vector3 origin = Vector3.zero;
            float safeDist = m_MaxPlanetSize * 5;     
            if(Vector3.Distance(origin, candidate) > safeDist)
            {
                for(int i = 0 ; i < planets.Count; ++i)
                {
                    if(Vector3.Distance(planets[i].transform.position, candidate) < safeDist)
                    {
                        return _GetValidPlanetPos(UnityEngine.Random.insideUnitSphere * m_SpaceSize, retries - 1);
                    }
                }
            }
            else
            {
                return _GetValidPlanetPos(UnityEngine.Random.insideUnitSphere * m_SpaceSize, retries - 1);
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

    public Material GetPlanetMaterial(Planet.ECELESTIALTYPE ctype)
    {
        int idx = -1;
        Material mat = m_PlanetMaterial;
        switch(ctype)
        {
            case Planet.ECELESTIALTYPE.PLANET:
                idx = UnityEngine.Random.Range(0, m_PlanetTextures.Length - 1);
            break;
            case Planet.ECELESTIALTYPE.STAR:
                mat = m_StarMaterial;
                idx = m_PlanetTextures.Length - 1;
            break;
            default:
            break;
        }
        if(idx == -1)
        {
            idx = UnityEngine.Random.Range(0, m_PlanetTextures.Length - 1);
        }

        if(!m_PlanetMaterials.ContainsKey(idx))
        {
            Texture t = _GetPlanetTexture(idx);
            Material m = new Material(mat);
            m.mainTexture = t;
            m_PlanetMaterials.Add(idx, m);
        }
        return m_PlanetMaterials[idx];
    }
}
