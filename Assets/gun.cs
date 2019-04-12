using UnityEngine;
using UnityEngine.Networking;

public class gun : NetworkBehaviour {

    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 60f;
    public float fireRate = 15f;

    public Camera fpsCam;
    public GameObject impactEffect;
    public ParticleSystem muzzleFlash;
    //public ShootEffect shootEff;

    private float nextTimeToFire = 0f;

    // Update is called once per frame
    void Update () {
		if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire) {

            nextTimeToFire = Time.time + 1 / fireRate;
            Shoot();
        }
	}

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    [ClientRpc]
    void RpcDoShootEffect()
    {
        Debug.Log("shoot not local");
        muzzleFlash.Play();
    }

    [Client]
    void Shoot() {

        if(!isLocalPlayer)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) {

            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();

            if(target != null)
            {
                target.takeDamage(damage);
            }

            if(hit.rigidbody)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 0.2f);
        }
    }
}
