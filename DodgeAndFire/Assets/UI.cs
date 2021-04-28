using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text playerName;

    public Slider Hp;
    public Image Background;

    public int maxHp;
    public int hp;

    public Vector3 originPos;

    bool isCollide;
    float collideStartTime;
    float collideEndTime = 0.5f;

    private void Start()
    {
        originPos = Hp.transform.localPosition;    
    }
    private void Update()
    {
        if (Background.fillAmount > Hp.value)
        {// 데미지 받았을 시.
            Background.fillAmount = Mathf.Lerp(Background.fillAmount, Hp.value - 0.1f, 0.015f);
            Hp.transform.localPosition = (Vector3)Random.insideUnitCircle * 0.5f * (Background.fillAmount - Hp.value) + originPos;
        }
        else
            Background.fillAmount = Mathf.Lerp(Background.fillAmount, Hp.value + 0.1f, 0.015f);
    }
}
