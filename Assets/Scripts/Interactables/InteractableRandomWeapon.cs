using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableRandomWeapon : InteractableCostPointsBase
{
    public Dictionary<int, GameObject> m_weapons;
    public Transform m_boxTransform;
    private void Start()
    {
        m_weapons = new Dictionary<int, GameObject>();
        foreach (SO_Weapon weapon in GameManager.GetInstance().m_weaponsInGame.m_weaponsInGame)
        {
            GameObject go = Instantiate(weapon.m_model, Vector3.zero, Quaternion.identity, m_boxTransform);
            go.SetActive(false);

            m_weapons.Add(weapon.m_weaponID, go);
        }
    }

    public override bool Interact(PlayerController interactor)
    {
        if (!base.Interact(interactor))
        {
            return false;
        }
        if (_areaController != null)
        {
            _areaController.UnlockArea();
        }

        return true;
    }

    public override bool ShowInteract(PlayerController interactor, bool look)
    {
        base.ShowInteract(interactor, look);

        return true;
    }

    public override bool HideInteract(PlayerController interactor, bool look)
    {
        base.HideInteract(interactor, look);

        return true;
    }

    IEnumerator RandomWeaponAnim()
    {
        Dictionary<int, GameObject> availableWeapons = new Dictionary<int, GameObject>();

        foreach (var weapon in m_weapons)
        {
            if (!GameManager.GetInstance().m_playerController.HaveWeaponOnInventory(weapon.Key))
            {
                availableWeapons.Add(weapon.Key , weapon.Value);
            }
        }
        List<int> weaponsIDS = availableWeapons.Keys.ToList();

        float time = 2 / weaponsIDS.Count;

        
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < weaponsIDS.Count; j++)
            {
                if (j != 0)
                {
                    availableWeapons[weaponsIDS[j - 1]].SetActive(false);
                }
                availableWeapons[weaponsIDS[j]].SetActive(true);
                yield return new WaitForSeconds(time);
            }
            availableWeapons[weaponsIDS[weaponsIDS.Count - 1]].SetActive(false);
        }
        //Set active weapon
}
}
