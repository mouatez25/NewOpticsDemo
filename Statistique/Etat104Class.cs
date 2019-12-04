using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewOptics.Statistique
{
    public partial class Etat104Class
    {
        public Etat104Class()
        {

        }

        public int Id { get; set; }
        public Decimal totaltva { get; set; }
        public Decimal totalht { get; set; }
        public string Raison { get; set; }
        public string adresse { get; set; }
        public string mf { get; set; }
        public string ai { get; set; }
        public string rc { get; set; }
    }
}
