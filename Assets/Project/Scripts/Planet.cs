using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet
{
    public GameObject planet{ get; private set;}

    public Planet(Vector3 pos, int idx, WorldSpace space, SPlanetConfig config)
    {
        planet = GameObject.Instantiate(space.planetPrefab);
        planet.transform.position = pos;
        planet.transform.SetParent(space.transform);
        planet.transform.localScale = config.scale;
        planet.name = "Planet_" + idx;
        Material t = space.GetPlanetMaterial(-1);
        planet.GetComponent<MeshRenderer>().material = t;
        GameObject.DestroyImmediate(planet.GetComponent<SphereCollider>());

    }


    public struct SPlanetConfig
    {
        public Vector3 scale;
    }
}
