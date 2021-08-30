using UnityEngine;

[CreateAssetMenu(fileName = "Animal Name", menuName = "Scriptable Objects/Animal", order = 1)]
public class Questions : ScriptableObject
{
    public bool haveSound;
    public int animalId;
    public string animalName;
    public Sprite animalSprite;
    public AudioClip animalSound;
}
