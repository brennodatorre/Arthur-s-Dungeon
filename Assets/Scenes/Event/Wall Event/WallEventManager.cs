
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Trait = PlayerData.Trait;

public class WallEventManager : MonoBehaviour
{
    
    public int jumpDC = 10;
    public int atkDC = 12;
    public int digDC = 14;


    [Space(10)]

    public GameObject wall;
    public GameObject Text;

    public GameObject leaveBtn;

    public GameObject right;
    public GameObject left;
    public GameObject breakBtn;
    public GameObject jumpBtn;
    public GameObject digBtn;

    public GameObject brickBtn;


    [Space(10)]

    public AudioClip wallBreakSound;
    public AudioClip jumpSound;
    public AudioClip atkFailSound;
    public AudioClip digSound;
    public AudioClip coinSound;
    public List<AudioClip> stepSounds;


    private PlayerData pData;
    private bool brickFound = false;


    public void Start()
    {
        pData = PlayerData.Instance;
    } 


    public void LookLeft()
    {
        left.SetActive(false);
        wall.SetActive(false);
        jumpBtn.SetActive(false);
        breakBtn.SetActive(false); 
        right.GetComponent<Button>().onClick.RemoveAllListeners();
        right.GetComponent<Button>().onClick.AddListener(LookMiddle);

        var colors = right.GetComponent<Button>().colors;
        colors.normalColor = Color.white;
        right.GetComponent<Button>().colors = colors;

        int rand = Random.Range(0, stepSounds.Count);
        AudioManager.Instance.PlaySound(stepSounds[rand]);

        Text.SetActive(false);
        Text.GetComponent<TextMeshProUGUI>().text = "You see a old shovel, you can try digging below the wall with it.";
        Text.SetActive(true);

        digBtn.SetActive(true);
    }
    public void LookRight()
    {
        right.SetActive(false);
        wall.SetActive(false);
        jumpBtn.SetActive(false);
        breakBtn.SetActive(false); 
        left.GetComponent<Button>().onClick.RemoveAllListeners();
        left.GetComponent<Button>().onClick.AddListener(LookMiddle);

        var colors = left.GetComponent<Button>().colors;
        colors.normalColor = Color.white;
        left.GetComponent<Button>().colors = colors;

        int rand = Random.Range(0, stepSounds.Count - 1);
        AudioManager.Instance.PlaySound(stepSounds[rand]);

        Text.SetActive(false);
        Text.GetComponent<TextMeshProUGUI>().text = "You see nothing of interest.";
        Text.SetActive(true);

        if (!brickFound) brickBtn.SetActive(true);
    }
    public void LookMiddle()
    {
        left.SetActive(true);
        right.SetActive(true);
        digBtn.SetActive(false);
        brickBtn.SetActive(false);
        wall.SetActive(true);
        jumpBtn.SetActive(true);
        breakBtn.SetActive(true);

        Text.SetActive(false);
        Text.GetComponent<TextMeshProUGUI>().text = "You see the wall.";
        Text.SetActive(true);

        int rand = Random.Range(0, stepSounds.Count);
        AudioManager.Instance.PlaySound(stepSounds[rand]);

        left.GetComponent<Button>().onClick.RemoveAllListeners();
        right.GetComponent<Button>().onClick.RemoveAllListeners();
        left.GetComponent<Button>().onClick.AddListener(LookLeft);
        right.GetComponent<Button>().onClick.AddListener(LookRight);
    }


    public void TryJumping()
    {
        int roll =pData.Roll(Trait.DEX);
        AudioManager.Instance.PlaySound(jumpSound);

        if (roll >= jumpDC)
        {
            Continue("You successfully jump over the wall!", jumpSound);
        }
        else
        {
            int lose = pData.getMaxHP() / 100 * 5;
            pData.takeTrueDamage(lose); // take 5% of max hp as true damage on fail
            StartCoroutine(MySceneManager.Instance.doPopUp("-" + lose.ToString() + " HP", wall.transform.position, Color.red));
            Continue("You fail to jump over the wall and hurt yourself.", jumpSound);
        }
    }


    public void TryATK()
    {
        int roll = pData.Roll(Trait.ATLETISM);
        AudioManager.Instance.PlaySound(atkFailSound);

        if (roll >= atkDC)
        {
            Continue("You successfully break the wall!", wallBreakSound);
        }
        else
        {
            int lose = pData.getMaxHP() / 100 * 7;
            pData.takeTrueDamage(lose); // take 7% of max hp as true damage on fail
            StartCoroutine(MySceneManager.Instance.doPopUp("-" + lose.ToString() + " HP", wall.transform.position, Color.red));
            Continue("You broke the wall, but hurt yourself.", atkFailSound);
        }
    }


    public void TryDigging()
    {
        right.SetActive(false);
        digBtn.SetActive(false);

        int roll = pData.Roll(Trait.CONSTITUTION);
        AudioManager.Instance.PlaySound(digSound);

        if (roll >= digDC)
        {
            Continue("You successfully dig through the wall!", digSound);
        }
        else
        {
            int lose = pData.getMaxHP() / 100 * 3;
            pData.takeTrueDamage(lose); // take 3% of max hp as true damage on fail
            StartCoroutine(MySceneManager.Instance.doPopUp("-" + lose.ToString() + " HP", wall.transform.position, Color.red));
            Continue("You dig a hole under the wall, but you hurt yourself trying to squeeze through.", digSound);
        }
    }


    public void foundBrick() {
        Text.SetActive(false);
        Text.GetComponent<TextMeshProUGUI>().text = "You found a loose brick, hidden behind it is a bag of coins.";
        Text.SetActive(true);

        pData.changeIlhas(7);
        AudioManager.Instance.PlaySound(coinSound);

        brickBtn.SetActive(false);
        brickFound = true;
    }





    private void Continue(string message, AudioClip clip)
    {
        Text.SetActive(false);
        wall.SetActive(false);

        AudioManager.Instance.PlaySound(clip);

        
        Text.GetComponent<TextMeshProUGUI>().text = message;
        Text.SetActive(true);

        leaveBtn.SetActive(true);
    }
    
}
