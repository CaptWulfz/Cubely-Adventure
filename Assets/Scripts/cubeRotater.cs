using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeRotater : MonoBehaviour
{
    [SerializeField] GameObject cubley;
    [SerializeField] GameObject controller;
    [SerializeField] Rigidbody cubeRB;
    
    bool moveUP = false,
         moveDown = false,
         moveLeft = false,
         moveRight = false;
    bool ascend = false;
    // Use this for initialization
    void Start()
    {
    }

    private IEnumerator Rotate(Vector3 angles, float duration)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;
        for (float t = 0; t < duration; t += Time.deltaTime){
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / duration);
            if (moveUP){
                Vector3 moving = this.transform.localPosition;
                moving.z += 2 / duration * Time.deltaTime;
                this.transform.localPosition = moving;
            }
            else if (moveDown){
                Vector3 moving = this.transform.localPosition;
                moving.z -= 2 / duration * Time.deltaTime;
                this.transform.localPosition = moving;
            }
            else if (moveLeft){
                Vector3 moving = this.transform.localPosition;
                moving.x -= 2 / duration * Time.deltaTime;
                this.transform.localPosition = moving;
            }
            else if (moveRight){
                Vector3 moving = this.transform.localPosition;
                moving.x += 2 / duration * Time.deltaTime;
                this.transform.localPosition = moving;
            }
            yield return null;
        }
        transform.rotation = endRotation;

        cubley.transform.parent = null;
        if (moveUP)
        {
            transform.Rotate(-90, 0, 0);
        }
        else if (moveDown)
        {
            transform.Rotate(90, 0, 0);
        }
        else if (moveLeft)
        {
            transform.Rotate(0, 0, -90);
        }
        else if (moveRight)
        {
            transform.Rotate(0, 0, 90);
        }
        cubley.transform.parent = controller.transform;

        moveUP = false;
        moveDown = false;
        moveLeft = false;
        moveRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ascend && !(moveUP || moveDown || moveLeft || moveRight)){
            if (Input.GetKeyDown(KeyCode.W)){
                moveUP = true;
                StartCoroutine(Rotate(new Vector3(90, 0, 0), 0.5f));
            }
            if (Input.GetKeyDown(KeyCode.S)){
                moveDown = true;
                StartCoroutine(Rotate(new Vector3(-90, 0, 0), 0.5f));
            }
            if (Input.GetKeyDown(KeyCode.A)){
                moveLeft = true;
                StartCoroutine(Rotate(new Vector3(0, 0, 90), 0.5f));
            }
            if (Input.GetKeyDown(KeyCode.D)){
                moveRight = true;
                StartCoroutine(Rotate(new Vector3(0, 0, -90), 0.5f));
            }
            
        }
        else if(ascend){
            Vector3 moving = this.transform.localPosition;
            transform.RotateAround(this.transform.position, new Vector3(0, 2, 0), 2);
            if (transform.localPosition.y < 40)
                moving.y += 0.0111111111111111f * 4;
            this.transform.localPosition = moving;
            cubeRB.useGravity = false;
            
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "PreassurePlate")
        {
            ascend = true;
        }
    }
}