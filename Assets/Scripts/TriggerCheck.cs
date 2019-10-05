using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggering");
        if (other.gameObject.tag == "Obstacle")
            Debug.Log(this.name + " is Colliding with an Obstacle");
    }
}
