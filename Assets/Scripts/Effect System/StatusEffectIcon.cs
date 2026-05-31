using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectIcon : MonoBehaviour
{
    //this works as a memory countainer for the icons, so that they can be updated in realtime
    public StatusEffect _statusEffect;

    


    public void MarkToDie()
    {
        StartCoroutine(DeathRoutine());
    }


    private IEnumerator DeathRoutine()
    {
        yield return AnimationManager.Instance.doShakeAnimation(this.gameObject, 1f);
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (StatusHudManager.Instance != null)
        StatusHudManager.Instance.RemoveIcon(this.gameObject);
    }

}
