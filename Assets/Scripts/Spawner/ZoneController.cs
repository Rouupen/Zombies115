using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class ZoneController : MonoBehaviour
{

    [SerializeField]
    private BoxCollider[] _zoneColliders;
    private HashSet<PlayerController> _playersInZone;
    private List<AreaController> _areas;
    private bool _zoneActive;

    private void Awake()
    {
        _playersInZone = new HashSet<PlayerController>();
        _areas = new List<AreaController>();

        //Aseguramos que sean triggers
        foreach (var zone in _zoneColliders)
        {
            zone.isTrigger = true;
        }
    }

    private void Start()
    {
        GameManager.GetInstance().m_zoneManager.AddZone(this);
    }

    void FixedUpdate()
    {
        UpdatePlayersInZone();

        _zoneActive = EvaluateIfZoneIsActive();
    }

    private void UpdatePlayersInZone()
    {
        // Returns all players in the zone at this moment
        HashSet<PlayerController> playersNowInZone = SearchForPlayersInsideZone();

        foreach (var player in playersNowInZone)
        {
            _playersInZone.Add(player); // HashSet ignores duplicates
        }

        //Evaluate eliminating those that are not in zone at this moment
        EvaluateToRemovePlayers(playersNowInZone);
    }

    private HashSet<PlayerController> SearchForPlayersInsideZone()
    {
        HashSet<PlayerController> players = new HashSet<PlayerController>();

        foreach (BoxCollider zone in _zoneColliders)
        {
            Vector3 center = zone.transform.TransformPoint(zone.center);
            Vector3 halfExtents = zone.size * 0.5f;
            Quaternion orientation = zone.transform.rotation;
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

        return players;
    }

    private void EvaluateToRemovePlayers(HashSet<PlayerController> playersNowInZone)
    {
        HashSet<PlayerController> playersToRemove = new HashSet<PlayerController>();
        foreach (PlayerController player in _playersInZone)
        {
            if (!playersNowInZone.Contains(player))
            {
                if (GameManager.GetInstance().m_zoneManager.CheckIfAnyZoneContainsPlayer(this, player))
                {
                    playersToRemove.Add(player);
                }
            }
        }

        foreach (PlayerController player in playersToRemove)
        {
            _playersInZone.Remove(player);
        }
    }

    public bool ZoneContainsPlayer(PlayerController player)
    {
        return _playersInZone.Contains(player);
    }

    private bool EvaluateIfZoneIsActive()
    {
        return _playersInZone.Count > 0;
    }

    public bool IsZoneActive()
    {
        return _zoneActive;
    }

    public void AddArea(AreaController area)
    {
        if (!_areas.Contains(area))
        {
            _areas.Add(area);
        }
    }

    public void RemoveArea(AreaController area)
    {
        if (_areas.Contains(area))
        {
            _areas.Remove(area);
        }
    }

    public List<AreaController> GetAreas()
    {
        return _areas;
    }
}
