using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;


public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    public GameObject animatorOBJ;
    private Animator animator;

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
    }



    public void doSlashAnimation(Entity target){

        StartCoroutine(playSlashAnima(target));
        
    }
    
    public void doClashAnimation()
    {
        clashSparkPS.transform.position = clashSparkStartPos; // Reset to original position

        Vector2 randomOffset = Random.insideUnitCircle * clashSparkOffset; // Random offset within a circle of radius X
        clashSparkPS.transform.position += new Vector3(randomOffset.x, randomOffset.y, 0f); //sets a new random position for the PS
        clashSparkPS.Play();
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

}
