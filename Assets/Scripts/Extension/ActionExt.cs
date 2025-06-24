using System;
public static class ActionExt
{
    public static void RunExt(this Action action)
    {
        if (action == null)
        {
            return;
        }

        action();
    }

    public static void RunExt<T>(this Action<T> action, T argument)
    {
        if (action == null)
        {
            return;
        }

        action(argument);
    }

    public static void RunExt<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        if (action == null)
        {
            return;
        }

        action(arg1, arg2);
    }

    public static void RunExt<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        if (action == null)
        {
            return;
        }

        action(arg1, arg2, arg3);
    }

    public static void RunExt<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (action == null)
        {
            return;
        }

        action(arg1, arg2, arg3, arg4);
    }
}