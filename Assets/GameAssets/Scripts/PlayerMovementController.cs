using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    Animator _animator;
    RigidbodyFirstPersonController _rigidBodyController;
    Shooting _shootingScript;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBodyController = GetComponent<RigidbodyFirstPersonController>();
        _shootingScript = GetComponent<Shooting>();
    }


    private void Update()
    {
        Vector2 input = _rigidBodyController.GetInput();

        Debug.Log("Horizontal: " + input.x);
        Debug.Log("Vertical: " + input.y);


        _animator.SetFloat("Horizontal", input.x);
        _animator.SetFloat("Vertical", input.y);

        if (_rigidBodyController.Running)
        {
            _animator.SetBool("IsRunning", true);
            //Debug.Log("RUNNING");
        }
        else
            _animator.SetBool("IsRunning", false);


        if (_rigidBodyController.GetButtonDown("Fire1"))
        {
            _shootingScript.Fire();
        }


        if (_rigidBodyController.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");
        }

    }
}
