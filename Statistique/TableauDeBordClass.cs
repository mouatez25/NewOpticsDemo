using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewOptics.Statistique
{
    public partial class TableauDeBordClass
    {
        public TableauDeBordClass()
        {

        }
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }

        public int Nombre { get; set; }
        public Decimal TotalCA { get; set; }
        public Decimal TotalConsomm { get; set; }
        public Decimal TotalMarge { get; set; }
        public Decimal TotalMoyen { get; set; }
        public Decimal RemiseFournisseur { get; set; }
        public Decimal RemiseClient { get; set; }
    }
    public partial class TableauDeBordList
    {
        public int Code { get; set; }
        public string Libelle { get; set; }
        public Decimal CA { get; set; }
        public Decimal Consommations { get; set; }
        public Decimal Bénéfice { get; set; }
        public Decimal Taux { get; set; }
      
    }
}
