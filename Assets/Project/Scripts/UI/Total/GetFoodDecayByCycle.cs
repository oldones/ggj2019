using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetFoodDecayByCycle : MonoBehaviour {

    Text textboxRef;

    // Use this for initialization
    void Start()
    {
        textboxRef = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        textboxRef.text = ResourceManager.getInstance().GetFoodConsumption().ToString();
    }
}
