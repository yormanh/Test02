using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviourPun
{
    public enum eRaiseEventsCode
    {
        TakeBomb = 0
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData eventData)
    {
        if (eventData.Code == (byte)eRaiseEventsCode.TakeBomb)
        {
            object[] data = (object[])eventData.CustomData;
            string nickNameBomb = (string)data[0];
            int phothonViewId = (int)data[1];
            int targetViewId = (int)data[2];

            GameObject[] go = GameObject.FindGameObjectsWithTag("Player");


            Debug.Log("phothonViewId: " + phothonViewId);
            Debug.Log("nickNameBomb: " + nickNameBomb);
            Debug.Log("targetViewId: " + targetViewId);

            foreach (GameObject lgo in go)
            {
                Debug.Log("lgo.GetComponent<PhotonView>().ViewID: " + lgo.GetComponent<PhotonView>().ViewID);

                if (lgo.GetComponent<PhotonView>().ViewID == targetViewId)
                {
                    Debug.Log(nickNameBomb + " tiene la bomba ");
                    lgo.GetComponent<PlayerSetup>().IsBomb = true;

                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!photonView.IsMine)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            PhotonView target = other.gameObject.GetComponent<PhotonView>();
            string nickNameBomb = target.Owner.NickName;

            object[] data = new object[] { nickNameBomb, photonView.ViewID, target.ViewID };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = false
            };

            PhotonNetwork.RaiseEvent((byte)eRaiseEventsCode.TakeBomb, data, raiseEventOptions, sendOptions);

            photonView.RPC("Destroy", RpcTarget.AllBuffered);
        }

    }


    [PunRPC]
    public void Destroy()
    {
        Destroy(this.gameObject, 0.5f);
    }


}
