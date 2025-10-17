using System;
using Terraria;

namespace TerrariaXMario.Utilities;
internal static class Threading
{
    internal static bool IsMainThread => Program.IsMainThread;

    internal static void RunOnMainThread(Action action)
    {
        if (IsMainThread) action();
        else Main.RunOnMainThread(action).Wait();
    }
}
