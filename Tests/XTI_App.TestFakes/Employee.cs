using System;

namespace XTI_App.TestFakes
{
    public sealed class Employee
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public EmployeeType EmployeeType { get; set; }
    }

}
