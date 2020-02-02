using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownUI : MonoBehaviour
{
    float _duration;
    float _time;
    RectTransform _rect;

    // Start is called before the first frame update
    void Start()
    {
        _rect = GetComponent<RectTransform>();
        PlayerController.Instance.OnMeleeAttack += Attack;
    }

    // Update is called once per frame
    void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
            _rect.localScale = new Vector3(Mathf.Max(0, _time / _duration), 1, 1);
        }
    }

    void Attack()
    {
        _duration = PlayerController.Instance.MeleeAttackCooldown;
        _time = _duration;
    }

    private void OnDestroy()
    {
        PlayerController.Instance.OnMeleeAttack -= Attack;
    }
}
