using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUpgradeWeapon : InteractableCostPointsBase
{
    public Dictionary<int, GameObject> m_weapons;
    public Transform m_weaponTransform;
    public Animator m_animator;
    private int m_currentWeapon = -1;
    bool _canInteract;
    Coroutine _weaponAnim;
    private void Start()
    {
        //m_weapons = new Dictionary<int, GameObject>();
        //foreach (SO_Weapon weapon in GameManager.GetInstance().m_weaponsInGame.m_weaponsInGame)
        //{
        //    GameObject go = Instantiate(weapon.m_model, m_weaponTransform.position, Quaternion.identity, m_weaponTransform);
        //    go.SetActive(false);

        //    m_weapons.Add(weapon.m_weaponID, go);
        //}
        _canInteract = true;
    }

    public override bool Interact(PlayerController interactor)
    {
        if (!_canInteract || GameManager.GetInstance().m_weaponsInGame.GetWeaponData(interactor.GetCurrentWeapon().GetWeaponID()).m_weaponUpgrade == null)
        {
            return false;
        }
        //if (m_currentWeapon != -1)
        //{
        //    GameManager.GetInstance().m_playerController.ChangeSlotWeapon(m_currentWeapon);
        //    interactor.GetCurrentWeapon().SetTotalAmmo(GameManager.GetInstance().m_weaponsInGame.GetWeaponData(m_currentWeapon).m_weaponStats.m_magazineAmmo, GameManager.GetInstance().m_weaponsInGame.GetWeaponData(m_currentWeapon).m_weaponStats.m_totalAmmo);
        //    if (_weaponAnim != null)
        //    {
        //        StopCoroutine(_weaponAnim);
        //    }
        //    StartCoroutine(EndUpgradeWeaponAnim(interactor));
        //    return true;
        //}
        if (!base.Interact(interactor))
        {
            return false;
        }
        interactor.ChangeSlotWeapon(GameManager.GetInstance().m_weaponsInGame.GetWeaponData(interactor.GetCurrentWeapon().GetWeaponID()).m_weaponUpgrade.m_weaponID);
        SetActive(true);
        
        //_weaponAnim = StartCoroutine(UpgradeWeaponAnim(interactor));



        return true;
    }

    public override bool ShowInteract(PlayerController interactor, bool look)
    {
        if (!_canInteract || GameManager.GetInstance().m_weaponsInGame.GetWeaponData(interactor.GetCurrentWeapon().GetWeaponID()).m_weaponUpgrade == null)
        {
            return false;

        }
        //if (m_currentWeapon != -1)
        //{
        //    string text = "Pick weapon";
        //    interactor.m_UIController.m_interactTextController.SetText(text);
        //    return true;
        //}
        base.ShowInteract(interactor, look);

        return true;
    }

    public override bool HideInteract(PlayerController interactor, bool look)
    {
        base.HideInteract(interactor, look);

        return true;
    }

    //IEnumerator UpgradeWeaponAnim(PlayerController interactor)
    //{
    //    m_animator.SetTrigger("Open");
    //    _canInteract = false;

    //    SetActive(true);

    //    foreach (var weapon in m_weapons)
    //    {
    //        if (!GameManager.GetInstance().m_playerController.HaveWeaponOnInventory(weapon.Key))
    //        {
    //            availableWeapons.Add(weapon.Key, weapon.Value);
    //        }
    //    }
    //    List<int> weaponsIDS = availableWeapons.Keys.ToList();

    //    float time = 2.0f / weaponsIDS.Count;


    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < weaponsIDS.Count; j++)
    //        {
    //            if (j != 0)
    //            {
    //                availableWeapons[weaponsIDS[j - 1]].SetActive(false);
    //            }
    //            availableWeapons[weaponsIDS[j]].SetActive(true);
    //            yield return new WaitForSeconds(time);
    //        }
    //        availableWeapons[weaponsIDS[weaponsIDS.Count - 1]].SetActive(false);
    //    }
    //    SetActive(false);
    //    SetActive(true);

    //    m_currentWeapon = weaponsIDS[Random.Range(0, weaponsIDS.Count)];
    //    availableWeapons[m_currentWeapon].SetActive(true);
    //    _canInteract = true;


    //    yield return new WaitForSeconds(10);

    //    StartCoroutine(EndUpgradeWeaponAnim(interactor));
    //}

    //public IEnumerator EndUpgradeWeaponAnim(PlayerController interactor)
    //{
    //    m_animator.SetTrigger("Close");

    //    HideInteract(interactor, false);
    //    SetActive(true);
    //    m_weapons[m_currentWeapon].SetActive(false);
    //    m_currentWeapon = -1;
    //    _canInteract = false;
    //    yield return new WaitForSeconds(2);
    //    _canInteract = true;

    //    SetActive(true);
    //}
}
