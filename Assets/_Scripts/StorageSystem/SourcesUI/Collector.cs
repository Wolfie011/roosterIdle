using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : UIDrag
{
    protected override void OnCollide(Building collidedSource)
    {
        //collect the produce
        collidedSource.GetComponent<ISource>().Collect();
    }
}
