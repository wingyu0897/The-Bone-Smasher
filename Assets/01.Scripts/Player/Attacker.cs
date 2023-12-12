using System;
using Unity.Netcode;
using UnityEngine;

public class Attacker : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Collider2D playerCollider;

    [HideInInspector] public WeaponAnimation weaponAnimator;

    private float lastThrowTime;

    public void SetWeapon(Weapon weapon)
	{
        this.weapon = weapon;
        weaponAnimator = weapon.transform.Find("Visual").GetComponent<WeaponAnimation>();
	}

	public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.ShootEvent += HandleUserSword;
    }

	public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.ShootEvent -= HandleUserSword;
    }

    private void HandleUserSword()
	{
        if (Time.time < lastThrowTime + weapon.cooltime) return;

        lastThrowTime = Time.time;
        Physics2D.IgnoreCollision(playerCollider, weapon.GetComponent<Collider2D>());
        PlayerAnimation();
        AttackMeleeServerRpc();
    }

    [ServerRpc]
	private void AttackMeleeServerRpc()
	{
        weapon.SetOwner(OwnerClientId);
        weapon.Attack();

        PlayerAnimationClientRpc();
    }

    [ClientRpc]
    private void PlayerAnimationClientRpc()
    {
        if (IsOwner) return;
        PlayerAnimation();
    }

    private void PlayerAnimation()
	{
        if (weaponAnimator == null || weapon == null)
		{
            GetComponent<WeaponSelector>().SetVisualOnServerRpc();
		    //weaponAnimator = weapon.transform.Find("Visual").GetComponent<WeaponAnimation>();
		}
        weaponAnimator.SetAttack();
    }
}
