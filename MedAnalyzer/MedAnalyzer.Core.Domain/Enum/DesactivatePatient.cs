using System;
using System.Collections.Generic;
using System.Text;

namespace MedAnalyzer.Core.Domain.Enum
{
    public enum DesactivatePatient
    {
        NotFound,
        HasActiveAppointments,
        Success,
        Failed
    }
}
