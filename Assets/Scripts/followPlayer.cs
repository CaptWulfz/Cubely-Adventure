using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour {

    [SerializeField] private GameObject player;
    private float x_diff = 0;
    private float y_diff = 0;
    private float z_diff = 0;
    private bool enabled = true;


    // Use this for initialization
    void Start() {
        Vector3 pos = player.transform.position;
        x_diff = pos.x - transform.position.x;
        y_diff = pos.y - transform.position.y;
        z_diff = pos.z - transform.position.z;
    }

    // Update is called once per frame
    void Update() {
        if (enabled) {
            Vector3 pos = player.transform.position;
            pos.x -= x_diff;
            pos.y -= y_diff;
            pos.z -= z_diff;
            transform.position = pos;
        }
        
    }

    public void turnon() {
        enabled = true;
    }

    public void turnoff() {
        enabled = false;
    }
}
