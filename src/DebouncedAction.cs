using System;

public struct DebouncedAction
{
    TimeSince ts;

    public void Run(float rate, Action cb)
    {
        if (ts > rate)
        {
            cb();
            ts = 0f;
        }
    }
}
