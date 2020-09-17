using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject[] _hands;
    [SerializeField] GameObject[] _soldier;


    [SerializeField] Camera _cameraPlayer;

    [SerializeField] Animator _animator;

    [SerializeField] GameObject _playerUIPrefab;
    [SerializeField] TextMeshProUGUI _playerNameText;

    [SerializeField] bool _isBomb;
    public bool IsBomb { get { return _isBomb; } set { _isBomb = value; } }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            foreach (GameObject go in _hands)
                go.SetActive(true);
            foreach (GameObject go in _soldier)
                go.SetActive(false);

            _cameraPlayer.enabled = true;
            GetComponent<RigidbodyFirstPersonController>().enabled = true;
            GetComponent<PlayerMovementController>().enabled = true;

            _animator.SetBool("IsSoldier", false);

            //Instantiate PlayerUI
            GameObject playerUIGameobject = Instantiate(_playerUIPrefab);

        }
        else
        {
            foreach (GameObject go in _hands)
                go.SetActive(false);
            foreach (GameObject go in _soldier)
                go.SetActive(true);

            _cameraPlayer.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            GetComponent<PlayerMovementController>().enabled = false;

            _animator.SetBool("IsSoldier", true);

        }


        _playerNameText.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
