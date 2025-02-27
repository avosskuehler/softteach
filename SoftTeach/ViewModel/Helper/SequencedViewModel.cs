﻿namespace SoftTeach.ViewModel.Helper
{
  using System;

  using SoftTeach.ViewModel.Curricula;

  /// <summary>
  /// ViewModel of an individual reihe
  /// </summary>
  public abstract class SequencedViewModel : ViewModelBase, ISequencedObject, IComparable
  {
    /// <summary>
    /// Holt oder setzt die Abfolgindex of this reihe
    /// </summary>
    public abstract int Reihenfolge { get; set; }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob die Reihenfolge Vorrang vor allen
    /// anderer Reihenfolgen der gleichen Zahl hat.
    /// </summary>
    public abstract bool IstZuerst { get; set; }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="viewModel">The object to be compared with this instance</param>
    /// <returns>Less than zero if This object is less than the other parameter. 
    /// Zero if This object is equal to other. Greater than zero if This object is greater than other.
    /// </returns>
    public int CompareTo(object viewModel)
    {
      var compareSequenz = viewModel as SequenzViewModel;
      if (compareSequenz == null)
      {
        return -1;
      }

      return this.Reihenfolge.CompareTo(compareSequenz.Reihenfolge);
    }
  }
}
