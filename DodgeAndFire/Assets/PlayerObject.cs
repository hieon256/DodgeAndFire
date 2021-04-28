using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    public Rigidbody2D rb;

    public UI UI;

    public float jumpPower;
    public int MaxJumpCount;

    private int JumpCount;

    Touch FiringTouch;

    int fireCount;

    bool Firing;
    public float FireRate;
    float FireTime;

    float[] startTime = new float[5];
    private void Start()
    {
        JumpCount = MaxJumpCount;
    }
    private void Update()
    {
        FireTime += Time.deltaTime;

        if(transform.position.y < -20)
        {
            ServerManager.Instance.CloseConnection();
        }

        if(UI != null)
        {
            UI.transform.position = transform.position;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
#endif

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            if (t.phase == TouchPhase.Began)
            {
                startTime[t.fingerId] = Time.time;
            }
            if (t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved)
            {
                if (Time.time - startTime[t.fingerId] >= 0.5f && FireTime >= FireRate && (!Firing || t.fingerId == FiringTouch.fingerId))
                {
                    FiringTouch = t;
                    Fire();
                    FireTime = 0;
                    Firing = true;
                }
            }
            if (t.phase == TouchPhase.Ended)
            {
                if (Time.time - startTime[t.fingerId] < 0.5f && JumpCount > 0)
                {
                    JumpCount--;
                    Dodge(t);
                }
                if (t.fingerId == FiringTouch.fingerId)
                {
                    Firing = false;
                }
            }
        }

        CharacterClass cc = new CharacterClass();
        cc.pos = transform.position;
        cc.hp = UI.hp;

        ServerManager.Instance.PlayerUpdate(cc);
    }
    public void Damage(int deal)
    {
        UI.hp -= deal;
        if (UI.hp <= 0)
        {
            UI.hp = 0;

            ServerManager.Instance.CloseConnection();
        }
        UI.Hp.value = (float)UI.hp / UI.maxHp;
    }
    private void Dodge(Touch t)
    {
        Vector2 vec = new Vector2(t.position.x - Screen.width / 2,t.position.y);
        vec.Normalize();

        rb.velocity = Vector2.zero;
        rb.AddForce(vec * jumpPower);
    }
    private void Fire()
    {
#if UNITY_ANDROID
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(new Vector3(FiringTouch.position.x, FiringTouch.position.y, 10));
#endif
#if UNITY_EDITOR
        clickPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
#endif
        Vector3 fireVec = clickPos - transform.position;
        fireVec.Normalize();

        GameObject fire = FireManager.Instance.pullObj();
        fire.transform.position = transform.position;
        fire.SetActive(true);
        fire.GetComponent<FireObject>().ForceFire(fireVec, fireCount);
        fireCount++;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = Vector2.zero;
        JumpCount = MaxJumpCount;
    }
}
