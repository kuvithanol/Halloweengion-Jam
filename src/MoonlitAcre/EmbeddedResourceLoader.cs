using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MoonlitAcre {
    public static class EmbeddedResourceLoader {
        public static void LoadEmbeddedResource(string name) {
            
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            var resourceName = thisAssembly.GetManifestResourceNames().First(r => r.Contains(name));
            var resource = thisAssembly.GetManifestResourceStream(resourceName);

            using MemoryStream memoryStream = new MemoryStream();
            
            //CopyTo stolen from the internets
            
            byte[] buffer = new byte[16384];
            int count;
            while ((count = resource!.Read(buffer, 0, buffer.Length)) > 0)
                memoryStream.Write(buffer, 0, count);
            
            //Loads texture into atlas, thanks for letting me yoink this Henpe (at least I think I yoinked it from you)
            
            Texture2D spriteTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            spriteTexture.LoadImage(memoryStream.ToArray());

            FAtlas atlas = new FAtlas(name, spriteTexture, FAtlasManager._nextAtlasIndex);
            Futile.atlasManager.AddAtlas(atlas);
            FAtlasManager._nextAtlasIndex++;
        }
    }
}