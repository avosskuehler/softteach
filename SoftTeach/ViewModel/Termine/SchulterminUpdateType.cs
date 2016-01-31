// -----------------------------------------------------------------------
// <copyright file="TerminUpdateType.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SoftTeach.ViewModel.Termine
{
  /// <summary>
  /// This enumeration list the possibilities for termin updates
  /// </summary>
  public enum SchulterminUpdateType
  {
    /// <summary>
    /// The termin was newly added.
    /// </summary>
    Added,

    /// <summary>
    /// The termin was deleted.
    /// </summary>
    Removed,

    /// <summary>
    /// The termin properties were changed, but not its date.
    /// </summary>
    Changed,

    /// <summary>
    /// The bschreibung property changed
    /// </summary>
    ChangedBeschreibung,

    /// <summary>
    /// The termin properties were changed including its date.
    /// </summary>
    ChangedWithNewDay,

    /// <summary>
    /// The betroffene klasse collection of this termin changed.
    /// </summary>
    BetroffeneKlasseChanged,
  }

  /// <summary>
  /// Contains information about a modified SchulterminViewModel
  /// </summary>
  public struct ModifiedTermin
  {
    /// <summary>
    /// The <see cref="SchulterminViewModel"/> that describes the new termin.
    /// </summary>
    public SchulterminViewModel SchulterminViewModel;

    /// <summary>
    /// The <see cref="SchulterminUpdateType"/> for this termin.
    /// </summary>
    public SchulterminUpdateType SchulterminUpdateType;

    /// <summary>
    /// If the <see cref="SchulterminUpdateType"/> is ChangedWithNewDay this
    /// contains the old termin, otherwise it is null.
    /// </summary>
    public object Parameter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifiedTermin"/> struct.
    /// </summary>
    /// <param name="schulterminViewModel">The <see cref="SchulterminViewModel"/> that describes the new termin.</param>
    /// <param name="schulterminUpdateType">The <see cref="SchulterminUpdateType"/> for this termin.</param>
    /// <param name="parameter"> If the <see cref="SchulterminUpdateType"/> is ChangedWithNewDay this
    /// contains the old termin, if it is BetroffeneKlasseChanged it contains
    /// the list of changes, otherwise it is null.</param>
    public ModifiedTermin(SchulterminViewModel schulterminViewModel, SchulterminUpdateType schulterminUpdateType, object parameter)
    {
      this.SchulterminViewModel = schulterminViewModel;
      this.SchulterminUpdateType = schulterminUpdateType;
      this.Parameter = parameter;
    }
  }
}
