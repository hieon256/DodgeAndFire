using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;
    public static BattleManager Instance
    {
        get { return instance; }
    }
    public void Start()
    {
        instance = GetComponent<BattleManager>();
    }

    public GameObject player;
    public PlayerObject playerScript;

    public GameObject enemyObject;
    Stack<GameObject> enemyStack = new Stack<GameObject>();

    List<GameObject> enemyList = new List<GameObject>();

    public Transform UICanvas;
    public GameObject uiObject;
    Stack<GameObject> uiStack = new Stack<GameObject>();

    public List<GameObject> randomSpawn = new List<GameObject>();

    public ParticleSystem deadEffect;
    public void DisconnectGame()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            try
            {
                pushUI(enemyList[0].GetComponent<EnemyObject>().UI.gameObject);
                enemyList[0].GetComponent<EnemyObject>().UI = null;
                pushObj(enemyList[0]);
                enemyList.Remove(enemyList[0]);
            }
            catch { continue; }
        }

        player.SetActive(false);
        playerScript.UI.gameObject.SetActive(false);
    }
    public void ConnectGame()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            try
            {
                pushUI(enemyList[0].GetComponent<EnemyObject>().UI.gameObject);
                enemyList[0].GetComponent<EnemyObject>().UI = null;
                pushObj(enemyList[0]);
                enemyList.Remove(enemyList[0]);
            }
            catch { continue; }
        }

        int ran = Random.Range(0, randomSpawn.Count - 1);

        float spawnX = randomSpawn[ran].transform.position.x;
        float spawnScaleX = randomSpawn[ran].transform.localScale.x;

        float ranX = Random.Range(spawnX - spawnScaleX / 2, spawnX + spawnScaleX / 2);

        player.transform.position = new Vector2(ranX, randomSpawn[ran].transform.position.y);

        playerScript.UI.playerName.text = ServerManager.Instance.userName;
        playerScript.UI.hp = playerScript.UI.maxHp;
        playerScript.UI.Hp.value = 1.0f;

        player.SetActive(true);
        playerScript.UI.gameObject.SetActive(true);

    }
    public void EnemyUpdate(string enemyName, CharacterClass cc)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].name == enemyName)
            {
                enemyList[i].transform.position = cc.pos;
                UI Ui = enemyList[i].GetComponent<EnemyObject>().UI;
                Ui.hp = cc.hp;
                Ui.Hp.value = (float)Ui.hp / Ui.maxHp;
                return;
            }
        }

        GameObject enemy = pullObj();
        enemy.name = enemyName;
        enemy.transform.position = cc.pos;
        enemyList.Add(enemy);

        GameObject ui = pullUI();
        ui.transform.position = enemy.transform.position;
        ui.SetActive(true);

        UI UI = ui.GetComponent<UI>();
        UI.playerName.text = enemyName;
        UI.hp = cc.hp;
        UI.Hp.value = (float)UI.hp / UI.maxHp;
        enemy.GetComponent<EnemyObject>().UI = UI;

        enemy.SetActive(true);
    }
    public void EnemyDelete(string enemyName)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].name == enemyName)
            {
                ParticleSystem ps = Instantiate(deadEffect);
                ps.transform.position = enemyList[i].transform.position;
                ps.Play();
                Destroy(ps, 1.0f);

                pushUI(enemyList[i].GetComponent<EnemyObject>().UI.gameObject);
                enemyList[i].GetComponent<EnemyObject>().UI = null;
                pushObj(enemyList[i]);
                enemyList.Remove(enemyList[i]);
                break;
            }
        }
    }
    public void DamagePlayer(string playerName)
    {
        if (ServerManager.Instance.userName == playerName)
        {
            playerScript.Damage(2);
        }
    }




    public void pushObj(GameObject obj)
    {
        obj.SetActive(false);
        enemyStack.Push(obj);
    }
    public GameObject pullObj()
    {
        GameObject obj;
        if (enemyStack.Count > 0)
        {
            obj = enemyStack.Pop();
        }
        else
        {
            obj = createObj();
        }
        return obj;
    }
    public GameObject createObj()
    {
        GameObject obj = Instantiate(enemyObject);
        return obj;
    }

    public void pushUI(GameObject obj)
    {
        obj.SetActive(false);
        uiStack.Push(obj);
    }
    public GameObject pullUI()
    {
        GameObject obj;
        if (uiStack.Count > 0)
        {
            obj = uiStack.Pop();
        }
        else
        {
            obj = createUI();
        }
        return obj;
    }
    public GameObject createUI()
    {
        GameObject obj = Instantiate(uiObject, UICanvas);
        return obj;
    }
}
