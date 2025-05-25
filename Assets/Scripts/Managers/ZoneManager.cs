using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : Manager
{
    private List<ZoneController> _zones;

    public override void Initialize()
    {
        _zones = new List<ZoneController>();
    }

    public override void Deinitialize()
    {
        _zones.Clear();
    }

    public void AddZone(ZoneController zone)
    {
        if (!_zones.Contains(zone))
        {
            _zones.Add(zone);
        }
    }

    public void RemoveZone(ZoneController zone)
    {
        if (_zones.Contains(zone))
        {
            _zones.Remove(zone);
        }
    }

    public bool CheckIfAnyZoneContainsPlayer(ZoneController selfZone, PlayerController player)
    {
        for (int i = 0; i < _zones.Count; i++)
        {
            if (_zones[i] != selfZone && _zones[i].ZoneContainsPlayer(player))
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckIfAnyZoneHaveAnAreaActive(AreaController selfArea, PlayerController player)
    {
        for (int i = 0; i < _zones.Count; i++)
        {
            if (_zones[i].IsZoneActive())
            {
                List<AreaController> zoneArea = _zones[i].GetAreas();
                for (int j = 0; j < zoneArea.Count; j++)
                {
                    if (zoneArea[j] != selfArea && zoneArea[j].IsAreaActive())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override void UpdateManager()
    {
        throw new System.NotImplementedException();
    }
}
