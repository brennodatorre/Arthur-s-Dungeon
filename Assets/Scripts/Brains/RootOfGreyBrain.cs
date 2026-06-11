using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootOfGreyBrain : Brain
{

    public GameObject cinzentadoPrefab;

    private Entity cinzentado;

    public override void WakeUp()
    {
        // find boss
        cinzentado = RoundManager.Instance.enemies.Find(e => e.entityID == cinzentadoPrefab.GetComponent<Entity>().entityID);
    }

    public override void DyingAction()
    {
        //tells the boss to remove itself from their list
        cinzentado.GetComponent<CinzentadoBrain>().RemoveRoot(GetComponent<Entity>());
    }

    public override void getIntent()
    {
        currentIntent = Intent.SPECIAL;

        specialActionToUse = () => {              };
    }
}
