using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    public GameObject floatingInfo;
    public TextMesh nameText;

    private Material playerMaterialClone;

    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    private string playerName;

    [SyncVar(hook = nameof(OnPlayerColorChanged))]
    private Color playerColor;

    private void OnPlayerNameChanged(string oldStr, string newStr)
    {
        nameText.text = newStr;
    }

    private void OnPlayerColorChanged(Color oldCol, Color newCol)
    {
        nameText.color = newCol;

        playerMaterialClone = new Material(GetComponent<Renderer>().material);
        playerMaterialClone.SetColor("_EmissionColor", newCol);

        GetComponent<Renderer>().material = playerMaterialClone;
    }

    [Command]
    private void CmdSetupPlayer(string nameValue, Color colorValue)
    {
        playerName = nameValue;
        playerColor = colorValue;
    }


    public override void OnStartLocalPlayer()
    {
        //…„œÒª˙”Î Player ∞Û∂®
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = Vector3.zero;

        floatingInfo.transform.localPosition = new Vector3(0, -.3f, .6f);
        floatingInfo.transform.localScale = new Vector3(.1f, .1f, .1f);

        ChangePlayerNameAndColor();
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            floatingInfo.transform.LookAt(Camera.main.transform);
            return;
        }

        var moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110f;
        var moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;

        transform.Rotate(0, moveX, 0);
        transform.Translate(0, 0, moveZ);


        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangePlayerNameAndColor();
        }
    }

    private void ChangePlayerNameAndColor()
    {
        var tempName = $"Player {UnityEngine.Random.Range(1, 999)}";
        var tempColor = new Color
        (
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            1
        );

        CmdSetupPlayer(tempName, tempColor);
    }
}
