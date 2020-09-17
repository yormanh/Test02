using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Shooting : MonoBehaviourPunCallbacks
{
    [SerializeField] Camera _camera;
    [SerializeField] GameObject _hitEffectFlarePrefab;

    [Header("Health")]
    [SerializeField] float _startHealth = 100;
    [SerializeField] Image _healthBar;
    float _health;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _health = _startHealth;
        _healthBar.fillAmount = _health / _startHealth;

        _animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffect", RpcTarget.AllBuffered, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
            }

        }
    }

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo info)
    {
        _health -= damage;
        Debug.Log("Health: " + _health);

        _healthBar.fillAmount = _health / _startHealth;
        if (_health <= 0f)
        {
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);

            StartCoroutine(SetScoreInternet(info.Sender.NickName));
        }
    }



    [PunRPC]
    public void CreateHitEffect(Vector3 position)
    {
        GameObject hitEffect = Instantiate(_hitEffectFlarePrefab, position, Quaternion.identity);
        Destroy(hitEffect, 0.5f);
    }


    void Die()
    {
        if (photonView.IsMine)
        {
            _animator.SetBool("IsDead", true);
            StartCoroutine(Respawn());
        }
    }
    IEnumerator Respawn()
    {
        GameObject reSpawnText = GameObject.Find("RespawnText");

        float respawnTime = 8.0f;
        while (respawnTime > 0.0f)
        {
            yield return new WaitForSecondsRealtime(1.0f);
            respawnTime -= 1.0f;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            reSpawnText.GetComponent<TextMeshProUGUI>().text = "Yor are Killed. Respawing at: " + respawnTime.ToString(".00");

        }

        _animator.SetBool("IsDead", false);
        reSpawnText.GetComponent<TextMeshProUGUI>().text = "";

        int randomPoint = Random.Range(-20, 20);
        transform.position = new Vector3(randomPoint, 0, randomPoint);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RagainHealth", RpcTarget.AllBuffered);
    }



    [PunRPC]
    public void RagainHealth()
    {
        _health = _startHealth;
        _healthBar.fillAmount = _health / _startHealth;

    }


    IEnumerator SetScoreInternet(string nickName)
    {
        string setScoreUrl = "http://yormanh.orgfree.com/test/addscore.php";


        //form.AddField("lsName", nickName);
        //form.AddField("liScore", 1);

        string lsUrl = setScoreUrl + "?lsName=" + nickName + "&liScore=" + 1;
        using (UnityWebRequest wwwWebRequest = UnityWebRequest.Get(lsUrl))
        {
            yield return wwwWebRequest.SendWebRequest();
        }

    }






}
