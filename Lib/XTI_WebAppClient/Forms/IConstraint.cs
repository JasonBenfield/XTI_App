namespace XTI_WebAppClient.Forms;

public interface IConstraint
{
    ConstraintResult Test(string caption, object value);
}

public interface IConstraint<T> : IConstraint
{
    ConstraintResult Test(string caption, T value);
}