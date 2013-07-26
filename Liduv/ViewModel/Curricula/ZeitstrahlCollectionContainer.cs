// -----------------------------------------------------------------------
// <copyright file="ZeitstrahlCollectionContainer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Liduv.ViewModel.Curricula
{
  using System.Collections.Generic;

  using Liduv.ViewModel.Helper;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class ZeitstrahlCollectionContainer
  {
    public IEnumerable<ViewModelBase> Collection { get; private set; }

    public string GroupingDescription { get; private set; }

    public ZeitstrahlCollectionContainer(IEnumerable<ViewModelBase> collection, string groupingDescription)
    {
      this.Collection = collection;
      this.GroupingDescription = groupingDescription;
    }
  }
}
