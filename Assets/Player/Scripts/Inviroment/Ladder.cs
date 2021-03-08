using System.Linq;
using UnityEngine;
using System.Collections;

public interface Interactable
{
    void Interact();
}
public class Ladder : MonoBehaviour, Interactable
{
    public float playerLerpSpeed = 10f;
    public float playerMoveSpeed = 10f;
    public float minDistanceBetweenPoints = 3f; //Минимальная дистанция, между игроком и одной из LadderPos, при котором игрок при слезании примет эту позицию 
    public Transform[] LadderPoses; //LaderPoses[0] - обязательно стартовая позиция

    Vector3 minPos;
    Vector3 maxPos;

    bool isInteracted = false;

    void Start()
    {
        maxPos.x = minPos.x = LadderPoses[0].position.x;
        maxPos.z = minPos.z = LadderPoses[0].position.z;

        maxPos.y = LadderPoses.Max(x => x.position.y) - minDistanceBetweenPoints * 0.5f;
    }
    public void Interact()
    {
        if(!isInteracted)
        StartCoroutine(LadderMode());
    }
    public bool OnMaxPointReached()
    {
        var playerCenter = Player.instance.transform.up * Player.instance.CrouchHeight;
        var dista = Vector3.Distance(Player.instance.transform.position + playerCenter, LadderPoses[LadderPoses.Length - 1].position);
        if (dista <= minDistanceBetweenPoints
            &&
            Vector3.Dot(Player.instance.controller.velocity, Vector3.up) > 0f)
        {
            return true;
        }
        return false;
    }
    public bool OnMinPointReached()
    {
        var playerCenter = Player.instance.transform.up * Player.instance.CrouchHeight;
        var dista = Vector3.Distance(Player.instance.transform.position + playerCenter, LadderPoses[0].position);
        if (dista <= minDistanceBetweenPoints
            &&
            Vector3.Dot(Player.instance.controller.velocity, Vector3.down) > 0f)
        {
            return true;
        }
        return false;
    }
    IEnumerator LadderMode()
    {
        var player = Player.instance;
        var playerCam = player.alignCamera;
        var playerTrans = Player.instance.transform;

        player.enabled = false;
        isInteracted = true;
        playerCam.XLockRotate = true;
        playerCam.YLockRotate = true;

        Transform nearestLadderPos = FindNearestPos(playerTrans);
        var LadderPos = new Vector3(maxPos.x, nearestLadderPos.position.y, maxPos.z);

        playerCam.StartLookAtX(LadderPoses[0].rotation);
        yield return StartCoroutine(Player.instance.TranslateAtPosition(LadderPos, playerLerpSpeed));
        playerCam.YLockRotate = false;

        var endPos = nearestLadderPos.position.y - Player.instance.controllerHeight * 0.6f;
    ladderController:
        while (!Input.GetKey(KeyCode.F))
        {
            var moveVector = nearestLadderPos.up * Input.GetAxis("Vertical") * playerMoveSpeed * Time.deltaTime;
            Player.instance.controller.Move(moveVector);
            playerTrans.position =
                new Vector3(
                    playerTrans.position.x,
                    Mathf.Clamp(playerTrans.position.y, minPos.y, maxPos.y),
                    playerTrans.position.z);
           
            yield return new WaitForEndOfFrame();
            
            if (OnMaxPointReached() || OnMinPointReached())
            {
                break;
            }
        }
        var endLadderPos = FindNearestPos(playerTrans);
        var playerCenter = playerTrans.up * Player.instance.CrouchHeight;
        var dist = Vector3.Distance(playerTrans.position + playerCenter, endLadderPos.position);
        if (Vector3.Distance(playerTrans.position + playerCenter, endLadderPos.position) >= minDistanceBetweenPoints)
        {
            yield return new WaitForEndOfFrame();
            goto ladderController;
        }
        yield return StartCoroutine(Player.instance.TranslateAtPosition(endLadderPos, playerLerpSpeed));
        playerCam.StopLookAt();
        Player.instance.enabled = true;
        Player.instance.alignCamera.XLockRotate = false;
        isInteracted = false;
    }

    Transform FindNearestPos(Transform player)
    {
        Transform result = null;
        float minDist = float.MaxValue;
        foreach(var pos in LadderPoses)
        {
            var dist = Vector3.Distance(player.position, pos.position);
            if(dist < minDist)
            {
                minDist = dist;
                result = pos;
            }
        }
        return result;
    }
}
