using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensivity = 1f;
    public bool invertLook;
    public float moveSpeed = 5f;
    

    private float verticalRotStore;
    private Vector2 mouseInput;
    private Vector3 moveDir, movement;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x,transform.rotation.eulerAngles.z);
        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -90f, 90f);
        if(invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(viewPoint.rotation.eulerAngles.x, viewPoint.rotation.eulerAngles.y, verticalRotStore);
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(viewPoint.rotation.eulerAngles.x, viewPoint.rotation.eulerAngles.y, -verticalRotStore);
        }

        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
