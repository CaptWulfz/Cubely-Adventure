using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField] public Material[] materials;
    private Dictionary<string, Material> cubeEffectMaterials;

    private float rollDuration = 0.3f;
    private int jumpHeight = 3;
    private bool canStartRolling = true;
    private float[] possibleAngles = { 0, 90, 180, 270, 360, -90, -180, -270, -360};
    private Transform prevCubeTransform;
    
    private GameObject cubeSide;
    private string effectName = "Base";
    private bool isFalling = false;
    Vector3 lockRotation;
    Vector3 lockPosition;

    private bool jumpRight = false;
    private bool jumpLeft = false;
    private bool jumpForward = false;
    private bool jumpBack = false;

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
        if (canStartRolling && !isFalling) {
            if (Input.GetKey(KeyCode.W))
                StartCoroutine(Roll(Vector3.forward));
            else if (Input.GetKey(KeyCode.A))
                StartCoroutine(Roll(Vector3.left));
            else if (Input.GetKey(KeyCode.S))
                StartCoroutine(Roll(Vector3.back));
            else if (Input.GetKey(KeyCode.D))
                StartCoroutine(Roll(Vector3.right));
        }

        if (jumpRight) {
            transform.position = transform.position + Vector3.right + (Vector3.up * jumpHeight);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 180);
            jumpRight = false;
            canStartRolling = true;
        } else if (jumpLeft) {
            transform.position = transform.position + Vector3.left + (Vector3.up * jumpHeight);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 180);
            jumpLeft = false;
            canStartRolling = true;
        } else if (jumpForward) {
            transform.position = transform.position + Vector3.forward + (Vector3.up * jumpHeight);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + 180, transform.eulerAngles.y, transform.eulerAngles.z);
            jumpForward = false;
            canStartRolling = true;
        } else if (jumpBack) {
            transform.position = transform.position + Vector3.back + (Vector3.up * jumpHeight);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 180, transform.eulerAngles.y, transform.eulerAngles.z);
            jumpBack = false;
            canStartRolling = true;
        }


        if (isFalling) {
            transform.rotation = Quaternion.Euler(lockRotation.x, lockRotation.y, lockRotation.z);
            transform.position = new Vector3(lockPosition.x, transform.position.y, lockPosition.z);

            checkIfFalling();
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

        checkIfFalling();

        canStartRolling = true;
    }

    private void checkIfFalling() {
        RaycastHit hit;
        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        Ray downRay = new Ray(transform.position, -Vector3.up);

        if (Physics.Raycast(downRay, out hit, Mathf.Infinity, layerMask)) {
            if (hit.distance > 0.51) {
                isFalling = true;
                lockRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                lockPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            }
            else if (hit.distance <= 0.51)
                isFalling = false;
        }
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


    private string checkSide(string collisionName) {
        string effectName = "Base";

        switch (collisionName) {
            case "Water":
                effectName = "Water";
                break;
            case "SpringRight":
                effectName = "Base";
                jumpRight = true;
                canStartRolling = false;
                break;
            case "SpringLeft":
                effectName = "Base";
                jumpLeft = true;
                canStartRolling = false;
                break;
            case "SpringForward":
                effectName = "Base";
                jumpForward = true;
                canStartRolling = false;
                break;
            case "SpringBack":
                effectName = "Base";
                jumpBack = true;
                canStartRolling = false;
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

        /*
        RaycastHit hit;
        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        Ray downRay = new Ray(transform.position, -Vector3.up);

        if (Physics.Raycast(downRay, out hit, Mathf.Infinity, layerMask)) {
            Debug.Log(hit.distance);
            if (hit.distance > 0.51) {
                isFalling = true;
                lockRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (hit.distance <= 0.51)
                isFalling = false;
        }
        */
        changeSideTexture(cubeSide, effectName);  
    }
    
}
