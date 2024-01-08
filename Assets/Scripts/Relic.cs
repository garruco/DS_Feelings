using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artifact;

public class Relic : MonoBehaviour
{
    public RelicParent parent;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CollectionDone(){
        
    }

    void StartCollection(){
        parent.CollectionDone();
    }

    void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                parent.OnTrigger(collision);
            }
        }
}
