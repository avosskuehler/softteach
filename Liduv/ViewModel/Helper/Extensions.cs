namespace Liduv.ViewModel.Helper
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows.Forms.VisualStyles;

  public static class Extensions
  {
    public static bool RemoveTest<T>(this ICollection<T> source, T viewModel)
    {
      var success = source.Remove(viewModel);
      if (!success)
      {
        Console.WriteLine("Remove failed for collection {0} and object {1}", source.GetType(), viewModel);
      }

      return success;
    }

    public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, int count)
    {
      var array = source.ToArray();
      return ShuffleInternal(array, Math.Min(count, array.Length)).Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
      var array = source.ToArray();
      var count = array.Length;
      return ShuffleInternal(array, Math.Min(count, array.Length)).Take(count);
    }

    private static IEnumerable<T> ShuffleInternal<T>(T[] array, int count)
    {
      var random = new Random();
      for (var n = 0; n < count; n++)
      {
        var k = random.Next(n, array.Length);
        var temp = array[n];
        array[n] = array[k];
        array[k] = temp;
      }

      return array;
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
    {
      var i = 0;
      var splits = from item in list
                   group item by i++ % parts into part
                   select part.AsEnumerable();
      return splits;
    }

    public static void Each<T>(this IEnumerable<T> items, Action<T> action)
    {
      foreach (var item in items)
        action(item);
    }

    public static string StripLeft(this string value, int length)
    {
      return value.Substring(length, value.Length - length);
    }

    public static string StripRight(this string value, int length)
    {
      return value.Substring(value.Length - length, length);
    }

    public static void Raise(this PropertyChangedEventHandler eventHandler, object source, string propertyName)
    {
      var handlers = eventHandler;
      if (handlers != null)
        handlers(source, new PropertyChangedEventArgs(propertyName));
    }

    public static void Raise(this EventHandler eventHandler, object source)
    {
      var handlers = eventHandler;
      if (handlers != null)
        handlers(source, EventArgs.Empty);
    }

    public static void Register(this INotifyPropertyChanged model, string propertyName, Action whenChanged)
    {
      model.PropertyChanged += (sender, args) =>
      {
        if (args.PropertyName == propertyName)
          whenChanged();
      };
    }

    public static void BubbleSort(this IList o)
    {
      for (int i = o.Count - 1; i >= 0; i--)
      {
        for (int j = 1; j <= i; j++)
        {
          object o1 = o[j - 1];
          object o2 = o[j];
          if (((IComparable)o1).CompareTo(o2) > 0)
          {
            o.Remove(o1);
            o.Insert(j, o1);
          }
        }
      }
    }
  }
}
