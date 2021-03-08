using UnityEngine;
using System.Collections.Generic;

public class ControllerNPC : MonoBehaviour
{
    public static bool spotted => instance != null ? instance.npcOnScene.Count > 0 : false;
    public static SortedSet<NPC> NPConScene => instance.npcOnScene;
    static ControllerNPC instance = null;

    SortedSet<NPC> npcOnScene = null;

    void Awake()
    {
        instance = this;
        npcOnScene = new SortedSet<NPC>();
    }
    public static void OnAgression(NPC npc)
    {
        instance.npcOnScene.Add(npc);
    }
    public static void OnRelax(NPC npc)
    {
        instance.npcOnScene.Remove(npc);
    }
}
