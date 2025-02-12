using TMPro;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;
using Mono.Cecil;
using static EnemyScript;

public class UIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Animator goldAnimator;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Animator healthAnimator;
    [SerializeField] private Image wind;
    [SerializeField] private Image water;
    [SerializeField] private Image fire;
    [SerializeField] private Image earth;
    [SerializeField] private Image barricade;
    [SerializeField] private Button roundButton;
    [SerializeField] private TextMeshProUGUI roundButtonText;
    void Start()
    {
    }

    void Update()
    {
        
    }
    public void activeTurret(string type)
    {
        switch (type)
        {
            case "wind":
                wind.transform.localScale = wind.transform.localScale * 1.3f;
                break;
            case "water":
                water.transform.localScale = water.transform.localScale * 1.3f;
                break;
            case "fire":
                fire.transform.localScale = fire.transform.localScale * 1.3f;
                break;
            case "earth":
                earth.transform.localScale = earth.transform.localScale * 1.3f;
                break;
            case "barricade":
                barricade.transform.localScale = barricade.transform.localScale * 1.3f;
                break;
        }
    }

    public void RoundButtonActive(bool active)
    {
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        int round = spawner.wave;
        if(active)
        {
            roundButton.gameObject.SetActive(true);
           roundButtonText.text = "Start round " + round.ToString(); ;
        }
        else
        {
            roundButton.gameObject.SetActive(false);
        }

    }

    public void cancelActiveTurret()
    {
        wind.transform.localScale = Vector3.one;
        water.transform.localScale = Vector3.one;
        fire.transform.localScale = Vector3.one;
        earth.transform.localScale = Vector3.one;
        barricade.transform.localScale = Vector3.one;
    }
    public void UpdateHealth(int value)
    {
        healthAnimator.SetTrigger("goldGain");
        healthText.text = value.ToString();

    }
    public void UpdateGold(int value)
    {
        goldAnimator.SetTrigger("goldGain");
        goldText.text = value.ToString();
        
    }
}
