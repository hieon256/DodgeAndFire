using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    private static FireManager instance;
    public static FireManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Start()
    {
        instance = GetComponent<FireManager>();
    }


    public GameObject fireObject;
    Stack<GameObject> fireStack = new Stack<GameObject>();

    public GameObject enemyFireObject;
    Stack<GameObject> enemyFireStack = new Stack<GameObject>();

    List<GameObject> EnemyFireList = new List<GameObject>();

    public GameObject collisionObject;
    Stack<GameObject> collisionStack = new Stack<GameObject>();


    public void EnemyFire(string playerName, FireClass fc)
    {
        GameObject ef = pullEnemyObj();
        EnemyFireList.Add(ef);
        ef.SetActive(true);
        ef.name = fc.oN;
        ef.transform.position = fc.gP;
        ef.GetComponent<EnemyFireObject>().ForceFire(fc.v);
    }
    public void EnemyFireExplode(string playerName, FireExplodeClass fec)
    {
        for(int i = 0; i < EnemyFireList.Count; i++)
        {
            if (EnemyFireList[i].name == fec.oN)
            {
                EnemyFireObject script = EnemyFireList[i].GetComponent<EnemyFireObject>();
                script.FireExplode();

                if(fec.tT == 2)
                {
                    BattleManager.Instance.DamagePlayer(fec.tN);
                }
                EnemyFireList.Remove(EnemyFireList[i]);
                return;
            }
        }
    }
    public void pushObj(GameObject obj)
    {
        obj.SetActive(false);
        fireStack.Push(obj);
    }
    public GameObject pullObj()
    {
        GameObject obj;
        if(fireStack.Count > 0)
        {
            obj = fireStack.Pop();
        }
        else
        {
            obj = createObj();
        }
        return obj;
    }
    public GameObject createObj()
    {
        GameObject obj = Instantiate(fireObject);
        return obj;
    }

    public void pushEnemyObj(GameObject obj)
    {
        obj.SetActive(false);
        enemyFireStack.Push(obj);
    }
    public GameObject pullEnemyObj()
    {
        GameObject obj;
        if (enemyFireStack.Count > 0)
        {
            obj = enemyFireStack.Pop();
        }
        else
        {
            obj = createEnemyObj();
        }
        return obj;
    }
    public GameObject createEnemyObj()
    {
        GameObject obj = Instantiate(enemyFireObject);
        return obj;
    }

    public void pushParticle(GameObject particle)
    {
        particle.SetActive(false);
        collisionStack.Push(particle);
    }
    public GameObject pullParticle()
    {
        GameObject particle;
        if (collisionStack.Count > 0)
        {
            particle = collisionStack.Pop();
        }
        else
        {
            particle = createParticle();
        }
        return particle;
    }
    public GameObject createParticle()
    {
        GameObject particle = Instantiate(collisionObject);
        return particle;
    }
}
