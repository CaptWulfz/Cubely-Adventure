using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSideCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(this.gameObject.name);
        if (collision.gameObject.name == "Plane")
        {
            Debug.Log(this.gameObject.name + " is Hitting the Plane");
        }
    }
}
