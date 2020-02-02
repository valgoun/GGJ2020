using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnManager : MonoBehaviour
{
    public GameObject Prefab;
    public Transform Player;
    public int MaxEntity = 0;
    public float MinRange = 10f;
    public float InitialDelay = 0f;
    public float StartDelay = 4f;
    public float EndDelay = 0.2f;

    private int m_spawnIndex = 1;
    private int m_entityCount = 0;
    private bool m_spawnActive = true;
    private List<Vector3> m_spawingPoints = new List<Vector3>();

    // Start is called before the first frame update
    private void Start()
    {
        foreach(Transform child in transform)
        {
            m_spawingPoints.Add(child.position);
        }

        DOVirtual.DelayedCall(ComputeDelay() + InitialDelay, SpawnCycle);
    }

    void SpawnCycle()
    {
        int spawnIndex = Random.Range(0, m_spawingPoints.Count);
        if(MinRange > 0)
            CheckSpawnPoint(ref spawnIndex);

        BaseAI Ai  = GameObject.Instantiate(Prefab, m_spawingPoints[spawnIndex], Quaternion.identity).GetComponent<BaseAI>();

        Ai.GetComponent<IDestroyable>().OnDestroyEvent += OnObjectDestroy;

        if(Ai != null)
            Ai.Target = Player;

        m_spawnIndex++;
        m_entityCount++;

        if (m_entityCount < MaxEntity)
            DOVirtual.DelayedCall(ComputeDelay(), SpawnCycle);
        else
            m_spawnActive = false;
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

    private void OnObjectDestroy()
    {
        m_entityCount -= 1;
        if(!m_spawnActive)
        {
            m_spawnActive = true;
            DOVirtual.DelayedCall(ComputeDelay(), SpawnCycle);
        }
    }
 
}
