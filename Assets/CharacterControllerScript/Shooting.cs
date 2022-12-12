using UnityEngine;
using Cinemachine;
using System;
using System.Collections;

public class Shooting : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;
    private void Start()
    {
       GetComponent<Player>().canfire = false;
    }
    public CinemachineFreeLook tppCamera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)&& GetComponent<Player>().canfire)
        {
            Shoot();
        }
       // RaycastHit hit;
        Debug.DrawRay(tran.position, fpsCam.transform.forward, Color.blue,1, true);
    }
    public Transform tran;
    private void Shoot()
    {
        RaycastHit hit;
       // Debug.DrawRay(tran.position, fpsCam.transform.forward, Color.blue, range);
        if(Physics.Raycast(tran.position, fpsCam.transform.forward, out hit, range)){
            Debug.Log(hit.transform.name);
            Target target=hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }
            if (hit.rigidbody!=null)
            {
                hit.rigidbody.AddForce(-hit.normal*impactForce);
            }
           //GameObject impactGo=Instantiate(ImpactEffect, hit.point,Quaternion.LookRotation(hit.normal));
           // Destroy(impactGo, 2f);
        }
    }
    //public GameObject ImpactEffect;
   [SerializeField] private float impactForce=30f;


    /*--------------------Laser-------------------------------*/
    public Camera playerCamera;
    public Transform laserOrigin;
    public float gunRange = 50f;
    public float fireRate = 0.2f;
    public float laserDuration = 0.05f;

    LineRenderer laserLine;
    float fireTimer;

    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        fireTimer += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && fireTimer > fireRate)
        {
            fireTimer = 0;
            laserLine.SetPosition(0, laserOrigin.position);
            RaycastHit hit;
            if (Physics.Raycast(tran.position, playerCamera.transform.forward, out hit, gunRange))
            {
                laserLine.SetPosition(1, hit.point);
                //Destroy(hit.transform.gameObject);
            }
            else
            {
                laserLine.SetPosition(1, tran.position + (playerCamera.transform.forward * gunRange));
            }
            StartCoroutine(ShootLaser());
        }
    }

    IEnumerator ShootLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }
}

