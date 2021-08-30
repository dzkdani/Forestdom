using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionButton : MonoBehaviour
{
    [Header("Option Button Status")]
    [SerializeField] bool haveSound;
    [SerializeField] int optionId;
    [SerializeField] Sprite optionSprite;
    [SerializeField] string optionText;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(CheckOption);
    }

    void CheckOption()
    {
        GameManager.instance.CheckAnswer(optionId);
    }

    public void SetOptionBtn(int id, Sprite sprite, string text, bool sounded)
    {
        haveSound = sounded;
        optionId = id;
        optionSprite = sprite;
        optionText = text;
    }

    private void OnDestroy()
    {
        
    }
}
