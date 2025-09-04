using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;


public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    public GameObject animatorOBJ;
    private Animator animator;
    private Canvas canvas;

    [Space]
    public bool gizmos = false;

    [Space]
    public Vector3 slashAnimaOffset;

    [Space]
    public ParticleSystem clashSparkPS;
    public float clashSparkOffset = 0.5f;
    Vector3 clashSparkStartPos;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }






    }

    void Start()
    {
        animator = animatorOBJ.GetComponent<Animator>();
        clashSparkStartPos = clashSparkPS.transform.position;
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>(); ;

        clashSparkStartPos = canvas.transform.position;
        clashSparkPS.transform.position = clashSparkStartPos;

    }



    public void doSlashAnimation(Entity target)
    {

        StartCoroutine(playSlashAnima(target));

    }

    public void doClashAnimation()
    {


        Vector2 randomOffset = Random.insideUnitCircle * clashSparkOffset; // Random offset within a circle of radius X
        clashSparkPS.transform.position = clashSparkStartPos;
        clashSparkPS.transform.position += new Vector3(randomOffset.x, randomOffset.y, 0f); //sets a new random position for the PS
        clashSparkPS.Play();
        //clashSparkPS.transform.position = clashSparkStartPos;


    }

    //does Dissolve effect on the entity upon death
    public IEnumerator DissolveUponDeath(Image sprite, bool revert = false)
    {

        Material mat = new Material(sprite.material);
        sprite.material = mat;

        float fade = 1;
        if (revert) { fade = 0; }

        while (revert ? fade < 1 : fade > 0)
        {
            if (revert) { fade += Time.deltaTime * 0.5f; } //fade in over time
            else { fade -= Time.deltaTime * 0.5f; } //fade out over time

            mat.SetFloat("_Fade", fade);
            yield return null; //wait for next frame
        }



    }

    void OnDrawGizmos()
    {
        // Draw the clash spark radius as a wire disc
        if (clashSparkPS != null && gizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(clashSparkStartPos, clashSparkOffset);

            // Draw the last random offset position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(clashSparkPS.transform.position, 5f);
        }
    }

    private IEnumerator playSlashAnima(Entity target)
    {

        animatorOBJ.GetComponent<UnityEngine.UI.Image>().enabled = true;

        animatorOBJ.transform.position = target.transform.position + slashAnimaOffset;

        animator.SetTrigger("doSlash");

        yield return null; //waits for next turn


        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float animationLength = clipInfo.Length > 0 ? clipInfo[0].clip.length : 0.5f;


        yield return new WaitForSeconds(animationLength);


        animatorOBJ.GetComponent<UnityEngine.UI.Image>().enabled = false;



    }

    public IEnumerator doBarChangeAnimation(Image bar, float ratio)
    {
        bool ratioIsBigger = ratio > bar.fillAmount;
        while (ratioIsBigger ? bar.fillAmount < ratio : bar.fillAmount > ratio)
        {
            if (ratioIsBigger) { bar.fillAmount += Time.deltaTime * 0.5f; }
            else { bar.fillAmount -= Time.deltaTime * 0.5f; }

            yield return null;
        }
    }

}
