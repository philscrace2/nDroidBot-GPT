using System.IO;

namespace org.testar.monkey.alayer
{
    public sealed class AWTCanvas
    {
        public void SaveAsPng(string path)
        {
            using var stream = File.Create(path);
        }
    }
}
