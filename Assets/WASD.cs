using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class WASD : MonoBehaviour
{
    [Tooltip("Rotation speed multiplier. 1 is default.")]
    public float rotationSpeed = 1;
    public float transformSpeed = 1;
    public float mouseSpeed = 4;
    public bool useMouseRotation = true;

    float modTransformSpeed;

    // Use this for initialization
    void Start()
    {
        modTransformSpeed = transformSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //speed up
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            modTransformSpeed = transformSpeed * 2;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            modTransformSpeed = transformSpeed;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            modTransformSpeed = transformSpeed * 5;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            modTransformSpeed = transformSpeed;
        }


        //arrow key rotation
        if (Input.GetKey(KeyCode.RightArrow))
        {
            gameObject.transform.Rotate(Vector3.up, rotationSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            gameObject.transform.Rotate(Vector3.up, -rotationSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), -1 * rotationSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), 1 * rotationSpeed);
        }

        //mouse camera rotation
        if (useMouseRotation)
        {
            MouseMove();
        }
        else if (Input.GetMouseButton(1))
        {
            MouseMove();
        }

        //WASD strafing
        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += new Vector3(transform.TransformDirection(Vector3.forward).x, 0, transform.TransformDirection(Vector3.forward).z) / 50 * modTransformSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += transform.TransformDirection(Vector3.left) / 50 * modTransformSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += new Vector3(transform.TransformDirection(Vector3.back).x, 0, transform.TransformDirection(Vector3.back).z) / 50 * modTransformSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += transform.TransformDirection(Vector3.right) / 50 * modTransformSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.localPosition += Vector3.down / 50 * modTransformSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.localPosition += Vector3.up / 50 * modTransformSpeed;
        }
    }

    void MouseMove()
    {
        gameObject.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseSpeed, Space.World);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), -Input.GetAxis("Mouse Y") * mouseSpeed);
    }
}
