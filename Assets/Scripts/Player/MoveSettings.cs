using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Movement/MoveSettings")]
public class MoveSettings : ScriptableObject
{
    public float movespeed = 5;

    public float jumpforce = 9;
    public int airJumps = 1;
    public float groundColliderSize = 0.36f;

    public float dashforce = 3f;
    public float dashtime = 0.5f;

}
