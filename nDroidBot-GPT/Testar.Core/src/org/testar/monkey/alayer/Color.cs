using org.testar.monkey;

namespace org.testar.monkey.alayer
{
    [Serializable]
    public sealed class Color
    {
        public static readonly Color Red = from(255, 0, 0, 255);
        public static readonly Color DeepPink = from(255, 20, 147, 255);
        public static readonly Color Aqua = from(0, 128, 255, 255);
        public static readonly Color Moss = from(0, 128, 64, 255);
        public static readonly Color Salmon = from(255, 102, 102, 255);
        public static readonly Color Blue = from(0, 0, 255, 255);
        public static readonly Color Navy = from(0, 0, 128, 255);
        public static readonly Color CornflowerBlue = from(100, 149, 237, 255);
        public static readonly Color SteelBlue = from(70, 130, 180, 255);
        public static readonly Color BlueViolet = from(138, 43, 226, 255);
        public static readonly Color Green = from(0, 255, 0, 255);
        public static readonly Color LimeGreen = from(50, 205, 50, 255);
        public static readonly Color Yellow = from(255, 255, 0, 255);
        public static readonly Color Gold = from(255, 215, 0, 255);
        public static readonly Color White = from(255, 255, 255, 255);
        public static readonly Color Black = from(0, 0, 0, 255);

        public static Color from(int red, int green, int blue, int alpha)
        {
            return new Color(red, green, blue, alpha);
        }

        private readonly int redValue;
        private readonly int greenValue;
        private readonly int blueValue;
        private readonly int alphaValue;
        private readonly int argb32Value;

        private Color(int red, int green, int blue, int alpha)
        {
            Assert.isTrue(red >= 0 && green >= 0 && blue >= 0 && alpha >= 0 &&
                          red <= 255 && green <= 255 && blue <= 255 && alpha <= 255);
            redValue = red;
            greenValue = green;
            blueValue = blue;
            alphaValue = alpha;
            argb32Value = red + (green << 8) + (blue << 16) + (alpha << 24);
        }

        public int red() => redValue;
        public int green() => greenValue;
        public int blue() => blueValue;
        public int alpha() => alphaValue;
        public int argb32() => argb32Value;

        public override int GetHashCode()
        {
            return argb32Value;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Color other)
            {
                return other.alphaValue == alphaValue &&
                       other.redValue == redValue &&
                       other.greenValue == greenValue &&
                       other.blueValue == blueValue;
            }

            return false;
        }

        public override string ToString()
        {
            return $"Color (red: {red()} green: {green()} blue: {blue()} alpha: {alpha()})";
        }
    }
}
