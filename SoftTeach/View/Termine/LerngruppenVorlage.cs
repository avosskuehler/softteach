using SoftTeach.Model.TeachyModel;
using SoftTeach.ViewModel.Datenbank;
using SoftTeach.ViewModel.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftTeach.View.Termine
{
  /// <summary>
  /// ViewModel of an individual Lerngruppe
  /// </summary>
  public class LerngruppenVorlage : ViewModelBase
  {
    /// <summary>
    /// The schuljahr currently assigned to this Lerngruppe
    /// </summary>
    private SchuljahrViewModel schuljahr;

    /// <summary>
    /// The fach currently assigned to this Lerngruppe
    /// </summary>
    private FachViewModel fach;
    private bool istBetroffen;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="LerngruppeViewModel"/> Klasse. 
    /// </summary>
    /// <param name="lerngruppe">
    /// The underlying schülerliste this ViewModel is to be based on
    /// </param>
    public LerngruppenVorlage(LerngruppeNeu lerngruppe)
    {
      if (lerngruppe == null)
      {
        throw new ArgumentNullException("lerngruppe");
      }

      this.Model = lerngruppe;
      this.UpdateLerngruppeCommand = new DelegateCommand(this.UpdateLerngruppe);
    }

    /// <summary>
    /// Holt den Befehl zur adding a new Schüler
    /// </summary>
    public DelegateCommand UpdateLerngruppeCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Lerngruppe this ViewModel is based on
    /// </summary>
    public LerngruppeNeu Model { get; private set; }

    public bool IstBetroffen
    {
      get => istBetroffen;
      set
      {
        istBetroffen = value;
        this.RaisePropertyChanged("IstBetroffen");
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung dieser Lerngruppe.
    /// </summary>
    public string LerngruppeBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }
    }

    /// <summary>
    /// Holt oder setzt die Schuljahr currently assigned to this Lerngruppe
    /// </summary>
    public SchuljahrViewModel LerngruppeSchuljahr
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Schuljahr == null)
        {
          return null;
        }

        if (this.schuljahr == null || this.schuljahr.Model != this.Model.Schuljahr)
        {
          this.schuljahr = App.MainViewModel.Schuljahre.SingleOrDefault(d => d.Model == this.Model.Schuljahr);
        }

        return this.schuljahr;
      }
    }

    /// <summary>
    /// Holt oder setzt die Fach currently assigned to this Lerngruppe
    /// </summary>
    public FachViewModel LerngruppeFach
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Fach == null)
        {
          return null;
        }

        if (this.fach == null || this.fach.Model != this.Model.Fach)
        {
          this.fach = App.MainViewModel.Fächer.SingleOrDefault(d => d.Model == this.Model.Fach);
        }

        return this.fach;
      }
    }

    /// <summary>
    /// Holt oder setzt den Jahrgang dieser Lerngruppe.
    /// </summary>
    public int LerngruppeJahrgang
    {
      get
      {
        return this.Model.Jahrgang;
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return this.LerngruppeBezeichnung;
    }


    /// <summary>
    /// Ändert den betroffenenstatus der Lerngruppe
    /// </summary>
    public void UpdateLerngruppe()
    {
      this.IstBetroffen = !this.istBetroffen;
    }
  }
}
