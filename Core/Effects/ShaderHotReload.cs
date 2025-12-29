using KBO.FXC;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Content.Sources;
using ReLogic.OS;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
#if DEBUG 
internal sealed class ShaderHotReload : ModSystem // gracias puta
{

    static string fxcPath;
    static string modsource;
    bool unloaded;
    public override void Load()
    {
        modsource = Environment.GetEnvironmentVariable("TXM_MODSOURCEPATH")!;
        if (Directory.Exists(modsource) && Platform.IsWindows)
        {
            _ = Task.Run(() => RunFileWatcher());
            //fxcPath = Path.Combine(modsource, "Core", "Effects", "Compiler", "fxc.exe")
        }
        else
        {
            modsource = null;
        }
    }
    public override void Unload()
    {
        unloaded = true;
    }
    private async Task RunFileWatcher()
    {
        await Task.Yield();
        FileSystemWatcher watcher = new(modsource, "*.fx");
        watcher.IncludeSubdirectories = true;
        while (true)
        {
            var res = watcher.WaitForChanged(WatcherChangeTypes.All, 1);
            if (unloaded) return;
            try
            {
                if (Path.GetExtension(res.Name.AsSpan()).Equals(".fx", StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(8).ConfigureAwait(false);
                    // this ignore the msbuild args, if required pass them as a parseable file 

                    string fullPath = Path.GetFullPath(Path.Combine(modsource, res.Name!));
                    string assetName = Path.GetRelativePath(modsource, AssetPathHelper.CleanPath(fullPath)).Replace('\\', '/');
                    Main.NewText($"Recompiling shader: {assetName}...");
                    var hresult = EffectCompiler.CompileEffect(fullPath!, [], [], D3DBindings.CompilerFlags.Debug, out byte[] shaderBinary, out string? diagnostics);
                    Main.NewText($"Recompiled shader: {assetName}: {hresult.code} {diagnostics}");

                    if (hresult.code == 0)
                    {
                        assetName = Path.ChangeExtension(assetName, null);
                        var asset = Mod.Assets.Request<Effect>(assetName, AssetRequestMode.DoNotLoad);

                        _ = Main.RunOnMainThread(() =>
                        {
                            var effect = new Effect(Main.instance.GraphicsDevice, shaderBinary);
                            var oldValue = asset.Value;

                            typeof(Asset<Effect>).GetField("ownValue", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)!
                                .SetValue(asset, effect);
                            // not disposed to prevent things from exploding
                            // this is a slight memory leak but ehhh
                            // if your pc can survive SLR playthroughs, it can survive 
                            // leaking a few objects during debugging
                            //oldValue.Dispose(); 

                        });
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
#endif