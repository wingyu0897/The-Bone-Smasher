using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform handTrm;

    private Camera mainCam;

	private void Start()
	{
		mainCam = Camera.main;
	}

	private void LateUpdate()
	{
		if (!IsOwner) return;

		Vector3 mousePos = mainCam.ScreenToWorldPoint(inputReader.AimPosition);
		Vector3 dir = mousePos - handTrm.position;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		handTrm.rotation = Quaternion.Euler(0, 0, angle);
	}
}
