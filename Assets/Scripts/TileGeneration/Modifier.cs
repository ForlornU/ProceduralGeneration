using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "ScriptableObjects/Generational Modifier", order = 0)]

public class Modifier : ScriptableObject
{
    public string modulename = "RandomWalk";
    public int tileCount = 250;
    public bool instant;
    [Range(0, 5)] public float creationspeed = 0.03f;

    //Or maybe we hold all the information of all passes here, like a list
}
