using UnityEngine;

public enum SpawnType
{
    Instant,
    Ground
}

public class SpawnPointData : MonoBehaviour
{
    public SpawnType m_spawnType;
}
