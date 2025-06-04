using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

/// <summary>
/// Controls everything related to the player and the equipped weapons
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Character")]
    public CharacterController m_characterController;
    public CharacterMovement m_characterMovement;
    public CharacterLook m_characterLook;
    public EntityHealth m_characterHealth;
    public CharacterInteraction m_characterInteraction;
    public CharacterPerks m_characterPerks;
    public UIController m_UIController;

    [Header("Weapons")]
    public WeaponSocketMovementController m_weaponSocketMovementController;
    public Volume m_volumeDeath;

    //Character variables
    private StateMachineFilter _characterStateMachine;

    //Weapon variables
    private WeaponBase _activeWeapon;
    private List<WeaponBase> _weaponList;
    private List<int> _weaponsInventoryId;
    private List<int> _weaponsInSlots;
    private int _currentWeaponSlot;


    /// <summary>
    /// Initialize character and weapons
    /// </summary>
    public void Initizalize()
    {
        InitializeStateMachine();
        InstantiateAllWeapons();


        //TEMP - Need a weapon manager
        GameManager.GetInstance().m_inputManager.m_weaponSelection1.started += Weapon1;
        GameManager.GetInstance().m_inputManager.m_weaponSelection2.started += Weapon2;


        //TEMP - set active weapon id 0
        SetCurrentWeapon(0);

        //TEMP
        m_characterHealth.m_onDeath += Die;

        //TEMP
        _weaponsInSlots = new List<int>();
        _weaponsInSlots.Add(0);
        //Empty temp
        _weaponsInSlots.Add(-1);
    }

    private void OnDestroy()
    {
        GameManager.GetInstance().m_inputManager.m_weaponSelection1.started -= Weapon1;
        GameManager.GetInstance().m_inputManager.m_weaponSelection2.started -= Weapon2;
    }

    /// <summary>
    /// Creates the player's state machine and sets the initial state to idle
    /// </summary>
    private void InitializeStateMachine()
    {
        _characterStateMachine = new StateMachineFilter();
        _characterStateMachine.InitializeStateMachine<IdleOrMoving>(GameManager.GetInstance().m_characterStatesData.GetStatesData());
    }


    /// <summary>
    /// Instantiates all weapons available in the game at startup to avoid creating and destroying them during gameplay,
    /// which could cause performance issues or stuttering. Also ensures each weapon is properly initialized and deactivated
    /// </summary>
    private void InstantiateAllWeapons()
    {
        SO_WeaponsInGame weaponsInGame = GameManager.GetInstance().m_weaponsInGame;
        _weaponList = new List<WeaponBase>();
        _weaponsInventoryId = new List<int>();

        for (int i = 0; i < weaponsInGame.m_weaponsInGame.Count; i++)
        {
            GameObject weaponObject = Instantiate(weaponsInGame.m_weaponsInGame[i].m_weaponPrefab, m_weaponSocketMovementController.transform, false);

            if (weaponObject.TryGetComponent<WeaponBase>(out WeaponBase weaponComponent))
            {
                _weaponList.Add(weaponComponent);
            }
            else
            {
                Debug.LogWarning(weaponObject.name + " dosen´t have a weapon component attached");
                _weaponList.Add(weaponObject.AddComponent<WeaponBase>());
            }

            // Initialize weapon values
            _weaponList[i].InitializeWeapon(weaponsInGame.m_weaponsInGame[i].m_weaponID, weaponsInGame.m_weaponsInGame[i].m_weaponStats, weaponsInGame.m_weaponsInGame[i].m_weaponData, this);
            weaponObject.SetActive(false);

            // Add weapon ID to the inventory
            _weaponsInventoryId.Add(weaponsInGame.m_weaponsInGame[i].m_weaponID);
        }
    }

    /// <summary>
    /// Returns the current weapon
    /// </summary>
    public WeaponBase GetCurrentWeapon()
    {
        return _activeWeapon;
    }

    /// <summary>
    /// Sets the currently equipped weapon by its ID.
    /// Returns true if the weapon was found and set, false otherwise
    /// </summary>
    public bool SetCurrentWeapon(int id)
    {
        if (id < 0)
        {
            Debug.LogWarning($"SetCurrentWeapon called with invalid ID: {id}. ID must be >= 0");
            return false;
        }
        for (int i = 0; i < _weaponList.Count; i++)
        {
            if (_weaponList[i].GetWeaponID() == id)
            {
                _weaponList[i].SetActiveWeapon();
                return true;
            }
        }

        Debug.LogWarning($"Weapon with ID {id} was not found in the weapon list.");
        return false;
    }

    /// <summary>
    /// Sets the specified weapon as the currently equipped weapon instance
    /// </summary>
    public void SetCurrentWeapon(WeaponBase weapon)
    {
        _activeWeapon = weapon;
    }

    public void ChangeSlotWeapon(int id)
    {
        for (int i = 0; i < _weaponsInventoryId.Count; i++)
        {
            if (id == _weaponsInventoryId[i])
            {
                //Checks empty slots
                for (int j = 0; j < _weaponsInSlots.Count; j++)
                {
                    if (_weaponsInSlots[j] == -1)
                    {
                        _weaponsInSlots[j] = id;
                        SetCurrentWeapon(id);
                        return;
                    }
                }

                //Change current weapon
                for (int j = 0; j < _weaponsInSlots.Count; j++)
                {
                    if (_weaponsInSlots[j] == GetCurrentWeapon().GetWeaponID())
                    {
                        _weaponsInSlots[j] = id;
                        SetCurrentWeapon(id);
                        return;
                    }
                }
            }
        }
    }

    //TEMP
    private void Weapon1(InputAction.CallbackContext context)
    {
        if (_weaponsInSlots[0] == -1)
        {
            return;
        }
        SetCurrentWeapon(_weaponsInSlots[0]);
    }

    //TEMP
    private void Weapon2(InputAction.CallbackContext context)
    {
        if (_weaponsInSlots[1] == -1)
        {
            return;
        }
        SetCurrentWeapon(_weaponsInSlots[1]);
    }

    public bool HaveWeaponOnInventory(int id)
    {
        for (int i = 0; i < _weaponsInSlots.Count; i++)
        {
            if (_weaponsInSlots[i] == id)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the character state machine
    /// </summary>
    public StateMachineFilter GetStateMachine()
    {
        return _characterStateMachine;
    }

    //TEMP
    public void SetIdleValues()
    {
        if (_activeWeapon != null)
        {
            _activeWeapon.SetIdleValues();
        }
    }

    //TEMP
    public void SetMovingValues()
    {
        if (_activeWeapon != null)
        {
            _activeWeapon.SetMovingValues();
        }
    }

    //TEMP
    public void Die()
    {
        GetStateMachine().SetCurrentState<Downed>();
        Debug.Log("DEAD");
    }
}
