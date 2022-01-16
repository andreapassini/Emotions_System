using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float destroyBulletAfterTime = 3f;

    public int damage = 20;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyBulletAfterTime());
    }

    public IEnumerator DestroyBulletAfterTime()
    {
        yield return new WaitForSeconds(destroyBulletAfterTime);
        Destroy(gameObject);
    }

	private void OnCollisionEnter(Collision collision)
	{
        collision.transform.GetComponent<Health>().TakeDmg(damage);
        Destroy(gameObject);
    }
}
