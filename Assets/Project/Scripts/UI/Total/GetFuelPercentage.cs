using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetFuelPercentage : MonoBehaviour {

    Text textRef;
    ResourceManager resourceMngRef;

    // Use this for initialization
    void Start()
    {
        textRef = GetComponent<Text>();
        resourceMngRef = GameObject.FindObjectOfType<ResourceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        textRef.text = (resourceMngRef.getFuelPercent()).ToString();
    }
}
