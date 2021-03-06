﻿using XTI_App.Api;
using XTI_App.Tests;

namespace XTI_App.TestFakes
{
    public sealed class EmployeeGroup : AppApiGroupWrapper
    {
        public EmployeeGroup(AppApiGroup source) : base(source)
        {
            var actions = new AppApiActionFactory(source);
            AddEmployee = source.AddAction
            (
                actions.Action
                (
                    nameof(AddEmployee),
                    source.Access.WithAllowed(FakeAppRoles.Instance.Manager),
                    () => new AddEmployeeValidation(),
                    () => new AddEmployeeAction()
                )
            );
            Employee = source.AddAction
            (
                actions.Action
                (
                    nameof(Employee),
                    () => new EmployeeAction(),
                    "Get Employee Information"
                )
            );
            SubmitFakeForm = source.AddAction
            (
                actions.Action(nameof(SubmitFakeForm), () => new SubmitFakeFormAction())
            );
        }
        public AppApiAction<AddEmployeeModel, int> AddEmployee { get; }
        public AppApiAction<int, Employee> Employee { get; }
        public AppApiAction<FakeForm, string> SubmitFakeForm { get; }
    }

}
