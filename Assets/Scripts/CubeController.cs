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

    private GameObject cubeSide;
    //private string effectName = "Base";

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
        if (canStartRolling) {
            if (Input.GetKey(KeyCode.W))
                StartCoroutine(Roll(Vector3.forward));
            else if (Input.GetKey(KeyCode.A))
                StartCoroutine(Roll(Vector3.left));
            else if (Input.GetKey(KeyCode.S))
                StartCoroutine(Roll(Vector3.back));
            else if (Input.GetKey(KeyCode.D))
                StartCoroutine(Roll(Vector3.right));

            if (Input.GetKeyUp(KeyCode.Space))
                Debug.Log(cubeEffectMaterials["Water"].name);
        }
    }

    private IEnumerator Roll(Vector3 direction) {
        canStartRolling = false;

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
    }

    private IEnumerator checkCollide(Collider collider) {
        yield return new WaitForSeconds(0.00005f);
        string effectName = "Base";

        if (collider.name == "Water")
            effectName = "Water";
        else
            effectName = "Base";

        changeSideTexture(cubeSide, effectName);
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log(other.name);
        //Debug.Log(cubeSide.name);
        StartCoroutine(checkCollide(other));
    }

    private void OnCollisionEnter(Collision collision) {
        
        ContactPoint[] points = collision.contacts;

        int i = 0;
        bool found = false;
        while (i < points.Length && !found) {
            string name = points[i].thisCollider.name;

            if (name.Equals(TouchNames.TOP_TOUCH)) {
                Debug.Log("Top is Colliding");
                cubeSide = transform.Find("Top").gameObject;
                RaycastHit hit;
                if (Physics.Raycast(cubeSide.transform.position, cubeSide.transform.TransformDirection(Vector3.up), out hit)) {
                    Debug.Log("Did Hit");
                    if (hit.collider.name == "Obstacle") {
                        Debug.Log("Vector direc: " + Vector3.left);
                        Debug.Log("Cube Direc: " + cubeSide.transform.TransformDirection(Vector3.up));
                        if (Vector3.Equals(Vector3.left, cubeSide.transform.TransformDirection(Vector3.up))) {
                            Debug.Log("There is an obstacle on the left");
                        }
                    }
                }
                found = true;
            }
            else if (name.Equals(TouchNames.BOTTOM_TOUCH)) {
                cubeSide = transform.Find("Bottom").gameObject;
                found = true;
            }
            else if (name.Equals(TouchNames.LEFT_TOUCH)) {
                cubeSide = transform.Find("Left").gameObject;
                found = true;
            }
            else if (name.Equals(TouchNames.RIGHT_TOUCH)) {
                cubeSide = transform.Find("Right").gameObject;
                found = true;
            }
            else if (name.Equals(TouchNames.FRONT_TOUCH)) {
                cubeSide = transform.Find("Front").gameObject;
                found = true;
            }
            else if (name.Equals(TouchNames.BACK_TOUCH)) {
                cubeSide = transform.Find("Back").gameObject;
                found = true;
            }
            else
                i++;
        }
        
    }
    
}