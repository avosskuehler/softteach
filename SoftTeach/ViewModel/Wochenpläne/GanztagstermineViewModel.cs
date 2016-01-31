// -----------------------------------------------------------------------
// <copyright file="TerminUpdateType.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SoftTeach.ViewModel.Wochenpläne
{
  using System;
  using System.Collections.Generic;

  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// Repräsentiert Ganztagstermine wir Geburtstage, Abitur, etc.
  /// </summary>
  public class GanztagstermineViewModel : ViewModelBase
  {
    /// <summary>
    /// Der 0-basierte Index des Wochentags dieses Ganztagstermins.
    /// </summary>
    private int wochentagIndex;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="GanztagstermineViewModel"/> Klasse. 
    /// </summary>
    /// <param name="ganztagsterminEinträge"> The ganztagstermin Einträge. </param>
    public GanztagstermineViewModel(List<TerminplanEintrag> ganztagsterminEinträge)
    {
      this.GanztagsterminEinträge = ganztagsterminEinträge;
    }

    /// <summary>
    /// Holt die <see cref="TerminplanEintrag"/> Einträge, für die ggf.
    /// Ganztagstermine angelegt werden sollen
    /// </summary>
    public List<TerminplanEintrag> GanztagsterminEinträge { get; private set; }

    /// <summary>
    /// Holt einen Wert, der angibt, ob dieser Ganztagstermin ist ein Dummy, weil ohne Einträge
    /// </summary>
    public bool IsDummy
    {
      get
      {
        return this.GanztagsterminEinträge == null || this.GanztagsterminEinträge.Count == 0;
      }
    }

    /// <summary>
    /// Holt oder setzt den Wochentagindex für diesen Ganztagstermin.
    /// </summary>
    public int WochentagIndex
    {
      get
      {
        if (this.GanztagsterminEinträge != null && this.GanztagsterminEinträge.Count > 0)
        {
          var firstTermin = this.GanztagsterminEinträge[0];
          return firstTermin.WochentagIndex;
        }

        return this.wochentagIndex;
      }

      set
      {
        if (!this.IsDummy)
        {
          throw new ArgumentOutOfRangeException("value", "Der Wochentagindex kann nicht zugewiesen werden.");
        }

        this.wochentagIndex = value;
      }
    }
  }
}
