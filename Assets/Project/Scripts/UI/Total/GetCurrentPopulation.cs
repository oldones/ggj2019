using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCurrentPopulation : MonoBehaviour {

    Text textRef;

    // Use this for initialization
    void Start () {
        textRef = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        textRef.text = SectorManager.getInstance().getCurrentPopulation().ToString();

    }
}
