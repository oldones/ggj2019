using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Planet : MonoBehaviour
{
    public GameObject planet{ get; private set;}

    void Awake()
    {
        planet = gameObject;    
    }

    public static Planet CreatePlanet(Vector3 pos, string pname, int idx, WorldSpace space, SPlanetConfig config)
    {
        GameObject p = GameObject.Instantiate(space.planetPrefab);
        Planet pl = p.GetComponent<Planet>();
        p.transform.position = pos;
        p.transform.SetParent(space.transform);
        p.transform.localScale = config.scale;
        p.name = pname; //"Planet_" + idx;
        Material t = space.GetPlanetMaterial(-1);
        p.GetComponent<MeshRenderer>().material = t;
        GameObject.DestroyImmediate(p.GetComponent<SphereCollider>());
        pl.SetPlanet(p);
        return pl;
    }

    public void SetPlanet(GameObject o)
    {
        planet = o;
    }

    public void UpdatePlanet(ShipConsole sc, float dt)
    {
        Debug.LogFormat("Updating planet: {0}", planet.name);
    }


    public struct SPlanetConfig
    {
        public Vector3 scale;
    }
}
