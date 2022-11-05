namespace SoftTeach.ViewModel.Helper
{
  using SoftTeach.ViewModel.Curricula;
  using System.Collections.ObjectModel;
  using System.Linq;

  public static class SequencingService
  {
    /// <summary>
    /// Resets the sequential order of a collection.
    /// </summary>
    /// <param name="targetCollection">The collection to be re-indexed.</param>
    public static ObservableCollection<T> SetCollectionSequence<T>(ObservableCollection<T> targetCollection) where T : class, ISequencedObject
    {
      // Initialize
      var sequenceNumber = 1;

      // Resequence
      var collection = targetCollection;
      foreach (var sequencedObject in collection)
      {
        sequencedObject.Reihenfolge = sequenceNumber;
        sequencedObject.IstZuerst = false;
        sequenceNumber++;
      }

      // Set return value
      return targetCollection;
    }

    public static ObservableCollection<T> NeuAnordnen<T>(ObservableCollection<T> targetCollection) where T : class, ISequencedObject
    {
      // Initialize
      var sequenceNumber = 1;

      // Resequence
      var collection = targetCollection.Where(o => o.Reihenfolge != -1).OrderBy(o => o.Reihenfolge).ThenBy(o => o.IstZuerst);
      foreach (var sequencedObject in collection)
      {
        sequencedObject.Reihenfolge = sequenceNumber;
        sequencedObject.IstZuerst = false;
        sequenceNumber++;
      }

      // Set return value
      return targetCollection;
    }
  }
}