using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Properties 
{
    public enum ActionType { Main, Sup, Bonus };

    public enum Origin { ROSES, HEX, LANDREAS, ARTHUR, SYSTEM, UNKNOWN, SURVIVOR, FLAME };

    
    public enum Target { SingleEnemy, Multi, Self, SingleAlly };


    public enum Property
    {
        #region regular properties 0-1000



        ROCKY = 0,
        WOOD = 1,
        FLESHY = 2,
        WET = 3,


        
        
        #endregion



        #region entity properties 1001 - 2000




        BIT = 1001,



        OBSTACLE = 1998,
        MINI_BOSS = 1999,
        BOSS = 2000,

        #endregion


    }


}
