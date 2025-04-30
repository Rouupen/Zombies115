using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController m_characterController;
    public CharacterMovement m_characterMovement;
    public CharacterLook m_characterLook;

    public Weapon m_weapon;
    public WeaponSocketMovementController m_weaponSocketMovementController;
    private List<Weapon> _weaponList;
    private List<int> _weaponsInventoryId;

    private StateMachineFilter _stateMachine;
    public void Initizalize()
    {
        InitializeStateMachine();
        InstantiateAllWeapons();

        //Temp
        _weaponsInventoryId = new List<int>();
        _weaponsInventoryId.Add(0);
        _weaponsInventoryId.Add(1);
        GameManager.GetInstance().m_inputManager.m_weaponSelection1.started += Weapon1;
        GameManager.GetInstance().m_inputManager.m_weaponSelection2.started += Weapon2;


        //TEMP - set active weapon id 0
        SetCurrentWeapon(0);
    }

    void InitializeStateMachine()
    {
        _stateMachine = new StateMachineFilter();
        _stateMachine.InitializeStateMachine<Idle>(GameManager.GetInstance().m_characterStatesData.GetStatesData());
    }

    void InstantiateAllWeapons()
    {
        SO_WeaponsInGame weaponsInGame = GameManager.GetInstance().m_weaponsInGame;
        _weaponList = new List<Weapon>();
        for (int i = 0; i < weaponsInGame.m_weaponsInGame.Count; i++)
        {
            GameObject go = Instantiate(weaponsInGame.m_weaponsInGame[i].m_weaponPrefab, m_weaponSocketMovementController.transform, false);

            go.SetActive(false);
            if (go.TryGetComponent<Weapon>(out Weapon weapon))
            {
                _weaponList.Add(weapon);
            }
            else
            {
                _weaponList.Add(go.AddComponent<Weapon>());
            }

            _weaponList[i].InitializeWeapon(weaponsInGame.m_weaponsInGame[i].m_weaponID, weaponsInGame.m_weaponsInGame[i].m_weaponStats, weaponsInGame.m_weaponsInGame[i].m_weaponData);
        }
    }


    public bool SetCurrentWeapon(int id)
    {
        if (id == -1)
        {
            return false;
        }
        for (int i = 0; i < _weaponList.Count; i++)
        {
            if (_weaponList[i].GetWaponID() == id)
            {
                _weaponList[i].SetActiveWeapon();
                return true;
            }
        }

        return false;
    }

    private void Weapon1(InputAction.CallbackContext context)
    {
        SetCurrentWeapon(_weaponsInventoryId[0]);
    }

    private void Weapon2(InputAction.CallbackContext context)
    {
        SetCurrentWeapon(_weaponsInventoryId[1]);
    }

    public StateMachineFilter GetStateMachine()
    {
        return _stateMachine;
    }
    public void SetIdleValues()
    {
        if (m_weapon != null)
        {
            m_weapon.SetIdleValues();
        }

    }

    public void SetMovingValues()
    {
        if (m_weapon != null)
        {
            m_weapon.SetMovingValues();
        }
    }

}
