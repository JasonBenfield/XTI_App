namespace XTI_Forms
{
    public interface IConstraint
    {
        ConstraintResult Test(object value);
        ConstraintModel ToModel();
    }

    public interface IConstraint<T> : IConstraint
    {
        ConstraintResult Test(T value);
    }
}
