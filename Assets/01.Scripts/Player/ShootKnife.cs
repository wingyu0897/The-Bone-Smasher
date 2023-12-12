using Unity.Netcode;
using UnityEngine;

public class ShootKnife : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform shootPositionTrm;
    [SerializeField] private GameObject serverKnifePrefab;
    [SerializeField] private GameObject clientKnifePrefab;
    [SerializeField] private Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] private float knifeSpeed;
    [SerializeField] private int knifeDamage;
    [SerializeField] private float throwCooltime;

    private float lastThrowTime;

	public override void OnNetworkSpawn()
	{
		if (!IsOwner) return;
        inputReader.ShootEvent += HandleShootKnife;
	}

	public override void OnNetworkDespawn()
	{
		if (!IsOwner) return;
        inputReader.ShootEvent -= HandleShootKnife;
	}

	private void Update()
	{
		if (lastThrowTime > 0)
			lastThrowTime -= Time.deltaTime;
	}

	private void HandleShootKnife()
	{
		if (Time.time < lastThrowTime + throwCooltime) return;

		Vector3 pos = shootPositionTrm.position;
		Vector3 direction = shootPositionTrm.right;
		lastThrowTime = Time.time;
		SpawnDummyKnife(pos, direction);

		ShootKnifeServerRpc(pos, direction);
	}

	[ServerRpc]
	private void ShootKnifeServerRpc(Vector3 pos, Vector3 dir)
	{
		UserData user = ServerSingleton.Instance.NetServer.GetUserDataByClientID(OwnerClientId);

		GameObject instance = Instantiate(serverKnifePrefab, pos, Quaternion.identity);
		instance.transform.right = dir;

		Physics2D.IgnoreCollision(playerCollider, instance.GetComponent<Collider2D>());

		if (instance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigid))
		{
			rigid.velocity = dir * knifeSpeed;
		}

		if (instance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact damage))
		{
			damage.SetDamage(knifeDamage);
			damage.SetOwner(OwnerClientId);
		}

		ShootDummyKnifeClientRpc(pos, dir);
	}

	[ClientRpc]
	private void ShootDummyKnifeClientRpc(Vector3 pos, Vector3 dir)
	{
		if (IsOwner) return;
		SpawnDummyKnife(pos, dir);
	}

	private void SpawnDummyKnife(Vector3 pos, Vector3 dir)
	{
		GameObject instance = Instantiate(clientKnifePrefab, pos, Quaternion.identity); 
		instance.transform.right = dir;

		Physics2D.IgnoreCollision(playerCollider, instance.GetComponent<Collider2D>());

		if (instance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigid))
		{
			rigid.velocity = dir * knifeSpeed;
		}
	}
}
