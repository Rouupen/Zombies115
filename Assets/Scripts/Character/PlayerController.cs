using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController m_characterController;
    public CharacterMovement m_characterMovement;
    public CharacterLook m_characterLook;

    public Weapon m_weapon;
    public WeaponSocketMovementController m_weaponSocketMovementController;
    private List<Weapon> _weaponList;

    private StateMachineFilter _stateMachine;
    public void Initizalize()
    {
        InitializeStateMachine();
        InstantiateAllWeapons();


        //TEMP - set active weapon id 0
        SetCurrentWeapon(1);
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

    public StateMachineFilter GetStateMachine()
    {
        return _stateMachine;
    }
}
