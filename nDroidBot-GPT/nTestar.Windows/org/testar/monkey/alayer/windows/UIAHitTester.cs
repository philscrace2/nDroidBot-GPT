using org.testar.monkey.alayer;
using org.testar.monkey;

namespace org.testar.monkey.alayer.windows
{
    public class UIAHitTester : HitTester
    {
        private readonly Widget? owner;

        public UIAHitTester()
        {
        }

        public UIAHitTester(Widget owner)
        {
            this.owner = owner;
        }

        public bool apply(double x, double y)
        {
            return apply(x, y, true);
        }

        public bool apply(double x, double y, bool obscuredByChildFeature)
        {
            if (owner == null)
            {
                return true;
            }

            Shape shape = owner.get(Tags.Shape, Rect.from(0, 0, 0, 0));
            if (!shape.contains(x, y))
            {
                return false;
            }

            if (!obscuredByChildFeature)
            {
                return true;
            }

            foreach (Widget candidate in Util.makeIterable(owner))
            {
                if (ReferenceEquals(candidate, owner))
                {
                    continue;
                }

                if (!candidate.get(Tags.Enabled, true) || candidate.get(Tags.Blocked, false))
                {
                    continue;
                }

                Shape candidateShape = candidate.get(Tags.Shape, Rect.from(0, 0, 0, 0));
                if (candidateShape.contains(x, y))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
