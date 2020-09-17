using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FPSGameManager : MonoBehaviour
{

    [SerializeField] GameObject _playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (_playerPrefab != null)
            {
                int randonPointZ = Random.Range(-10, 0);
                int randonPointX = Random.Range(-5, 0);
                PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(randonPointX, 0, randonPointZ), Quaternion.identity);
                //PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.Log("playerPrefab!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
