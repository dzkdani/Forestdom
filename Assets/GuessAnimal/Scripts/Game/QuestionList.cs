using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalListName", menuName = "Scriptable Objects/Question List", order = 1)]
public class QuestionList : ScriptableObject
{
    public List<Questions> questionList;
}
