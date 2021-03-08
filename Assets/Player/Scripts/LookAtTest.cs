using UnityEngine;
using System.Collections;
public class LookAtTest : MonoBehaviour
{
    public Transform observer = null;
    public Transform observerChild = null;
    IEnumerator Start()
    {
        yield return new WaitUntil(() => Player.instance == null);
        //Player.instance.alignCamera.StartLookAt(transform);
        yield break;
    }
}
