using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnManager : MonoBehaviour
{
    public GameObject MeleePrefab;
    public GameObject RangedPrefab;
    public Transform Player;
    public float MinRange = 10f;
    public float StartDelay = 4f;
    public float EndDelay = 0.2f;

    private int m_spawnIndex = 1;
    private List<Vector3> m_spawingPoints = new List<Vector3>();

    // Start is called before the first frame update
    private void Start()
    {
        foreach(Transform child in transform)
        {
            m_spawingPoints.Add(child.position);
        }

        DOVirtual.DelayedCall(ComputeDelay(), SpawnCycle);
    }

    void SpawnCycle()
    {
        int spawnIndex = Random.Range(0, m_spawingPoints.Count);
        CheckSpawnPoint(ref spawnIndex);

        if(m_spawnIndex > 10 && m_spawnIndex % 3 == 0) // Spawn Ranged
        {
            GameObject.Instantiate(RangedPrefab, m_spawingPoints[spawnIndex], Quaternion.identity).GetComponent<BaseAI>().Target = Player;
        }
        else // Spawn Melee
        {
            GameObject.Instantiate(MeleePrefab, m_spawingPoints[spawnIndex], Quaternion.identity).GetComponent<BaseAI>().Target = Player;
        }

        m_spawnIndex++;

        DOVirtual.DelayedCall(ComputeDelay(), SpawnCycle);
    }

    private void CheckSpawnPoint(ref int spawnIndex)
    {
        if ((m_spawingPoints[spawnIndex] - Player.position).sqrMagnitude < MinRange * MinRange)
            spawnIndex = (spawnIndex + 1) % m_spawingPoints.Count;
    }

    private float ComputeDelay()
    {
        return (StartDelay - EndDelay) / (float)m_spawnIndex + EndDelay;
    }
 
}
