using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    public int damage;
    public float timeBetweenShoting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphics;
    public CameraShake cameraShake;
    public float cameraShakeMagnitude, cameraShakeDuration;
    public TextMeshProUGUI text;

    //Sound
    public AudioSource shoot;
    public AudioSource afterShoot;

    //Animation
    public Animator anim;
    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShot = true;
    }
    private void Update()
    {
        MyInput();

        //SetText
        text.SetText(bulletsLeft + " / " + magazineSize);   
    }
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Shoot
        if (readyToShot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            shoot.PlayOneShot(shoot.clip);
            Invoke("SoundEfect", 1f);
            anim.Play("Shotanim",0,0f);
            //anim.Play("SecondShotgunAnim",0,0f);
            Shoot();
        }
    }
    private void SoundEfect()
    {
        afterShoot.PlayOneShot(afterShoot.clip);
    }
    private void Shoot()
    {
        readyToShot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x,y,z);
        

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            if (rayHit.collider.CompareTag("Enemy"))
                rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
        }

        //ShakeCamera
        cameraShake.Shake(cameraShakeDuration, cameraShakeMagnitude);

        //Graphics
        GameObject newHole = Instantiate(bulletHoleGraphics, rayHit.point + rayHit.normal * 0.001f, Quaternion.identity) as GameObject;
        newHole.transform.LookAt(rayHit.point + rayHit.normal);
        Destroy(newHole, 5f);
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBetweenShoting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void ResetShot()
    {
        readyToShot = true;
    }
}
