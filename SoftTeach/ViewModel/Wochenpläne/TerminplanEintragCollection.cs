namespace SoftTeach.ViewModel.Wochenpläne
{
  using System.Collections.Generic;
  using System.Linq;

  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// Für jeden Termin der Woche (Unterrichtsstunde, AG, Treffen) wird ein Wochenplaneintrag erstellt.
  /// Er erscheint im Wochenplan.
  /// </summary>
  public class TerminplanEintragCollection : ViewModelBase
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="TerminplanEintragCollection"/> Klasse. 
    /// </summary>
    /// <param name="wochentagIndex"> The wochentag Index. </param>
    /// <param name="einträge">Die Liste der Wochenplaneinträge.</param>
    public TerminplanEintragCollection(int wochentagIndex, List<TerminplanEintrag> einträge)
    {
      this.WochentagIndex = wochentagIndex;
      this.TerminplanEinträge = einträge;
      this.ErsteUnterrichtsstundeIndex = einträge.Min(o => o.ErsteUnterrichtsstundeIndex);
      this.LetzteUnterrichtsstundeIndex = einträge.Max(o => o.LetzteUnterrichtsstundeIndex);
    }

    /// <summary>
    /// Holt the parent <see cref="WochenplanWorkspaceViewModel"/> to which this WochenplanEintrag
    /// is added to.
    /// </summary>
    public List<TerminplanEintrag> TerminplanEinträge { get; private set; }

    /// <summary>
    /// Holt den 0-basierte Index des Wochentags für diesen Wochenplaneintrag
    /// </summary>
    public int WochentagIndex { get; private set; }

    /// <summary>
    /// Holt die erste Stunde dieser Terminserie.
    /// </summary>
    public int ErsteUnterrichtsstundeIndex { get; private set; }

    /// <summary>
    /// Holt die letzte Stunde dieser Terminserie
    /// </summary>
    public int LetzteUnterrichtsstundeIndex { get; private set; }

    /// <summary>
    /// Holt die Anzahl der Termine zur gleichen Zeit.
    /// </summary>
    public int AnzahlEinträge
    {
      get
      {
        return this.TerminplanEinträge.Count;
      }
    }

    /// <summary>
    /// Holt die Stundenanzahl des längsten Termins des Tages.
    /// </summary>
    public int MaxStundenAnzahl
    {
      get
      {
        var maxStunden = this.TerminplanEinträge.Max(o => o.Stundenanzahl);
        return maxStunden;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob diese Collection einen Dummy enthält.
    /// </summary>
    public bool IsDummy
    {
      get
      {
        return this.TerminplanEinträge.Any(o => o.IsDummy);
      }
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return string.Format(
        "TerminplanEintragCollection am Tag {0} von Stunde {1} bis {2}",
        this.WochentagIndex,
        this.ErsteUnterrichtsstundeIndex,
        this.LetzteUnterrichtsstundeIndex);
    }
  }
}
