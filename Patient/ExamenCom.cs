using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionClinique.Patient
{
    public partial class ExamenCom
    {
        public ExamenCom()
        {

        }
        public bool GroupageRhésus { get; set; }
        public bool FNSEQUILIBRE { get; set; }
        public bool VS { get; set; }
        public bool UREE { get; set; }
        public bool CREA { get; set; }
        public bool GLYC { get; set; }
        public bool CHOLTOTAL { get; set; }
        public bool CHOHDLLDL { get; set; }

        public bool Triglycerides { get; set; }
        public bool ELECHB { get; set; }
        public bool FERSER { get; set; }
        public bool SEROL { get; set; }
        public bool IONOGR { get; set; }
        public bool ACIDEURIQUE { get; set; }
        public bool T3T4TSH { get; set; }
        public bool ECBUATB { get; set; }
        public bool COPRO { get; set; }
    }
}
