using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Planet : MonoBehaviour
{
    public GameObject planet{ get; private set;}
    public Transform trf{get{return m_Trf;}}
    public ECELESTIALTYPE celestType{get; private set;}
    private Transform m_Trf;

    public enum ECELESTIALTYPE
    {
        PLANET,
        STAR
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
        pl.SetConfig(config);
        Material t = space.GetPlanetMaterial(pl.celestType);
        p.GetComponent<MeshRenderer>().material = t;
        GameObject.DestroyImmediate(p.GetComponent<SphereCollider>());
        return pl;
    }

    public void SetConfig(SPlanetConfig cfg)
    {
        planet = cfg.obj;
        celestType = cfg.cType;
    }

    public void UpdatePlanet(ShipConsole sc, float dt)
    {
//        Debug.LogFormat("Updating planet: {0}", planet.name);
    }


    public struct SPlanetConfig
    {
        public Vector3 scale;
        public GameObject obj;
        public Planet.ECELESTIALTYPE cType;
    }
}
