using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GetSurvivalRatePercentage : MonoBehaviour {

    Text textRef;
    ResourceManager resourceMngRef;
    Sector sectorRef;


	// Use this for initialization
	void Start () {
        textRef = GetComponent<Text>();
        resourceMngRef = GameObject.FindObjectOfType<ResourceManager>();
        sectorRef = GetComponentInParent<Sector>();
	}
	
	// Update is called once per frame
	void Update () {
        try
        {
            textRef.text = sectorRef.getDeathProbability().ToString();
        }
        catch(System.NullReferenceException e)
        {
            print("GG");
        }
        
        
	}
}
