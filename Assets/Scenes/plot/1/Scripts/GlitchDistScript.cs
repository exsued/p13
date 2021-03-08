using UnityEngine;
using System.Collections;

public class GlitchDistScript : MonoBehaviour
{
    float curDistance;
    public float MinDistance = 1f, MaxDistance = 4f;

    public Color startColor;
    float grayMax;
    private IEnumerator Start()
    {
        yield return new WaitWhile(() => Player.instance == null);
        startColor = Player.instance.nightVision.color;
        while (true)
        {
            curDistance = Vector3.Distance(Player.instance.transform.position, transform.position) - MinDistance;
            var magnitude = MaxDistance - MinDistance;
            var distProcents = Mathf.Clamp(curDistance / magnitude, 0f, 1f);
            print(magnitude + " " + distProcents + " ::" + grayMax);
            Player.instance.nightVision.color = startColor * distProcents;
            yield return new WaitForEndOfFrame();
        }
    }
}
