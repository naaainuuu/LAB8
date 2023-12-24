using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;
    
    public float moveSpeed = 5f, runSpeed = 8f;
    public float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon;  

    private Camera cam;

    public float jumpForce = 12f, gravityMod = 2.5f;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundlayers;

    public GameObject bulletImpact;
    //public float timeBetweenShots = 0.5f;
    private float shotCounter;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public float maxHeat = 10f, /*heatPerShot = .1f,*/ coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overHeated;

    public Gun[] allGuns;
    private int selectedGun;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
        UIController.instance.weaponTempSlider.maxValue = maxHeat;
        SwitchGun();
        Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        transform.position = newTrans.position;
        transform.rotation = newTrans.rotation;
    }

    void Update()
    {
        //Viewing Around using mouse
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x,transform.rotation.eulerAngles.z);
        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -90f, 90f);
        //Inverting Mouse look
        if(invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }

        //movement
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        //Sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }

        float yVel = movement.y;
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVel;

        //checking grounded
        if(charCon.isGrounded)
        {
            movement.y = 0f;
        }

        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundlayers);

        //Jumping
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }

        //gravity
        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;
        charCon.Move(movement * Time.deltaTime);

        if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;
            if(muzzleCounter <= 0)
            {
                allGuns[selectedGun].muzzleFlash.SetActive(false);
            }
            
        }
        

        //shooting
        if(!overHeated)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
            if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
            {
                shotCounter -= Time.deltaTime;
                if (shotCounter <= 0)
                {
                    Shoot();
                }
            }
            heatCounter -= coolRate * Time.deltaTime;
        }
        else
        //overheating
        {
            heatCounter -= overheatCoolRate * Time.deltaTime;
            if (heatCounter <= 0)
            {
                overHeated = false;

                UIController.instance.overHeatedMessage.gameObject.SetActive(false);
            }
        }
        if (heatCounter < 0)
        {
            heatCounter = 0;
        }

        UIController.instance.weaponTempSlider.value = heatCounter;

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.RightControl))
        {
            selectedGun++;
            if(selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }
            SwitchGun();
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if(selectedGun < 0)
            { 
                selectedGun = allGuns.Length - 1; 
            }
            SwitchGun();
        }

        for(int i = 0; i < allGuns.Length; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                selectedGun = i;
                SwitchGun();
            }
        }
        
        //unlcoking cursor in build
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if(Cursor.lockState == CursorLockMode.None)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }

    private void Shoot()
    {
        //loacting where the shot is hit using raycast
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("We hit " + hit.collider.gameObject.name);
            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(bulletImpactObject, 5f);
        }

        //automatic shooting
        shotCounter = allGuns[selectedGun].timeBetweenShots;

        //overheating
        heatCounter += allGuns[selectedGun].heatPerShot;
        if(heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;
            overHeated = true;

            UIController.instance.overHeatedMessage.gameObject.SetActive(true);
        }
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;
    }

    void LateUpdate()
    {
        //moving camera to the correct position
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    void SwitchGun()
    {
        foreach(Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);

        allGuns[selectedGun].muzzleFlash.SetActive(false);

    }

}
