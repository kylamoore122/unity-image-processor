using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveSpeed = 1f;
    private float rotSpeed = 5f;
    private float scrollSpeed = 2.5f;
    private float camSensitivity = 0.25f;
    private Vector3 moveOrigin = new Vector3(0f, 0f, 0f);
    private bool isPanning = false;
    private float scrollAxis = 0f;

    void Update () 
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) 
        {
            transform.position += moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0) 
        {
            scrollAxis = Input.GetAxis("Mouse ScrollWheel");

            transform.position += scrollSpeed * new Vector3(0f, 0f, -scrollAxis);

            if (scrollAxis > 0)
            {
                //transform.position = Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), -scrollSpeed);
                //GetComponent<Camera>().fieldOfView--;
            }
            if (scrollAxis < 0)
            {
                //transform.position = Vector3.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), scrollSpeed);
                //GetComponent<Camera>().fieldOfView++;
            }
        }

        if (Input.GetMouseButton(1))
        {
            //Debug.Log("[Mouse down 1...]");
            //transform.LookAt(moveOrigin);
            moveOrigin = Input.mousePosition;
            isPanning = true;
        }
        else if (!Input.GetMouseButton(1))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            float maxVal = 30f;
            float minVal = -30f;
            float rotX = transform.rotation.eulerAngles.x;
            float rotY = transform.rotation.eulerAngles.y;

            transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * rotSpeed, Input.GetAxis("Mouse X") * rotSpeed, 0));
            //Debug.Log("eulerAngle X:" + transform.rotation.eulerAngles.x);
            //Debug.Log("eulerAngle Y:" + transform.rotation.eulerAngles.y);

            // Custom euler clamps
            if (transform.rotation.eulerAngles.x <= maxVal || transform.rotation.eulerAngles.x >= 360f + minVal)
            {
                rotX = transform.rotation.eulerAngles.x;
            }
            if (transform.rotation.eulerAngles.y <= maxVal || transform.rotation.eulerAngles.y >= 360f + minVal)
            {
                rotY = transform.rotation.eulerAngles.y;
            }

            transform.rotation = Quaternion.Euler(rotX, rotY, 0);
        }
    }
}
