using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationalInstitution.Domain.Enums
{
    public enum UserRole
    {
        [Description("Administrador")]
        Admin = 1,

        [Description("Coordinador")]
        Coordinator = 2,

        [Description("Profesor")]
        Professor = 3,

        [Description("Estudiante")]
        Student = 4
    }
}
