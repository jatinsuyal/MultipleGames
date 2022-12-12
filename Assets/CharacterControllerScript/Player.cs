using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using System;


public class Player : MonoBehaviour
{
  //  public static Player instance;
    public Transform freeCam;
    public CharacterController cc;
    public bool groundedPlayer;
    Vector3 playerVelocity;
    public Animator anim;
    public float gravityValue=-10f, playerSpeed=5f;
    public float xVal, yVal;
    public GameObject tppPoint;
    public CinemachineFreeLook tppCamera;
    public CinemachineVirtualCamera fppCamera;
    CinemachinePOV pov;
    public bool canWork=true,GunHolding;
    
    public float sensi;
    [SerializeField]
    float animY, animX;
    float forwardSpeedMultiplier = 1f;
    Vector3 movementValue = Vector3.zero;
    public bool canJump = true;
  //  public GameObject jumpBtn;
    public RuntimeAnimatorController normalController,runController;
    public bool canRun = false;
    Guns guns;
    void Awake() {
    }
    void Start() {
      //  instance = this;
        if (canRun)
        {
            anim.runtimeAnimatorController = runController;
        }
        else {
            anim.runtimeAnimatorController = normalController;

        }
        canWork = true;
        pov = fppCamera.GetCinemachineComponent<CinemachinePOV>();
        GunHolding =false;
        //tppCamera.m_YAxis.m_InputAxisName = "";
        guns = Guns.Gun1;
    }


    void Update()
    {
        if (!canWork) return;
        playerMovementFn();
        Vector3 targetRot = new Vector3(0f, freeCam.transform.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 30);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShiftButtonDown();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ShiftButtonUp();
        }
        /*-Added-*/
        if (Input.GetKeyDown(KeyCode.Alpha1)) { guns = Guns.Gun1; ChangeGun(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { guns = Guns.Gun2; ChangeGun(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { guns = Guns.Gun3; ChangeGun(); }

    }
    //[HideInInspector]
    public bool canfire=false;
    public bool isworking;
    float speed=0.5f;
    private IEnumerator CanFire()
    {
        yield return new WaitForSeconds(2);
        canfire = true;
    }
    void playerMovementFn()
    {
        if (isworking)
        {
            groundedPlayer = cc.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                anim.SetBool("jumpUp", false);

                jumpPointer = jumpAllowed;
                playerVelocity.y = 0f;
            }
            if (groundedPlayer)
            {
                //yVal = SimpleInput.GetAxis("Vertical");
                yVal = Input.GetAxis("Vertical");
            }
            if (Input.GetMouseButtonDown(1)){ StartCoroutine(CanFire()); xVal = 1;}
            if (Input.GetMouseButtonUp(1)){ StopAllCoroutines(); canfire = false;xVal = 0; }
            if (canRun)
            {
                if (yVal < .8f)
                {
                    forwardSpeedMultiplier = .5f;
                }
                else
                {
                    forwardSpeedMultiplier = 1.2f;
                }
            }
            else
            {
                forwardSpeedMultiplier = .8f;
                //speed = 0.5f;
            }

            Vector2 dir = new Vector2(/*xVal*/0, yVal);
            dir = dir.normalized;

            animY = Mathf.Lerp(animY, yVal, Time.deltaTime * 10);
            animX = Mathf.Lerp(animX, xVal, Time.deltaTime * 10);

            anim.SetFloat("Run", animY * speed);
            anim.SetFloat("Gun", animX);

            if (cc.enabled && yVal>=0)
            {
                if (groundedPlayer) movementValue = transform.right * dir.x + (transform.forward * dir.y) * forwardSpeedMultiplier;
                cc.Move(movementValue * Time.deltaTime * playerSpeed);

                playerVelocity.y += gravityValue * Time.deltaTime;
                cc.Move(playerVelocity * Time.deltaTime);//just for gravity
            }
        }
    }
    public bool isDragDown;
    public void dragDown() {
        isDragDown = true;
        
    }
    public void dragUp() {
        isDragDown = false;
    }
    Slider sensiSlider;
    Text v;
    public void sensiChange() {
        sensi = sensiSlider.value;
        v.text = sensi.ToString();
    }
    public bool isFpp = false;
    public void changeCamera() {
        if (isFpp)
        {
            isFpp = false;
            

            tppCamera.m_XAxis.Value = pov.m_HorizontalAxis.Value;
            tppCamera.m_YAxis.Value = 0.6f;// pov.m_VerticalAxis.Value;

            tppCamera.m_Priority = 10;
            fppCamera.m_Priority = 5;
        }
        else {
            isFpp = true;

            pov.m_HorizontalAxis.Value= tppCamera.m_XAxis.Value;
            pov.m_VerticalAxis.Value = 0;// tppCamera.m_YAxis.Value;

            tppCamera.m_Priority = 5;
            fppCamera.m_Priority = 10;
        }
    }

    public int jumpAllowed = 1;
    int jumpPointer;
    public float jumpHeight=10f;

    public void jumpPressed() {
        if (!canJump) return;
        
        if (jumpPointer == 1)
        {
            jumpPointer -= 1;
            anim.SetBool("jumpUp", true);

            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
    }
    /*-------------------------------------------Newly Added Coded---------------------------------------*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
           // GameManager.Instance.ShowEntryButtons(true);
        }
        if (other.gameObject.tag == "PickUpItems")
        {
            GunHolding = true;
            ChangeGun();
            CheckCanGun(true);
        }
    }

    private void ChangeGun()
    {
        for (int i = 0; i < AddGuns.Count; i++)
        {
            AddGuns[i].SetActive(false);
        }
        if (guns == Guns.Gun1)
        {
            AddGuns[0].SetActive(true);
            print("1 pressed");
        }
        else if (guns == Guns.Gun2)
        {
            print("2 pressed");

            AddGuns[1].SetActive(true);
        }
        else if (guns == Guns.Gun3)
        {
            print("3 pressed");

            AddGuns[2].SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
           

        }
        if (other.gameObject.tag == "PickUpItems")
        {
           // GunHolding = false;
           // Glacier.SetActive(false);
        }
    }
    
    public void ShiftButtonDown()
    {
        canRun = true;
        speed = 1f;
    }
    void CheckCanGun(bool canRun)
    {
        if (canRun)
        {
            anim.runtimeAnimatorController = runController;
        }
        else
        {
            anim.runtimeAnimatorController = normalController;

        }
    }
    public void ShiftButtonUp()
    {
        canRun = false;
        speed=0.5f;
    }
    /*--Newly Added After giving denpok-*/
    public List<GameObject> AddGuns;
    public enum Guns { Gun1, Gun2,Gun3 }

}
