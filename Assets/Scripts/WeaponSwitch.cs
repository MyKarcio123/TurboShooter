using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour
{
    public int selectedWeapon = 0;
    int curentWeapon = 0;

    private void Start()
    {
        SelectWeapon();
    }
    private void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon +1> transform.childCount-1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
            SelectWeapon();
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon -1 < 0)
            {
                selectedWeapon = transform.childCount-1;
            }
            else
            {
                selectedWeapon--;
            }
            SelectWeapon();
        }else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
            SelectWeapon();
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
            SelectWeapon();
        }else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
            SelectWeapon();
        }
    }
    void SelectWeapon()
    {
        transform.GetChild(curentWeapon).gameObject.SetActive(false);
        transform.GetChild(selectedWeapon).gameObject.SetActive(true);
        curentWeapon = selectedWeapon;
    }
}
