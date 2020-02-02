using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public GameObject HealthPrefab;

    List<GameObject> healths = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < PlayerLifeManager.Instance.MaxLife; i++)
            healths.Add(Instantiate(HealthPrefab, transform));

        PlayerLifeManager.Instance.OnDamaged += RemoveHealth;
    }

    void RemoveHealth(Vector3 pos)
    {
        if(healths.Count > 0)
        {
            Destroy(healths[healths.Count - 1]);
            healths.RemoveAt(healths.Count - 1);
        }
    }

    private void OnDestroy()
    {
        PlayerLifeManager.Instance.OnDamaged -= RemoveHealth;
    }
}
