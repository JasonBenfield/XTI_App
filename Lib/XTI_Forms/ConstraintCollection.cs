using System.Collections.Generic;
using XTI_Core;

namespace XTI_Forms
{
    public interface IConstraintCollection
    {
        IConstraint[] Values();
        void Validate(ErrorList errors, IField field);
    }
    public sealed class ConstraintCollection<T> : IConstraintCollection
    {
        private readonly List<IConstraint<T>> constraints = new List<IConstraint<T>>();

        public IConstraint<T>[] Values() => constraints.ToArray();
        IConstraint[] IConstraintCollection.Values() => Values();

        public bool IsNullAllowed { get; private set; } = true;

        public void MustNotBeNull()
        {
            IsNullAllowed = false;
        }

        private bool skipped = false;

        public void SkipValidation() => skipped = true;

        public void UnskipValidation() => skipped = false;

        public void Add(params IConstraint<T>[] constraintsToAdd)
            => Add((IEnumerable<IConstraint<T>>)constraintsToAdd);

        public void Add(IEnumerable<IConstraint<T>> constraintsToAdd)
        {
            if (constraintsToAdd != null)
            {
                constraints.AddRange(constraintsToAdd);
            }
        }

        public void Add(RangeConstraint<T> rangeConstraint)
        {
            constraints.AddRange(rangeConstraint.Constraints());
        }

        public void Validate(ErrorList errors, IField field)
        {
            if (!skipped)
            {
                var value = field.Value();
                if (value == null)
                {
                    if (!IsNullAllowed)
                    {
                        errors.Add(field.Error(FormErrors.MustNotBeNull));
                    }
                    return;
                }
                foreach (var constraint in constraints)
                {
                    var result = constraint.Test(value);
                    if (!result.IsValid)
                    {
                        errors.Add(field.Error(result.ErrorMessage));
                        return;
                    }
                }
            }
        }
    }
}
