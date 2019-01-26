using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Planet : MonoBehaviour
{
    public GameObject planet{ get; private set;}
    public Transform trf{get{return m_Trf;}}
    public SPlanetConfig config{ get; private set;}
    private Transform m_Trf;

    [SerializeField]
    private SPlanetConfig m_Config;

    public enum ECELESTIALTYPE
    {
        PLANET,
        STAR
    }

    public enum ERESOURCE
    {
        FUEL,
        INVESTIGATION
    }

    void Awake()
    {
        planet = gameObject;
        m_Trf = transform;    
    }

    public static Planet CreatePlanet(Vector3 pos, string pname, int idx, WorldSpace space, SPlanetConfig config)
    {
        GameObject p = GameObject.Instantiate(space.planetPrefab);
        Planet pl = p.GetComponent<Planet>();
        p.transform.position = pos;
        p.transform.SetParent(space.transform);
        p.transform.localScale = config.scale;
        p.name = pname; //"Planet_" + idx;
        config.obj = p;
        config.resource = ERESOURCE.FUEL;
        switch(config.celestType)
        {
            case ECELESTIALTYPE.PLANET:
                float rand = UnityEngine.Random.Range(0f,1f);
                if(rand > 0.6f)
                    config.resource = ERESOURCE.INVESTIGATION;
            break;
            default:
            break;
        }
        pl.SetConfig(config);
        Material t = space.GetPlanetMaterial(config.celestType);
        p.GetComponent<MeshRenderer>().material = t;
        GameObject.DestroyImmediate(p.GetComponent<SphereCollider>());
        return pl;
    }

    public void SetConfig(SPlanetConfig cfg)
    {
        config = cfg;
        m_Config = cfg;
    }

    public void UpdatePlanet(ShipConsole sc, float dt)
    {
//        Debug.LogFormat("Updating planet: {0}", planet.name);
    }

    [System.Serializable]
    public struct SPlanetConfig
    {
        public Vector3 scale;
        public GameObject obj;
        public Planet.ECELESTIALTYPE celestType;
        public ERESOURCE resource;
    }
}
