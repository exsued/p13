using UnityEngine;

[System.Serializable]
public enum NPCState
{
    Idle,
    Patrol,
    Attack
}
public class NPC : MonoBehaviour
{
    public NPCState state;
}
