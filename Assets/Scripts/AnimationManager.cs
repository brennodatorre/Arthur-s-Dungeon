using System.Collections;


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


    [Space(7)]
    public float shakeAnimationMagnitude = 0.1f;

    [Space(7)] [Header ("Turn Step")]
    public float ts_speed = 30f;
    public float ts_offset = 40f;
    public float ts_scale = 1.08f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Color normalColor = Color.white;
    public Color backStepColor = new Color(0.6f, 0.6f, 0.6f, 1f);




    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           
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

    public void doClashAnimation()
    {


        Vector2 randomOffset = Random.insideUnitCircle * clashSparkOffset; // Random offset within a circle of radius X
        clashSparkPS.transform.position = clashSparkStartPos;
        clashSparkPS.transform.position += new Vector3(randomOffset.x, randomOffset.y, 0f); //sets a new random position for the PS
        clashSparkPS.Play();
        //clashSparkPS.transform.position = clashSparkStartPos;


    }

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

    public Coroutine doShakeAnimation(GameObject ob, float duration = 0.5f)
    {
        return StartCoroutine(ShakeAnimation(ob, duration));
    }

    private IEnumerator ShakeAnimation(GameObject ob, float duration = 0.5f)
    {
        Vector3 originalPos = ob.transform.position;
        float elapsed = 0.0f;
        

        while (elapsed < duration)
        {

            if (ob == null)
            yield break;

            float x = Random.Range(-1f, 1f) * shakeAnimationMagnitude;
            float y = Random.Range(-1f, 1f) * shakeAnimationMagnitude;

            ob.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        ob.transform.position = originalPos;
    }

  




    public Coroutine TurnStep(GameObject ob, bool forward)
    {
        RectTransform rect = ob.GetComponent<RectTransform>();
        Image img = ob.GetComponent<Image>();

        Vector2 dir = forward ? Vector2.down : Vector2.up;
        float scaleTarget = forward ? ts_scale : (1f / ts_scale);

        return StartCoroutine(Step(rect, img, dir, scaleTarget));
    }

    private IEnumerator Step(RectTransform rect, Image img, Vector2 direction, float scaleMultiplier)
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;

        Color startColor = img != null ? img.color : Color.white;

        Vector2 targetPos = startPos + direction * ts_offset;
        Vector3 targetScale = startScale * scaleMultiplier;

        Color targetColor = direction == Vector2.down
            ? normalColor
            : backStepColor;

        //set it to furdest child if in front
        rect.gameObject.transform.SetAsLastSibling();

        float moved = 0f;

        while (moved < ts_offset)
        {
            float step = Mathf.Min(ts_speed * Time.deltaTime, ts_offset - moved);
            moved += step;

            float t = ease.Evaluate(moved / ts_offset);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            rect.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (img != null)
                img.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        rect.anchoredPosition = targetPos;
        rect.localScale = targetScale;

        if (img != null)
            img.color = targetColor;
    }

}
