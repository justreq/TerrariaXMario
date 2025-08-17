using System;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;

namespace TerrariaXMario.Core;
internal abstract class BasePatch : ILoadable
{
    internal abstract void Patch(Mod mod);
    internal virtual void Unpatch() { }

    void ILoadable.Load(Mod mod) { Patch(mod); }
    void ILoadable.Unload() { Unpatch(); }

    /// <summary>
    /// Helper method for throwing detailed exceptions upon TryGoToNext failures
    /// </summary>
    /// <param name="opcodes">String of opcodes that were targeted in the TryGoToNext</param>
    /// <param name="callerMember">DO NOT SET THIS PARAMETER UNLESS YOU WANT DIABETES</param>
    /// <exception cref="Exception"></exception>
    internal void ThrowError(string opcodes, [CallerMemberName] string callerMember = "")
    {
        throw new Exception($"Failed patch: {GetType().Name}.{callerMember} (couldn't match {opcodes})");
    }
}