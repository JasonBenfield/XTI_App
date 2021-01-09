using System;
using System.Collections.Generic;
using System.Linq;
using XTI_Core;

namespace XTI_Forms
{
    public abstract class SimpleField<T> : IField<T>
    {
        private readonly ConstraintCollection<T> constraints;

        protected SimpleField(string prefix, string name)
        {
            Key = new FieldKey(prefix, name);
            Caption = string.Join(" ", new CamelCasedWord(name).Words());
            constraints = new ConstraintCollection<T>();
        }

        protected SimpleField()
        {
        }

        public FieldKey Key { get; }
        public string Name { get => Key.Name; }
        public string Caption { get; set; }

        public SimpleField<T> MustNotBeNull()
        {
            constraints.MustNotBeNull();
            return this;
        }

        public SimpleField<T> AddConstraints(RangeConstraint<T> rangeConstraint)
            => AddConstraints(rangeConstraint.Constraints());

        public SimpleField<T> AddConstraints(params IConstraint<T>[] constraintsToAdd)
            => AddConstraints((IEnumerable<IConstraint<T>>)constraintsToAdd);

        public SimpleField<T> AddConstraints(IEnumerable<IConstraint<T>> constraintsToAdd)
        {
            constraints.Add(constraintsToAdd);
            return this;
        }

        public void Export(IDictionary<string, object> values)
        {
            values.Add(Key.Value(), Value());
        }

        public void Import(IDictionary<string, object> values)
        {
            if (values.TryGetValue(Key.Value(), out var value))
            {
                SetValue(new ConvertedValue<T>(value).Value());
            }
        }

        private T value;

        public void SetValue(T value) => this.value = value;

        public void Validate(ErrorList errors)
        {
            Validating(errors);
            constraints.Validate(errors, this);
        }

        protected virtual void Validating(ErrorList errors) { }

        public T Value() => value;

        object IField.Value() => Value();

        public void SkipValidation() => constraints.SkipValidation();

        public void UnskipValidation() => constraints.UnskipValidation();

        public ErrorModel Error(string message) => new ErrorModel(message, Caption, Key.Value());

        public FieldModel ToModel()
        {
            var model = _ToModel();
            model.Name = Key.Name;
            model.Caption = Caption;
            model.Constraints = constraints.Values().Select(c => c.ToModel()).ToArray();
            model.InputDataType = typeof(T);
            model.IsNullAllowed = constraints.IsNullAllowed;
            model.Value = Value();
            return model;
        }

        protected virtual SimpleFieldModel _ToModel() => new SimpleFieldModel();
    }
}
