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
    [SerializeField]
    private List<Planet> m_Planets;
    public List<Planet> planets{get{return m_Planets;}}
    [SerializeField]
    private Dictionary<int, Material> m_PlanetMaterials;
    [SerializeField]
    private ShipConsole m_ShipConsole = null;
    private Transform m_Trf;

    public static WorldSpace Instance;


    private void Awake()
    {
        Instance = this;
        m_Trf = transform;
        for(int i = 0 ; i < m_NumPlanets; ++i)
        {
            m_Planets.Add(m_Trf.GetChild(i).GetComponent<Planet>());
        }
        m_ShipConsole.Init();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            m_ShipConsole.ScanClosestPlanet(true);
        }
        m_ShipConsole.UpdateShipConsole(dt);
    }
    
    public void BuildSpace()
    {
        m_Trf = transform;
        m_PlanetMaterials = new Dictionary<int, Material>();
        m_Planets = new List<Planet>();
        for(int i = 0 ; i < m_NumPlanets; ++i)
        {
            //m_Planets.Add(CreatePlanet(i));
            m_Planets.Add(CreatePlanet(i));
        }
    }

    private Planet CreatePlanet(int i)
    {
        Planet.SPlanetConfig cfg = new Planet.SPlanetConfig();
        Vector3 pos = _ValidSpacePos(Random.insideUnitSphere * m_SpaceSize);
        float scaleRand = UnityEngine.Random.Range(m_MinPlanetSize, m_MaxPlanetSize);
        cfg.scale = Vector3.one * scaleRand;
        return Planet.CreatePlanet(pos, _GetPlanetName(), i, this, cfg);
    }

    private string _GetPlanetName()
    {
        int retries = 100;
        bool validName = true;
        int count = m_Planets.Count;
        string name = "";
        do
        {
            name = _GenName();
            validName = true;

            for(int i = 0 ; i < count; ++i)
            {
                if(name == m_Planets[i].planet.name)
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
        "Samos-","Mitchell-","Laplace-","Lowel-", "Bannerker-", "Galle-","Ptolemy-", "Copernicus-", "Newton-","Huygens-","Cassini-", "Sagan-","Hawking-"};
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
        m_Planets = null;
    }

    private Vector3 _ValidSpacePos(Vector3 candidate, int retries = 100)
    {
        for(int i = 0 ; i < m_Planets.Count; ++i)
        {
            if(retries > 0 && Vector3.Distance(m_Planets[i].planet.transform.position, candidate) < m_MaxPlanetSize * 3)
            {
                return _ValidSpacePos(Random.insideUnitSphere * m_SpaceSize, retries - 1);
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
