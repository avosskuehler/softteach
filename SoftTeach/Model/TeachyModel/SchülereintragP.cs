using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftTeach.Model.TeachyModel
{
  public partial class SchülereintragNeu
  {
    public override string ToString()
    {
      return string.Format("{0} {1}", this.Person != null ? this.Person.Vorname : "VN", this.Person != null ? this.Person.Nachname : "NN");
    }
  }
}
