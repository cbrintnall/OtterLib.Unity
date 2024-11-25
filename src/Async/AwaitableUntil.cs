using System;
using System.Threading.Tasks;
using UnityEngine;

public static class Async
{
    public static async Awaitable WaitUntil(Func<bool> cb)
    {
        while (!cb())
        {
            await Awaitable.NextFrameAsync();
        }
    }
}
