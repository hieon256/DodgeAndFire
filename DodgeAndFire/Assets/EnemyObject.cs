using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : MonoBehaviour
{
    public UI UI;

    private void Update()
    {
        if(UI != null)
        {
            UI.transform.position = transform.position;
        }
    }
    public void Damage(int deal)
    {
        UI.hp -= deal;
        if (UI.hp <= 0)
        {
            UI.hp = 0;

        }
        UI.Hp.value = (float)UI.hp / UI.maxHp;
    }
}
