namespace Liduv.ViewModel.Helper
{
  using System.Collections.ObjectModel;

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
      foreach (var sequencedObject in targetCollection)
      {
        sequencedObject.AbfolgeIndex = sequenceNumber;
        sequenceNumber++;
      }

      // Set return value
      return targetCollection;
    }
  }
}