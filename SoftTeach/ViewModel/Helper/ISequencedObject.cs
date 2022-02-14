namespace SoftTeach.ViewModel.Helper
{
  /// <summary>
  /// An object that can be given a sequential order in a collection.
  /// </summary>
  public interface ISequencedObject
  {
    /// <summary>
    /// The sequence number of the object
    /// </summary>
    int Reihenfolge { get; set; }
  }
}