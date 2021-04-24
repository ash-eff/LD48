using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bimmy : PlayerCharacter
{
    public bool isHoldingBody;
    public Zombie zombie;
    public Hole currentHole;
    public bool canPlaceBody;
    public Transform carryPosition;
    public GameObject pickupIndicator;
    public GameObject buryIndicator;
    public bool isShooting = false;
    public float rateOfFire;
    public float lastShot = 0;
    public GameObject bulletPrefab;
    public Transform muzzlePosition;

    public override void Update()
    {
        base.Update();
        if (isShooting)
        {
            FireWeapon(angleToCursor);
        }
    }
    
    public override void ActionOne()
    {
        if (isSelected && !isHoldingBody)
        {
            isShooting = true;
        }
    }

    public override void ActionOneCancelled()
    {
        isShooting = false;
    }

    public override void ActionTwo()
    {
        if (isSelected)
        {
            if (isHoldingBody)
            {
                if (canPlaceBody)
                {
                    isHoldingBody = false;
                    Destroy(zombie.gameObject);
                    zombie = null;
                    currentHole.hasBody = true;
                }
                else
                {
                    isHoldingBody = false;
                    zombie.transform.position = transform.position;
                    zombie.transform.parent = null;
                }
            }
            else
            {
                if (zombie != null && zombie.isDead)
                {
                    pickupIndicator.SetActive(false);
                    zombie.transform.position = carryPosition.position;
                    zombie.transform.parent = carryPosition;
                    isHoldingBody = true;
                }
            }
        }
    }
    
    private void FireWeapon(float rot)
    {
        if (Time.time > rateOfFire + lastShot && !isHoldingBody)
        {
            //cam.CameraShake();
            GameObject obj = Instantiate(bulletPrefab, muzzlePosition.position, Quaternion.Euler(0,0, rot));
            //obj.GetComponent<Bullet>().rot = rot;
            lastShot = Time.time;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHoldingBody)
        {
            if (other.CompareTag("Hole"))
            {
                currentHole = other.gameObject.GetComponent<Hole>();
                if (!currentHole.hasBody)
                {
                    canPlaceBody = true;
                    buryIndicator.SetActive(true);
                }
            }
        }


        if (other.CompareTag("Zombie"))
        {
            zombie = other.gameObject.GetComponent<Zombie>();
            if(zombie.isDead)
                pickupIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hole"))
        {
            canPlaceBody = false;
            buryIndicator.SetActive(false);
        }
        
        if (other.CompareTag("Zombie"))
        {
            if (!isHoldingBody)
            {
                zombie = null;
                pickupIndicator.SetActive(false);
            }
                
        }
    }
}