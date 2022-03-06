using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stam : MonoBehaviour
{
    public int curStam = 0;
    public int maxStam = 100;

    public StamBar stamBar;

    void Start()
    {
        curStam = maxStam;
    }

    void Update()
    {
        if( Input.GetKeyDown( KeyCode.Space ) )
        {
            DamagePlayer(10);
        }
    }

    public void DamagePlayer( int damage )
    {
        curStam -= damage;

        stamBar.SetStam( curStam );
    }
}
