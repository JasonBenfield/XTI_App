using System.Collections.Generic;
using System.Linq;

namespace XTI_Forms
{
    public class RangeConstraint<T>
    {
        private readonly List<IConstraint<T>> constraints = new List<IConstraint<T>>();

        public static RangeStart<T> FromOnOrAbove(T lowerValue) =>
            new RangeStart<T>(new LowerRangeConstraint<T>(lowerValue, true));

        public static RangeStart<T> FromAbove(T lowerValue) =>
            new RangeStart<T>(new LowerRangeConstraint<T>(lowerValue, false));

        public static RangeConstraint<T> OnOrAbove(T lowerValue) =>
            new RangeConstraint<T>(new LowerRangeConstraint<T>(lowerValue, true));

        public static RangeConstraint<T> Above(T lowerValue) =>
            new RangeConstraint<T>(new LowerRangeConstraint<T>(lowerValue, false));

        public static RangeConstraint<T> OnOrBelow(T upperValue) =>
            new RangeConstraint<T>(new UpperRangeConstraint<T>(upperValue, true));

        public static RangeConstraint<T> Below(T upperValue) =>
            new RangeConstraint<T>(new UpperRangeConstraint<T>(upperValue, false));

        internal RangeConstraint(params IConstraint<T>[] constraints)
        {
            if (constraints != null)
            {
                this.constraints.AddRange(constraints.Where(c => c != null));
            }
        }

        public IEnumerable<IConstraint<T>> Constraints() => constraints;
    }
}
