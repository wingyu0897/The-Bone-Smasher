using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    private Animator _animator;

    private readonly int attackHash = Animator.StringToHash("attack");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetAttack()
    {
        _animator.SetTrigger(attackHash);
    }
}
