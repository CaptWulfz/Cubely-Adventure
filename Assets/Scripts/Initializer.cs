using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] GameObject cubely;
    GameObject top;     

    // Start is called before the first frame update
    void Start()
    {
        top = cubely.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        /*
        ContactPoint[] points = collision.contacts;
        int i = 0;
        bool found = false;
        while (i < points.Length && !found)
        {
            string name = points[i].thisCollider.name;
            if (name.Equals(TouchNames.TOP_TOUCH))
            {
                Debug.Log("Top");
                found = true;
            } else
            {
                i++;
            }
        }
        */
    }
}
