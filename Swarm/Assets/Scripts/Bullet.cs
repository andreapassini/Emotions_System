using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float destroyBulletAfterTime = 3f;

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

    void OnTriggerEnter()
    {
        Destroy(gameObject);
    }
}
