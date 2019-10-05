﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] public Material[] materials;
    private Dictionary<string, Material> cubeEffectMaterials;
    private float rollDuration = 0.3f;
    private bool canStartRolling = true;
    private float[] possibleAngles = { 0, 90, 180, 270, 360, -90, -180, -270, -360};
    private Transform prevCubeTransform;

    public GameObject lCheck;
    public GameObject rCheck;
    public GameObject fCheck;
    public GameObject bCheck;
    private GameObject cubeSide;
    private string effectName = "Base";
    private bool isDoneMoving;
    private bool isBlocked;

    // Start is called before the first frame update
    void Start() {
        cubeEffectMaterials = new Dictionary<string, Material>();
        cubeEffectMaterials.Add("Base", materials[0]);
        cubeEffectMaterials.Add("Water", materials[1]);
        prevCubeTransform = transform;
        cubeSide = transform.Find("Bottom").gameObject;
    }

    // Update is called once per frame
    void Update() {
        if (canStartRolling)
        {
            if (Input.GetKey(KeyCode.W) && (fCheck.GetComponent<TriggerCheck>().isBlocked == false))
                StartCoroutine(Roll(Vector3.forward));
            else if (Input.GetKey(KeyCode.A) && (lCheck.GetComponent<TriggerCheck>().isBlocked == false))
                StartCoroutine(Roll(Vector3.left));
            else if (Input.GetKey(KeyCode.S) && (bCheck.GetComponent<TriggerCheck>().isBlocked == false))
                StartCoroutine(Roll(Vector3.back));
            else if (Input.GetKey(KeyCode.D) && (rCheck.GetComponent<TriggerCheck>().isBlocked == false))
                StartCoroutine(Roll(Vector3.right));

            if (Input.GetKeyUp(KeyCode.Space))
                Debug.Log(cubeEffectMaterials["Water"].name);
        }
        else
        {
            isDoneMoving = false;
            MoveCheck(isDoneMoving);
        }

    }

    private IEnumerator Roll(Vector3 direction) {
        canStartRolling = false;
        //Debug.Log("Moving");
        float rollDecimal = 0;
        float rollAngle = 90;
        float halfWidth = transform.localScale.x / 2;
        Vector3 pointAround = transform.position + (Vector3.down * halfWidth) + (direction * halfWidth);
        Vector3 rollAxis = Vector3.Cross(Vector3.up, direction);
        
        Vector3 endPosition = transform.position + direction;

        float oldAngle = 0;

        while (rollDecimal < rollDuration) {
            yield return new WaitForEndOfFrame();
            rollDecimal += Time.deltaTime;
            float newAngle = (rollDecimal / rollDuration) * rollAngle;
            float rotateThrough = newAngle - oldAngle;
            oldAngle = newAngle;

            transform.RotateAround(pointAround, rollAxis, rotateThrough);
        }

        transform.position = endPosition;

        Quaternion endRotation = Quaternion.Euler(setRotation(transform.eulerAngles.x), setRotation(transform.eulerAngles.y), setRotation(transform.eulerAngles.z));
        transform.rotation = endRotation;

        prevCubeTransform = transform;

        canStartRolling = true;
    }

    private float setRotation(float currAngle) {
        float lowestDiff = -99; //Default Value
        int index = 0;
        for (int i = 0; i < possibleAngles.Length; i++) {
            float angle = possibleAngles[i];
            float diff = currAngle - angle;
            if (diff == -99) {
                lowestDiff = diff;
                index = i;
            } else if (Mathf.Abs(diff) < Mathf.Abs(lowestDiff)) {
                lowestDiff = diff;
                index = i;
            }
        }

        return possibleAngles[index];
    }

    private void changeSideTexture(GameObject side, string effectName) {
        MeshRenderer renderer = side.GetComponent<MeshRenderer>();
        renderer.material = cubeEffectMaterials[effectName];
        isDoneMoving = true;
        MoveCheck(isDoneMoving);
        //Debug.Log("Done moving");
    }


    private string checkSide(string collisionName) {
        string effectName = "Base";
        
        switch (collisionName) {
            case "Water":
                effectName = "Water";
                break;
            default:
                effectName = "Base";
                break;
        }
        return effectName;
    }
    private void OnCollisionEnter(Collision collision) {
        
        ContactPoint[] points = collision.contacts;

        int i = 0;
        bool found = false;
        while (i < points.Length && !found) {
            string name = points[i].thisCollider.name;

            if (name.Equals(TouchNames.TOP_TOUCH)) {
                cubeSide = transform.Find("Top").gameObject;
                /*
                RaycastHit hit;
                if (Physics.Raycast(cubeSide.transform.position, cubeSide.transform.TransformDirection(Vector3.up), out hit)) {
                    Debug.Log("Did Hit");
                    if (hit.collider.tag == "Obstacle") {
                        Debug.Log("Vector direc: " + Vector3.left);
                        Debug.Log("Cube Direc: " + cubeSide.transform.TransformDirection(Vector3.up));
                        if (Vector3.Equals(Vector3.left, cubeSide.transform.TransformDirection(Vector3.up))) {
                            Debug.Log("There is an obstacle on the left");
                        }
                    }
                }
                */
                effectName = checkSide(collision.gameObject.name);
                found = true;
            }
            if (name.Equals(TouchNames.BOTTOM_TOUCH)) {
                cubeSide = transform.Find("Bottom").gameObject;
                effectName = checkSide(collision.gameObject.name);
                found = true;
            }
            if (name.Equals(TouchNames.LEFT_TOUCH)) {
                cubeSide = transform.Find("Left").gameObject;
                effectName = checkSide(collision.gameObject.name);                 
                found = true;
            }
            if (name.Equals(TouchNames.RIGHT_TOUCH)) {
                cubeSide = transform.Find("Right").gameObject;
                effectName = checkSide(collision.gameObject.name);
                found = true;
            }
            if (name.Equals(TouchNames.FRONT_TOUCH)) {
                cubeSide = transform.Find("Front").gameObject;
                effectName = checkSide(collision.gameObject.name);
                found = true;
            }
            if (name.Equals(TouchNames.BACK_TOUCH)) {
                cubeSide = transform.Find("Back").gameObject;
                effectName = checkSide(collision.gameObject.name);
                found = true;
            }
            else
                i++;
        }
        
        changeSideTexture(cubeSide, effectName);  
    }

    private void MoveCheck(bool doneMoving)
    {
        if (doneMoving)
        {
            //Debug.Log("WAITING");

            lCheck.GetComponent<BoxCollider>().enabled = true;
            rCheck.GetComponent<BoxCollider>().enabled = true;
            fCheck.GetComponent<BoxCollider>().enabled = true;
            bCheck.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            //Debug.Log("MOVING");

            lCheck.GetComponent<TriggerCheck>().isBlocked = false;
            rCheck.GetComponent<TriggerCheck>().isBlocked = false;
            fCheck.GetComponent<TriggerCheck>().isBlocked = false;
            bCheck.GetComponent<TriggerCheck>().isBlocked = false;
            lCheck.GetComponent<BoxCollider>().enabled = false;
            rCheck.GetComponent<BoxCollider>().enabled= false;
            fCheck.GetComponent<BoxCollider>().enabled = false;
            bCheck.GetComponent<BoxCollider>().enabled = false;
        }
    }
    
}
