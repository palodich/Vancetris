using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    /*
    void Update()
    {
        TextMesh debugText;
        MinoBlock activeMinoBlock;

        debugText = gameObject.GetComponent<TextMesh>();
        string text;

        if (GameManger.Instance.ActiveMino == null)
        {
            text = "minoTimer: " + GameManger.Instance.minoTimer + "\n";
            text += "minoTimerDelay: " + GameManger.Instance.minoSpawnDelay + "\n";
        }
        else
        {
            activeMinoBlock = GameManger.Instance.ActiveMino.GetComponent<MinoBlock>();

            text = "ActiveMino: " + GameManger.Instance.ActiveMino.name + "\n";
            text += "activeMinoType: " + activeMinoBlock.activeMinoType + "\n";
            text += "activeMinoOrientation: " + activeMinoBlock.activeMinoOrientation + "\n\n";

            text += "gravityTimer: " + GameManger.Instance.gravityTimer + "\n";
            text += "currentGravityDelay: " + GameManger.Instance.currentGravityDelay + "\n\n";

            text += "buttonTimer: " + GameManger.Instance.buttonTimer + "\n";
            text += "buttonHoldDelay: " + GameManger.Instance.buttonHoldDelay + "\n";
            text += "movedOnce: " + GameManger.Instance.movedOnce + "\n\n";

            text += "moveRepeatTimer: " + GameManger.Instance.moveRepeatTimer + "\n";
            text += "moveRepeatDelay: " + GameManger.Instance.moveRepeatDelay + "\n\n";

            text += "lockTimer: " + GameManger.Instance.lockTimer + "\n";
            text += "lockTimerDelay: " + GameManger.Instance.lockTimerDelay + "\n\n";
        }

        debugText.text = text;
    }
    */
}
