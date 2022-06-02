using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel_accounting.EF
{
    public partial class Employee
    {
        public string FIO { get => $"{LastName} {FirstName} {MiddleName}"; }
    }
}
