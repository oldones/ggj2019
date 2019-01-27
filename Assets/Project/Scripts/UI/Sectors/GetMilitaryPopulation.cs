using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GetMilitaryPopulation : MonoBehaviour {


    Text textRef;
    Sector sectorRef;
    // Use this for initialization
    void Start()
    {
        textRef = GetComponent<Text>();
        sectorRef = GetComponentInParent<Sector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sectorRef != null)
        {
            textRef.text = sectorRef.getMilitaryPopulation().ToString();


        }
    }
}
