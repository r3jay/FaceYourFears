using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    // Sensitivity modifiers
    [SerializeField] int sensHori;
    [SerializeField] int sensVert;

    // Lock so camera can't flip
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    // invert controls
    [SerializeField] bool invert;

    float xRotation;


    // Start is called before the first frame update
    void Start()
    {
        // locks the cursor onto the screen/window
        Cursor.lockState = CursorLockMode.Locked;
        // Set cursor to invisible
        Cursor.visible = false;
    }

    // LateUpdate will begin after Update
    // stops jerkiness from camera/playercontroller fighting over control
    void LateUpdate()
    {
        // get the input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHori;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVert;

        if (invert)
        {
            xRotation += mouseY;
        }
        else
        {
            xRotation -= mouseY;
        }


        // clamp rotation
        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        // rotate the camera on the x axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // rotate the player on the y axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
