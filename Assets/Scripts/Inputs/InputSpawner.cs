using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

interface IDestroyable
{
    event System.Action OnDestroyEvent;
}

public class InputSpawner : MonoBehaviour
{
    public GameObject Prefab;
    public float RespawnTime;
    [Range(1, 2)]
    public float RandomFactor = 1.05f;
    // Start is called before the first frame update
    void Start()
    {
        if (Prefab.GetComponent<IDestroyable>() == null)
            Debug.LogError("No Destroyable Component found on prefab !");

        Spawn();
    }

    void Spawn()
    {
        Instantiate(Prefab, transform).GetComponent<IDestroyable>().OnDestroyEvent += ()=> DOVirtual.DelayedCall(RespawnTime * Random.Range(1, RandomFactor), Spawn); 
    }

}
