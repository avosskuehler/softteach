namespace SoftTeach.ViewModel.Curricula
{
  using System.Collections.Generic;

  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ZeitstrahlCollection
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
