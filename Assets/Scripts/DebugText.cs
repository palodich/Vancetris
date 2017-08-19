using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    void Update()
    {
        TextMesh debugText;
        MinoBlock activeMinoBlock;

        debugText = gameObject.GetComponent<TextMesh>();
        string text;

        if (GameManger.instance.activeMino == null)
        {
            text = "minoTimer: " + GameManger.instance.minoTimer + "\n";
            text += "minoTimerDelay: " + GameManger.instance.minoSpawnDelay + "\n";
        }
        else
        {
            activeMinoBlock = GameManger.instance.activeMino.GetComponent<MinoBlock>();

            text  = "activeMino: " + GameManger.instance.activeMino.name + "\n";
            text += "activeMinoType: " + activeMinoBlock.activeMinoType + "\n";
            text += "activeMinoOrientation: " + activeMinoBlock.activeMinoOrientation + "\n\n";

            text += "gravityTimer: " + GameManger.instance.gravityTimer + "\n";
            text += "currentGravityDelay: " + GameManger.instance.currentGravityDelay + "\n\n";

            text += "buttonTimer: " + GameManger.instance.buttonTimer + "\n";
            text += "buttonHoldDelay: " + GameManger.instance.buttonHoldDelay + "\n";
            text += "movedOnce: " + GameManger.instance.movedOnce + "\n\n";

            text += "moveRepeatTimer: " + GameManger.instance.moveRepeatTimer + "\n";
            text += "moveRepeatDelay: " + GameManger.instance.moveRepeatDelay + "\n\n";

            text += "lockTimer: " + GameManger.instance.lockTimer + "\n";
            text += "lockTimerDelay: " + GameManger.instance.lockTimerDelay + "\n\n";
        }

        debugText.text = text;
    }
}
