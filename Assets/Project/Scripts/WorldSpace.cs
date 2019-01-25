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
    private Texture[] m_PlanetTextures;
    [SerializeField]
    private Material m_PlanetMaterial;
    [SerializeField]
    private GameObject m_PlanetPrefab;
    public GameObject planetPrefab{ get {return m_PlanetPrefab;}}
    [SerializeField]
    private List<Planet> m_Planets;
    [SerializeField]
    private Dictionary<int, Material> m_PlanetMaterials;
    private Transform m_Trf;
    
    public void BuildSpace()
    {
        m_Trf = transform;
        m_Planets = new List<Planet>();
        m_PlanetMaterials = new Dictionary<int, Material>();
        Planet.SPlanetConfig cfg = new Planet.SPlanetConfig();
        
        Vector3 pos;
        for(int i = 0 ; i < m_NumPlanets; ++i)
        {
            pos = _ValidSpacePos(Random.insideUnitSphere * m_SpaceSize);
            float scaleRand = UnityEngine.Random.Range(m_MinPlanetSize, m_MaxPlanetSize);
            cfg.scale = Vector3.one * scaleRand;
            m_Planets.Add(new Planet(pos, i, this, cfg));
        }
    }

    public void WipeSpace()
    {
        foreach (Planet p in m_Planets)
        {
            GameObject.DestroyImmediate(p.planet);
        }
        m_Planets.Clear();
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
