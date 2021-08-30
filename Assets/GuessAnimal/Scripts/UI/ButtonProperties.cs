using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonProperties : MonoBehaviour
{
    [Header("Button Components")]
    [Tooltip("null if no panel to open")]
    [SerializeField] GameObject relatedPanel;
    [Tooltip("Scene: scene&exit, Panel: open panel, Exit: quit game, GPS: open gplay")]
    [SerializeField] string buttonFunction;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ButtonFunc);
    }

    void ButtonFunc()
    {
        if (buttonFunction != "Option")
        {
            AudioManager.instance.PlaySFX(0);
            if (buttonFunction == "Volume")
            {
                if (AudioManager.instance.audioPaused)
                {
                    GetComponentInChildren<TextMeshProUGUI>().text = "Sound On";
                }
                else
                {
                    GetComponentInChildren<TextMeshProUGUI>().text = "Sound Off";
                }
            }
            else
            {
                ButtonManager.instance.ButtonPressedTween(gameObject, buttonFunction, relatedPanel);
            }
        }
    }

    private void OnDestroy()
    {

    }
}
