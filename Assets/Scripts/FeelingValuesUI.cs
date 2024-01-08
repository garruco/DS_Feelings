using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeelingValuesUI : MonoBehaviour
{
    public GameObject UIFeelings;
    bool state = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tab)){
            state =! state;
            UIFeelings.SetActive(state);
        }
    }
}
