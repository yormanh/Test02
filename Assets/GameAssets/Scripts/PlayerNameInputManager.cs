using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameInputManager : MonoBehaviour
{
    public void SetPayerName(string playerName)
    {

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Player name is empty");
            return;
        }


        PhotonNetwork.NickName = playerName;

    }
}
