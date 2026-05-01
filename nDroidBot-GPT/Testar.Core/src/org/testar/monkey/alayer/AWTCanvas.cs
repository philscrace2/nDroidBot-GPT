using System.IO;

namespace org.testar.monkey.alayer
{
    public sealed class AWTCanvas
    {
        private static readonly byte[] FallbackPngBytes = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO9H7f8AAAAASUVORK5CYII=");
        private readonly byte[] pngBytes;

        public AWTCanvas()
        {
            pngBytes = FallbackPngBytes;
        }

        public AWTCanvas(byte[] pngBytes)
        {
            this.pngBytes = pngBytes != null && pngBytes.Length > 0 ? pngBytes : FallbackPngBytes;
        }

        public void SaveAsPng(string path)
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(path, pngBytes);
        }

        public bool HasData()
        {
            return pngBytes.Length > 0;
        }
    }
}
