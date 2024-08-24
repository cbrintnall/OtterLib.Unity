using System;
using UnityEngine;

public static class AnimationUtilities
{
    public static bool IsAnimationPlaying(this Animator animator, string name) =>
        animator.GetCurrentAnimatorStateInfo(0).IsName(name);

    public static bool IsAnimationPlaying(this Animator animator, int layer, string name) =>
        animator.GetCurrentAnimatorStateInfo(layer).IsName(name);
}
