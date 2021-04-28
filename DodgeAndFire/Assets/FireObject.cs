using System.Collections;
using UnityEngine;

public class FireObject : MonoBehaviour
{
    public float power;
    public Rigidbody2D rb;
    public ParticleSystem trail;
    public CircleCollider2D cc;

    private void Update()
    {
        if(transform.position.y < -20)
        {
            FireExplode(0, "Fall");

            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            cc.enabled = false;

            trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            GameObject particle = FireManager.Instance.pullParticle();
            particle.transform.position = transform.position;
            particle.SetActive(true);

            FireManager.Instance.pushObj(gameObject);
            FireManager.Instance.pushParticle(particle);
        }
    }
    public void ForceFire(Vector3 forceVec, int count)
    {
        name = ServerManager.Instance.userName + count;

        rb.gravityScale = 1.0f;
        cc.enabled = true;
        trail.Play();
        rb.AddForce(forceVec * power);

        FireClass fc = new FireClass();
        fc.gP = transform.position;
        fc.oN = ServerManager.Instance.userName + count;
        fc.v = forceVec;

        ServerManager.Instance.PlayerFire(fc);
        count++;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag != "Wall" && collision.transform.tag != "Enemy")
            return;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        cc.enabled = false;

        trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        GameObject particle = FireManager.Instance.pullParticle();
        particle.transform.position = transform.position;
        particle.SetActive(true);

        if(collision.transform.tag == "Wall")
            FireExplode(1, "Wall");
        else if(collision.transform.tag == "Enemy")
            FireExplode(2, collision.name);

        StartCoroutine(waitSecond(particle));
    }
    IEnumerator waitSecond(GameObject particle)
    {
        yield return new WaitForSeconds(1.2f);
        FireManager.Instance.pushObj(gameObject);
        FireManager.Instance.pushParticle(particle);
    }
    private void FireExplode(int type, string Name)
    {
        FireExplodeClass fec = new FireExplodeClass();
        fec.oN = name;
        fec.tT = type;
        fec.tN = Name;

        ServerManager.Instance.FireExplode(fec);
    }
}
