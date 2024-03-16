using System;
using System.Collections;
using UnityEngine;

public class WaitForAll : CustomYieldInstruction
{
    public override bool keepWaiting => routine.MoveNext();

    private IEnumerator routine;

    public WaitForAll(params YieldInstruction[] routines)
    {
        routine = routines.GetEnumerator();
    }
}
