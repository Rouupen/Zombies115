using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    [SerializeField]
    ZoneController _zoneController;
    [SerializeField]
    private BoxCollider[] _areaColliders;
    private HashSet<PlayerController> _playersInArea;
    private bool _areaActive;

    [ReadOnly(true)]
    [SerializeField]
    private bool _areaUnlocked;

    [SerializeField]
    private SpawnPointData[] _spawnPointsInArea;
    [SerializeField]
    private InteractableCostPointsBase[] _interactablesInArea;

    private void Start()
    {
        _playersInArea = new HashSet<PlayerController>();
        //Aseguramos que sean triggers
        foreach (var area in _areaColliders)
        {
            area.isTrigger = true;
        }

        if (_zoneController)
        {
            _zoneController.AddArea(this);
        }

        foreach (InteractableCostPointsBase interactable in _interactablesInArea)
        {
            interactable.AddArea(this);
        }

        if (_areaUnlocked)
        {

        }

    }

    private void FixedUpdate()
    {
        if (_zoneController == null || !_zoneController.IsZoneActive())
        {
            _areaActive = false;
            return;
        }

        UpdatePlayersInAreaOrNear();
        bool active = EvaluateIfAreaIsActive();

        if (_areaActive != active && _areaUnlocked)
        {
            if (active)
            {
                foreach (SpawnPointData spawn in _spawnPointsInArea)
                {
                    GameManager.GetInstance().m_spawnManager.AddSpawnPoint(spawn); 
                }
            }
            else
            {
                foreach (SpawnPointData spawn in _spawnPointsInArea)
                {
                    GameManager.GetInstance().m_spawnManager.RemoveSpawnPoint(spawn);
                }
            }
            _areaActive = active;
        }

    }

    private void UpdatePlayersInAreaOrNear()
    {
        // Returns all players in the zone at this moment
        HashSet<PlayerController> playersNowInArea = SearchForPlayersInsideOrNearArea();

        foreach (var player in playersNowInArea)
        {
            _playersInArea.Add(player); // HashSet ignores duplicates
        }

        //Evaluate eliminating those that are not in area at this moment
        EvaluateToRemovePlayers(playersNowInArea);
    }

    private HashSet<PlayerController> SearchForPlayersInsideOrNearArea()
    {
        HashSet<PlayerController> players = new HashSet<PlayerController>();

        foreach (BoxCollider area in _areaColliders)
        {
            Vector3 center = area.transform.TransformPoint(area.center);
            Vector3 halfExtents = area.size * 0.5f;
            Quaternion orientation = area.transform.rotation;
            int layerMask = 1 << LayerMask.NameToLayer(Const.LAYER_PLAYER);

            Collider[] overlaps = Physics.OverlapBox(center, halfExtents, orientation, layerMask, QueryTriggerInteraction.Ignore);

            foreach (Collider collision in overlaps)
            {
                if (collision.TryGetComponent<PlayerController>(out PlayerController player))
                {
                    players.Add(player);
                }
            }
        }

        //TEMP - will change if multiplayer is added
        foreach (BoxCollider area in _areaColliders)
        {
            Vector3 playerPosition = GameManager.GetInstance().m_playerController.transform.position;
            Vector3 closestPoint = area.ClosestPoint(playerPosition);

            if ((closestPoint - playerPosition).magnitude < Const.MIN_PLAYER_DIST_TO_AREA)
            {
                players.Add(GameManager.GetInstance().m_playerController);
            }
        }

        return players;
    }

    private void EvaluateToRemovePlayers(HashSet<PlayerController> playersNowInZone)
    {
        HashSet<PlayerController> playersToRemove = new HashSet<PlayerController>();
        foreach (PlayerController player in _playersInArea)
        {
            if (!playersNowInZone.Contains(player))
            {
                if (_zoneController != null)
                {
                    if (GameManager.GetInstance().m_zoneManager.CheckIfAnyZoneHaveAnAreaActive(this, player))
                    {
                        playersToRemove.Add(player);
                    }
                }
                else
                {
                    playersToRemove.Add(player);
                }
            }
        }

        foreach (PlayerController player in playersToRemove)
        {
            _playersInArea.Remove(player);
        }
    }

    public bool AreaContainsPlayer(PlayerController player)
    {
        return _playersInArea.Contains(player);
    }

    private bool EvaluateIfAreaIsActive()
    {
        return _playersInArea.Count > 0;
    }

    public bool IsAreaActive()
    {
        return _areaActive;
    }

    public void UnlockArea()
    {
        foreach (InteractableCostPointsBase interactable in _interactablesInArea)
        {
            interactable.Unlock();
        }
        _areaUnlocked = true;
    }
}
