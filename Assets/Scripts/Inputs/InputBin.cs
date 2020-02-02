using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputBin : MonoBehaviour
{
    Button _button;
    
    void Start()
    {
        _button = GetComponent<Button>();
    }

    void Update()
    {
        _button.interactable = (InputButton.SelectedButton != null);
    }

    public void Pressed()
    {
        InputUI.Instance.DestroyButton(InputButton.SelectedButton);
    }
}
