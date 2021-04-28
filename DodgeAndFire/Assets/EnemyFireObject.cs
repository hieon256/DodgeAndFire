using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireObject : MonoBehaviour
{
    public float power;
    public Rigidbody2D rb;
    public ParticleSystem trail;
    public CircleCollider2D cc;

    public void ForceFire(Vector3 forceVec)
    {
        rb.gravityScale = 1.0f;
        cc.enabled = true;
        trail.Play();
        rb.AddForce(forceVec * power);
    }
    public void FireExplode()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        cc.enabled = false;

        trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        GameObject particle = FireManager.Instance.pullParticle();
        particle.transform.position = transform.position;
        particle.SetActive(true);

        StartCoroutine(waitSecond(particle));
    }
    IEnumerator waitSecond(GameObject particle)
    {
        yield return new WaitForSeconds(1.2f);
        FireManager.Instance.pushEnemyObj(gameObject);
        FireManager.Instance.pushParticle(particle);
    }
}
