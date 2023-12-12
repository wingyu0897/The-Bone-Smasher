using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public int damage;
	public float cooltime;
	private ulong clientId;
	
 	private Collider2D col;

	public ContactFilter2D contactFiler;

	private void Start()
	{
		col = GetComponent<Collider2D>();
	}

	public void SetOwner(ulong ownerClientId)
	{
		clientId = ownerClientId;
	}

	public void Attack()
	{
		List<Collider2D> collisions = new List<Collider2D>();
		col.OverlapCollider(contactFiler, collisions);
		foreach (Collider2D collision in collisions)
		{
			if (collision.attachedRigidbody == null) continue;

			if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
			{
				health.TakeDamage(damage, clientId);
			}
		}
	}
}
