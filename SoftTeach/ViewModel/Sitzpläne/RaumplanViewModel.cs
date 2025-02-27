﻿namespace SoftTeach.ViewModel.Sitzpläne
{
  using System;
  using System.Collections.ObjectModel;
  using System.Collections.Specialized;
  using System.IO;
  using System.Linq;
  using System.Windows.Controls;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Shapes;
  using Microsoft.Win32;
  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.TeachyModel;
  using SoftTeach.Resources.Controls;
  using SoftTeach.UndoRedo;
  using SoftTeach.View.Sitzpläne;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual Raumplan
  /// </summary>
  public class RaumplanViewModel : ViewModelBase, IComparable
  {
    ///// <summary>
    ///// Der Grundriss des Raumplans
    ///// </summary>
    //private BitmapSource grundriss;

    /// <summary>
    /// The raum currently assigned to this Raumplan
    /// </summary>
    private RaumViewModel raum;

    /// <summary>
    /// Die momentan ausgewählte Sitzplatz
    /// </summary>
    private SitzplatzViewModel currentSitzplatz;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="RaumplanViewModel"/> Klasse. 
    /// </summary>
    /// <param name="raumplan">
    /// The underlying raumplan this ViewModel is to be based on
    /// </param>
    public RaumplanViewModel(Raumplan raumplan)
    {
      this.Model = raumplan ?? throw new ArgumentNullException(nameof(raumplan));

      this.OpenImageFileCommand = new DelegateCommand(this.OpenImageFile);
      this.EditRaumplanCommand = new DelegateCommand(this.EditRaumplan);

      // Build data structures for phasen
      this.Sitzplätze = new ObservableCollection<SitzplatzViewModel>();

      foreach (var sitzplatz in raumplan.Sitzplätze)
      {
        var vm = new SitzplatzViewModel(sitzplatz);
        //App.MainViewModel.Sitzplätze.Add(vm);
        this.Sitzplätze.Add(vm);
      }

      // Alte Version hatte keine Sitzplatzkennzeichnungen
      if (raumplan.Sitzplätze.All(o => o.Reihenfolge == 0))
      {
        SequencingService.SetCollectionSequence(this.Sitzplätze);
      }

      this.Sitzplätze.CollectionChanged += this.SitzplätzeCollectionChanged;
    }

    /// <summary>
    /// Holt den Befehl zum Laden einer Raumplandatei
    /// </summary>
    public DelegateCommand OpenImageFileCommand { get; private set; }

    /// <summary>
    /// Holt den Befehl zum Ändern des Raumplans
    /// </summary>
    public DelegateCommand EditRaumplanCommand { get; private set; }

    /// <summary>
    /// Holt den underlying Raumplan this ViewModel is based on
    /// </summary>
    public Raumplan Model { get; private set; }

    /// <summary>
    /// Holt oder setzt die Sitzplätze dieser Raumplan
    /// </summary>
    public ObservableCollection<SitzplatzViewModel> Sitzplätze { get; set; }

    /// <summary>
    /// Holt oder setzt die currently selected phase
    /// </summary>
    public SitzplatzViewModel CurrentSitzplatz
    {
      get
      {
        return this.currentSitzplatz;
      }

      set
      {
        this.currentSitzplatz = value;
        this.RaisePropertyChanged("Sitzplatz");
      }
    }

    /// <summary>
    /// Holt oder setzt die Bezeichnung
    /// </summary>
    public string RaumplanBezeichnung
    {
      get
      {
        return this.Model.Bezeichnung;
      }

      set
      {
        if (value == this.Model.Bezeichnung) return;
        this.UndoablePropertyChanging(this, nameof(RaumplanBezeichnung), this.Model.Bezeichnung, value);
        this.Model.Bezeichnung = value;
        this.RaisePropertyChanged("RaumplanBezeichnung");
      }
    }

    /// <summary>
    /// Holt oder setzt den Grundriss als Datei
    /// </summary>
    public byte[] RaumplanGrundriss
    {
      get
      {
        return this.Model.Grundriss;
      }

      set
      {
        if (value == this.Model.Grundriss) return;
        this.UndoablePropertyChanging(this, nameof(RaumplanGrundriss), this.Model.Grundriss, value);
        this.Model.Grundriss = value;
        this.RaisePropertyChanged("RaumplanGrundriss");
      }
    }

    /// <summary>
    /// Holt das Bild der Unterschrift als ImageSource
    /// </summary>
    [DependsUpon("RaumplanGrundriss")]
    public ImageSource RaumplanImage
    {
      get
      {
        if (this.RaumplanGrundriss == null || this.RaumplanGrundriss.Length == 0)
        {
          return null;
        }

        // Store binary data read from the database in a byte array
        MemoryStream stream = new MemoryStream();
        stream.Write(this.RaumplanGrundriss, 0, this.RaumplanGrundriss.Length);
        stream.Position = 0;

        System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
        var bi = new BitmapImage();
        bi.BeginInit();

        MemoryStream ms = new MemoryStream();
        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        ms.Seek(0, SeekOrigin.Begin);
        bi.StreamSource = ms;
        bi.EndInit();
        return bi;
      }
    }

    ///// <summary>
    ///// Holt oder setzt die Foto of this Person as a BitmapSource
    ///// </summary>
    //[DependsUpon("RaumplanGrundriss")]
    //public BitmapSource RaumplanImage
    //{
    //  get
    //  {
    //    if (this.RaumplanGrundriss != null && this.grundriss == null)
    //    {
    //      var bmp = new System.Drawing.Bitmap(new MemoryStream(this.RaumplanGrundriss));
    //      this.grundriss = ImageTools.CreateBitmapSourceFromBitmap(bmp);
    //    }

    //    return this.grundriss;
    //  }

    //  set
    //  {
    //    if (value != null)
    //    {
    //      var thumb = ImageTools.CreateBitmapFromBitmapImage(value);
    //      TypeConverter bitmapConverter = TypeDescriptor.GetConverter(thumb.GetType());
    //      this.RaumplanGrundriss = (byte[])bitmapConverter.ConvertTo(thumb, typeof(byte[]));
    //    }

    //    this.grundriss = value;
    //    this.RaisePropertyChanged("PersonBild");
    //  }
    //}

    /// <summary>
    /// Holt oder setzt den Raum für den Raumplan
    /// </summary>
    public RaumViewModel RaumplanRaum
    {
      get
      {
        // We need to reflect any changes made in the model so we check the current value before returning
        if (this.Model.Raum == null)
        {
          return null;
        }

        if (this.raum == null || this.raum.Model != this.Model.Raum)
        {
          this.raum = App.MainViewModel.Räume.SingleOrDefault(d => d.Model == this.Model.Raum);
        }

        return this.raum;
      }

      set
      {
        if (value == null) return;
        if (this.raum != null)
        {
          if (value.RaumBezeichnung == this.raum.RaumBezeichnung) return;
        }

        this.UndoablePropertyChanging(this, nameof(RaumplanRaum), this.raum, value);
        this.raum = value;
        this.Model.Raum = value.Model;
        this.RaisePropertyChanged("RaumplanRaum");
      }
    }

    /// <summary>
    /// Holt die Anzahl der verfügbaren Sitzplätze
    /// </summary>
    public int RaumplanAnzahlSitzplätze
    {
      get
      {
        return this.Sitzplätze.Count();
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherRaumplanViewModel = obj as RaumplanViewModel;
      if (otherRaumplanViewModel != null)
      {
        return StringLogicalComparer.Compare(this.RaumplanBezeichnung, otherRaumplanViewModel.RaumplanBezeichnung);
      }

      throw new ArgumentException("Object is not a RaumplanViewModel");
    }

    /// <summary>
    /// Gibt eine lesbare Repräsentation des ViewModels
    /// </summary>
    /// <returns>Ein <see cref="string"/> mit einer Kurzform des ViewModels.</returns>
    public override string ToString()
    {
      return "Raumplan: " + this.RaumplanBezeichnung;
    }

    /// <summary>
    /// Handles addition a new phase to this raumplan
    /// </summary>
    /// <param name="left"> The left. </param>
    /// <param name="top"> The top. </param>
    /// <param name="shape"> The shape. </param>
    public void AddSitzplatz(double left, double top, SitzplatzShape shape)
    {
      this.AddSitzplatz(left, top, shape.Width, shape.Height, ((RotateTransform)shape.RenderTransform).Angle);
    }

    /// <summary>
    /// Handles addition a new phase to this raumplan
    /// </summary>
    /// <param name="left"> The left. </param>
    /// <param name="top"> The top. </param>
    /// <param name="width"> The width. </param>
    /// <param name="height"> The height. </param>
    /// <param name="angle"> The angle. </param>
    public void AddSitzplatz(double left, double top, double width, double height, double angle)
    {
      var sitzplatz = new Sitzplatz
      {
        LinksObenX = left,
        LinksObenY = top,
        Breite = width,
        Höhe = height,
        Drehwinkel = angle,
        Raumplan = this.Model
      };
      var sitzplatzViewModel = new SitzplatzViewModel(sitzplatz);

      using (new UndoBatch(App.MainViewModel, string.Format("Neuer Sitzplatz {0} erstellt.", sitzplatzViewModel), false))
      {
        //App.MainViewModel.Sitzplätze.Add(sitzplatzViewModel);
        //App.UnitOfWork.Context.Sitzplätze.Add(sitzplatzViewModel.Model);
        this.Sitzplätze.Add(sitzplatzViewModel);
        this.CurrentSitzplatz = sitzplatzViewModel;
      }
    }

    /// <summary>
    /// Handles deletion of the given phase
    /// </summary>
    /// <param name="sitzplatzViewModel"> The sitzplatz View Model. </param>
    public void DeleteSitzplatz(SitzplatzViewModel sitzplatzViewModel)
    {
      using (new UndoBatch(App.MainViewModel, string.Format("Sitzplatz {0} gelöscht.", sitzplatzViewModel), false))
      {
        //App.UnitOfWork.Context.Sitzplätze.Remove(sitzplatzViewModel.Model);
        var result = this.Sitzplätze.RemoveTest(sitzplatzViewModel);
      }
    }

    /// <summary>
    /// Ruft den Raumpläne Dialog zur Beraumplanung auf
    /// </summary>
    private void EditRaumplan()
    {
      bool undo;
      using (new UndoBatch(App.MainViewModel, string.Format("Raumplan {0} geändert.", this), false))
      {
        var dlg = new EditRaumplanDialog(this);
        undo = !dlg.ShowDialog().GetValueOrDefault(false);
      }

      if (undo)
      {
        App.MainViewModel.ExecuteUndoCommand();
      }
    }

    /// <summary>
    /// Tritt auf, wenn die ModelCollection verändert wurde.
    /// Gibt die Änderungen an den Undostack weiter.
    /// </summary>
    /// <param name="sender">Die auslösende Collection</param>
    /// <param name="e">Die NotifyCollectionChangedEventArgs mit den Infos.</param>
    private void SitzplätzeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UndoableCollectionChanged(this, nameof(Sitzplätze), this.Sitzplätze, e, true, "Änderung der Sitzplätze");
    }

    /// <summary>
    /// Speichert die Datei in das Bildfeld der Datenbankentity
    /// </summary>
    public string Bilddatei
    {
      set
      {
        if (!File.Exists(value))
        {
          return;
        }

        //Initialize a file stream to read the image file
        using (var fs = new FileStream(value, FileMode.Open, FileAccess.Read))
        {
          //Initialize a byte array with size of stream
          byte[] imgByteArr = new byte[fs.Length];

          //Read data from the file stream and put into the byte array
          fs.Read(imgByteArr, 0, Convert.ToInt32(fs.Length));

          this.RaumplanGrundriss = imgByteArr;
        }
      }
    }

    /// <summary>
    /// Öffnet eine Raumplanbilddatei
    /// </summary>
    private void OpenImageFile()
    {
      var dlg = new OpenFileDialog
      {
        CheckFileExists = true,
        CheckPathExists = true,
        Title = "Bitte Raumplangrundriss auswählen",
        Filter = "PNG-Dateien (*.png)|*.png",
        Multiselect = false
      };
      if (dlg.ShowDialog().GetValueOrDefault(false))
      {
        if (File.Exists(dlg.FileName))
        {
          using (var stream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            var bitmapFrame = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            var width = bitmapFrame.PixelWidth;
            var height = bitmapFrame.PixelHeight;
            if (width != 800 || height != 500)
            {
              InformationDialog.Show("Bildgröße nicht korrekt", "Bitte nur Scans mit 800x500 Pixeln verwenden", false);
              return;
            }
          }

          this.Bilddatei = dlg.FileName;
        }
      }
    }
  }
}