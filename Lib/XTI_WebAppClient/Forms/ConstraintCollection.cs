namespace XTI_WebAppClient.Forms;

public class ConstraintCollection<T>
{
    private readonly List<IConstraint<T>> constraints = new List<IConstraint<T>>();
    private bool isNullAllowed = true;

    public void MustNotBeNull()
    {
        isNullAllowed = false;
    }

    private bool skipped = false;

    public void SkipValidation() { skipped = true; }

    public void UnskipValidation() { skipped = false; }

    public void Add(IConstraint<T> constraint)
    {
        constraints.Add(constraint);
    }

    public void Validate(ErrorList errors, IField field)
    {
        if (!skipped)
        {
            var value = field.Value();
            if (value == null)
            {
                if (!isNullAllowed)
                {
                    errors.Add(field.Error($"{field.FieldCaption} is required"));
                }
            }
            else
            {
                foreach (var c in constraints)
                {
                    var result = c.Test(field.FieldCaption, value);
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