namespace Liduv.ViewModel.Wochenpläne
{
  using System.Collections.Generic;
  using System.Linq;

  using Liduv.ViewModel.Helper;

  /// <summary>
  /// Für jeden Termin der Woche (Unterrichtsstunde, AG, Treffen) wird ein Wochenplaneintrag erstellt.
  /// Er erscheint im Wochenplan.
  /// </summary>
  public class WochenplanEintragCollection : ViewModelBase
  {
    /// <summary>
    /// Die erste Stunde dieses Termins.
    /// </summary>
    private int ersteUnterrichtsstundeIndex;

    /// <summary>
    /// Die letzte Stunde dieses Termins.
    /// </summary>
    private int letzteUnterrichtsstundeIndex;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="WochenplanEintragCollection"/> Klasse. 
    /// </summary>
    /// <param name="wochentagIndex"> The wochentag Index. </param>
    /// <param name="einträge">Die Liste der Wochenplaneinträge.</param>
    public WochenplanEintragCollection(int wochentagIndex, List<WochenplanEintrag> einträge)
    {
      this.WochentagIndex = wochentagIndex;
      this.WochenplanEinträge = einträge;
    }

    /// <summary>
    /// Holt the parent <see cref="WochenplanWorkspaceViewModel"/> to which this WochenplanEintrag
    /// is added to.
    /// </summary>
    public List<WochenplanEintrag> WochenplanEinträge { get; private set; }

    /// <summary>
    /// Der 0-basierte Index des Wochentags für diesen Wochenplaneintrag
    /// </summary>
    public int WochentagIndex { get; set; }

    /// <summary>
    /// Holt einen Wert, der angibt, ob dieser Wochenplaneintrag leer ist (ohne Termine).
    /// </summary>
    public int AnzahlEinträge
    {
      get
      {
        return this.WochenplanEinträge.Count;
      }
    }

    /// <summary>
    /// Holt die Stundenanzahl des längsten Termins des Tages.
    /// </summary>
    public int MaxStundenAnzahl
    {
      get
      {
        var maxStunden = this.WochenplanEinträge.Max(o => o.Stundenanzahl);
        return maxStunden;
      }
    }

    /// <summary>
    /// Holt einen Wert, der angibt, ob dieser Wochenplaneintrag leer ist (ohne Termine).
    /// </summary>
    public bool IsDummy
    {
      get
      {
        return this.WochenplanEinträge.Count == 0;
      }
    }
    ///// <summary>
    ///// Holt oder setzt den Index für die erste Stunde dieses Termins.
    ///// </summary>
    //public int ErsteUnterrichtsstundeIndex
    //{
    //  get
    //  {
    //    if (this.TerminViewModel != null)
    //    {
    //      this.ersteUnterrichtsstundeIndex = this.TerminViewModel.TerminErsteUnterrichtsstunde.UnterrichtsstundeIndex;
    //    }

    //    return this.ersteUnterrichtsstundeIndex;
    //  }

    //  set
    //  {
    //    if (!this.IsDummy)
    //    {
    //      throw new ArgumentOutOfRangeException("value", "Der StundenplaneintragErsteUnterrichtsstundeIndex kann nicht zugewiesen werden.");
    //    }

    //    this.ersteUnterrichtsstundeIndex = value;
    //    this.RaisePropertyChanged("ErsteUnterrichtsstundeIndex");
    //  }
    //}

    ///// <summary>
    ///// Holt oder setzt den index der letzten Unterrichtstunde des Wochenplaneintrags
    ///// </summary>
    //public int LetzteUnterrichtsstundeIndex
    //{
    //  get
    //  {
    //    if (this.TerminViewModel != null)
    //    {
    //      this.letzteUnterrichtsstundeIndex = this.TerminViewModel.TerminLetzteUnterrichtsstunde.UnterrichtsstundeIndex;
    //    }

    //    return this.letzteUnterrichtsstundeIndex;
    //  }

    //  set
    //  {
    //    if (!this.IsDummy)
    //    {
    //      throw new ArgumentOutOfRangeException("value", "Der StundenplaneintragLetzteUnterrichtsstundeIndex kann nicht zugewiesen werden.");
    //    }

    //    this.letzteUnterrichtsstundeIndex = value;
    //    this.RaisePropertyChanged("ErsteUnterrichtsstundeIndex");
    //  }
    //}

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return string.Format("WochenplanEintragCollection am Tag {0}", this.WochentagIndex);
    }
  }
}
