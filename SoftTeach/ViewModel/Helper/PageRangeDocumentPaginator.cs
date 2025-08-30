namespace SoftTeach.ViewModel.Helper
{
  using System;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Media;

  /// <summary>
  /// Encapsulates a DocumentPaginator and allows
  /// to paginate just some specific pages (a "PageRange")
  /// of the encapsulated DocumentPaginator
  ///  (c) Thomas Claudius Huber 2010 
  ///      http://www.thomasclaudiushuber.com
  /// </summary>

  public class PageRangeDocumentPaginator : DocumentPaginator
  {
    private readonly int _startIndex;
    private readonly int _endIndex;
    private readonly DocumentPaginator _paginator;
    public PageRangeDocumentPaginator(DocumentPaginator paginator, PageRange pageRange)
    {
      this._startIndex = pageRange.PageFrom - 1;
      this._endIndex = pageRange.PageTo - 1;
      this._paginator = paginator;

      // Adjust the _endIndex
      this._endIndex = Math.Min(this._endIndex, this._paginator.PageCount - 1);
    }

    public override DocumentPage GetPage(int pageNumber)
    {
      var page = this._paginator.GetPage(pageNumber + this._startIndex);

      // Create a new ContainerVisual as a new parent for page children
      var cv = new ContainerVisual();
      if (page.Visual is FixedPage)
      {
        foreach (var child in ((FixedPage)page.Visual).Children)
        {
          // Make a shallow clone of the child using reflection
          var childClone = (UIElement)child.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(child, null);

          // Setting the parent of the cloned child to the created ContainerVisual by using Reflection.
          // WARNING: If we use Add and Remove methods on the FixedPage.Children, for some reason it will
          //          throw an exception concerning event handlers after the printing job has finished.
          var parentField = childClone.GetType().GetField("_parent", BindingFlags.Instance | BindingFlags.NonPublic);
          if (parentField != null)
          {
            parentField.SetValue(childClone, null);
            cv.Children.Add(childClone);
          }
        }

        return new DocumentPage(cv, page.Size, page.BleedBox, page.ContentBox);
      }

      return page;
    }

    public override bool IsPageCountValid => true;

    public override int PageCount
    {
      get
      {
        if (this._startIndex > this._paginator.PageCount - 1)
          return 0;
        if (this._startIndex > this._endIndex)
          return 0;

        return this._endIndex - this._startIndex + 1;
      }
    }

    public override Size PageSize
    {
      get => this._paginator.PageSize;
      set => this._paginator.PageSize = value;
    }

    public override IDocumentPaginatorSource Source => this._paginator.Source;
  }
}
