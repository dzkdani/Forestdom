using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonModeSelector : MonoBehaviour
{
    [SerializeField] string mode;
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(SelectMode);
    }

    void SelectMode()
    {
        GameModeManager.instance.SelectGameMode(mode);
    }

    private void OnDestroy()
    {
        
    }

}
