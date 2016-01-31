namespace SoftTeach.ViewModel.Personen
{
  using System;
  using System.ComponentModel;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media.Imaging;

  using GongSolutions.Wpf.DragDrop;

  using SoftTeach.ExceptionHandling;
  using SoftTeach.Model.EntityFramework;
  using SoftTeach.Resources.Controls;
  using SoftTeach.ViewModel.Helper;

  /// <summary>
  /// ViewModel of an individual person
  /// </summary>
  public class PersonViewModel : ViewModelBase, IComparable, IDropTarget
  {
    /// <summary>
    /// Das Bild der Person
    /// </summary>
    private BitmapSource bild;

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="PersonViewModel"/> Klasse. 
    /// </summary>
    public PersonViewModel()
    {
      this.DeletePersonCommand = new DelegateCommand(this.DeletePerson);
      var person = new Person();
      this.Model = person;
      App.MainViewModel.Personen.Add(this);
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="PersonViewModel"/> Klasse. 
    /// </summary>
    /// <param name="person">
    /// The underlying person this ViewModel is to be based on
    /// </param>
    public PersonViewModel(Person person)
    {
      if (person == null)
      {
        throw new ArgumentNullException("person");
      }

      this.Model = person;
      this.DeletePersonCommand = new DelegateCommand(this.DeletePerson);
    }

    /// <summary>
    /// Holt den Befehl zur deleting this person
    /// </summary>
    public DelegateCommand DeletePersonCommand { get; private set; }

    /// <summary>
    /// Holt oder setzt die underlying Person this ViewModel is based on
    /// </summary>
    public Person Model { get; protected set; }

    /// <summary>
    /// Holt oder setzt die Vorname
    /// </summary>
    public string PersonVorname
    {
      get
      {
        return this.Model.Vorname;
      }

      set
      {
        if (value == this.Model.Vorname) return;
        this.UndoablePropertyChanging(this, "PersonVorname", this.Model.Vorname, value);
        this.Model.Vorname = value;
        this.RaisePropertyChanged("PersonVorname");
      }
    }

    /// <summary>
    /// Holt den Vornamen in Kurzform
    /// </summary>
    public string PersonVornameKurz
    {
      get
      {
        var split = this.PersonVorname.Split(' ');
        var split2 = split[0].Split('-');
        if (split2.Count() > 1)
        {
          var posBindestrich = this.PersonVorname.IndexOf('-');
          return this.PersonVorname.Substring(0, posBindestrich + 2);
        }

        return split[0];
      }
    }

    /// <summary>
    /// Holt oder setzt die Nachname
    /// </summary>
    public string PersonNachname
    {
      get
      {
        return this.Model.Nachname;
      }

      set
      {
        if (value == this.Model.Nachname) return;
        this.UndoablePropertyChanging(this, "PersonNachname", this.Model.Nachname, value);
        this.Model.Nachname = value;
        this.RaisePropertyChanged("PersonNachname");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob die Person weiblich ist
    /// </summary>
    public bool PersonIstWeiblich
    {
      get
      {
        return this.Model.Geschlecht;
      }

      set
      {
        if (value == this.Model.Geschlecht) return;
        this.UndoablePropertyChanging(this, "PersonIstWeiblich", this.Model.Geschlecht, value);
        this.Model.Geschlecht = value;
        this.RaisePropertyChanged("PersonIstWeiblich");
      }
    }

    ///// <summary>
    ///// Holt oder setzt einen Wert, der angibt, ob die Person männlich ist
    ///// </summary>
    //[DependsUpon("PersonIstWeiblich")]
    //public bool PersonIstMännlich
    //{
    //  get
    //  {
    //    return !this.PersonIstWeiblich;
    //  }

    //  set
    //  {
    //    this.PersonIstWeiblich = !value;
    //  }
    //}

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob diese Person ein Lehrer ist.
    /// </summary>
    public bool PersonIstLehrer
    {
      get
      {
        return this.Model.IstLehrer;
      }

      set
      {
        if (value == this.Model.IstLehrer) return;
        this.UndoablePropertyChanging(this, "PersonIstLehrer", this.Model.IstLehrer, value);
        this.Model.IstLehrer = value;
        this.RaisePropertyChanged("PersonIstLehrer");
      }
    }

    /// <summary>
    /// Holt oder setzt einen Wert, der angibt, ob diese Person ein Schüler ist.
    /// </summary>
    [DependsUpon("PersonIstLehrer")]
    public bool PersonIstSchüler
    {
      get
      {
        return !this.PersonIstLehrer;
      }

      set
      {
        this.PersonIstLehrer = !value;
      }
    }

    /// <summary>
    /// Holt oder setzt die Geburtstag
    /// </summary>
    public DateTime? PersonGeburtstag
    {
      get
      {
        return this.Model.Geburtstag;
      }

      set
      {
        if (value == this.Model.Geburtstag) return;
        this.UndoablePropertyChanging(this, "PersonGeburtstag", this.Model.Geburtstag, value);
        this.Model.Geburtstag = value;
        this.RaisePropertyChanged("PersonGeburtstag");
      }
    }

    /// <summary>
    /// Holt den Jahrgang of this Person
    /// </summary>
    [DependsUpon("PersonGeburtstag")]
    public int? PersonJahrgang
    {
      get
      {
        if (this.Model.Geburtstag.HasValue)
        {
          return this.Model.Geburtstag.Value.Year;
        }

        return null;
      }
    }

    /// <summary>
    /// Holt oder setzt die Titel
    /// </summary>
    public string PersonTitel
    {
      get
      {
        return this.Model.Titel;
      }

      set
      {
        if (value == this.Model.Titel) return;
        this.UndoablePropertyChanging(this, "PersonTitel", this.Model.Titel, value);
        this.Model.Titel = value;
        this.RaisePropertyChanged("PersonTitel");
      }
    }

    /// <summary>
    /// Holt oder setzt die Telefon
    /// </summary>
    public string PersonTelefon
    {
      get
      {
        return this.Model.Telefon;
      }

      set
      {
        if (value == this.Model.Telefon) return;
        this.UndoablePropertyChanging(this, "PersonTelefon", this.Model.Telefon, value);
        this.Model.Telefon = value;
        this.RaisePropertyChanged("PersonTelefon");
      }
    }


    /// <summary>
    /// Holt oder setzt die Fax
    /// </summary>
    public string PersonFax
    {
      get
      {
        return this.Model.Fax;
      }

      set
      {
        if (value == this.Model.Fax) return;
        this.UndoablePropertyChanging(this, "PersonFax", this.Model.Fax, value);
        this.Model.Fax = value;
        this.RaisePropertyChanged("PersonFax");
      }
    }

    /// <summary>
    /// Holt oder setzt die Handy
    /// </summary>
    public string PersonHandy
    {
      get
      {
        return this.Model.Handy;
      }

      set
      {
        if (value == this.Model.Handy) return;
        this.UndoablePropertyChanging(this, "PersonHandy", this.Model.Handy, value);
        this.Model.Handy = value;
        this.RaisePropertyChanged("PersonHandy");
      }
    }

    /// <summary>
    /// Holt oder setzt die EMail
    /// </summary>
    public string PersonEMail
    {
      get
      {
        return this.Model.EMail;
      }

      set
      {
        if (value == this.Model.EMail) return;
        this.UndoablePropertyChanging(this, "PersonEMail", this.Model.EMail, value);
        this.Model.EMail = value;
        this.RaisePropertyChanged("PersonEMail");
      }
    }

    /// <summary>
    /// Holt oder setzt die PLZ
    /// </summary>
    public string PersonPLZ
    {
      get
      {
        return this.Model.PLZ;
      }

      set
      {
        if (value == this.Model.PLZ) return;
        this.UndoablePropertyChanging(this, "PersonPLZ", this.Model.PLZ, value);
        this.Model.PLZ = value;
        this.RaisePropertyChanged("PersonPLZ");
      }
    }


    /// <summary>
    /// Holt oder setzt die Straße
    /// </summary>
    public string PersonStraße
    {
      get
      {
        return this.Model.Straße;
      }

      set
      {
        if (value == this.Model.Straße) return;
        this.UndoablePropertyChanging(this, "PersonStraße", this.Model.Straße, value);
        this.Model.Straße = value;
        this.RaisePropertyChanged("PersonStraße");
      }
    }

    /// <summary>
    /// Holt oder setzt die Hausnummer
    /// </summary>
    public string PersonHausnummer
    {
      get
      {
        return this.Model.Hausnummer;
      }

      set
      {
        if (value == this.Model.Hausnummer) return;
        this.UndoablePropertyChanging(this, "PersonHausnummer", this.Model.Hausnummer, value);
        this.Model.Hausnummer = value;
        this.RaisePropertyChanged("PersonHausnummer");
      }
    }

    /// <summary>
    /// Holt oder setzt die Ort
    /// </summary>
    public string PersonOrt
    {
      get
      {
        return this.Model.Ort;
      }

      set
      {
        if (value == this.Model.Ort) return;
        this.UndoablePropertyChanging(this, "PersonOrt", this.Model.Ort, value);
        this.Model.Ort = value;
        this.RaisePropertyChanged("PersonOrt");
      }
    }

    /// <summary>
    /// Holt oder setzt die Foto
    /// </summary>
    public byte[] PersonFoto
    {
      get
      {
        return this.Model.Foto;
      }

      set
      {
        if (value == this.Model.Foto) return;
        this.UndoablePropertyChanging(this, "PersonFoto", this.Model.Foto, value);
        this.Model.Foto = value;
        this.RaisePropertyChanged("PersonFoto");
      }
    }

    /// <summary>
    /// Holt oder setzt die Gruppennummer
    /// </summary>
    public int Gruppennummer { get; set; }

    /// <summary>
    /// Holt oder setzt die Foto of this Person as a BitmapSource
    /// </summary>
    [DependsUpon("PersonFoto")]
    public BitmapSource PersonBild
    {
      get
      {
        if (this.PersonFoto != null && this.bild == null)
        {
          var bmp = new System.Drawing.Bitmap(new MemoryStream(this.PersonFoto));
          this.bild = ImageTools.CreateBitmapSourceFromBitmap(bmp);
        }

        return this.bild;
      }

      set
      {
        if (value != null)
        {
          var thumb = ImageTools.CreateBitmapFromBitmapImage(value);
          TypeConverter bitmapConverter = TypeDescriptor.GetConverter(thumb.GetType());
          this.PersonFoto = (byte[])bitmapConverter.ConvertTo(thumb, typeof(byte[]));
        }

        this.bild = value;
        this.RaisePropertyChanged("PersonBild");
      }
    }

    /// <summary>
    /// Holt eine Kurzbeschreibung der Person
    /// </summary>
    [DependsUpon("PersonVorname")]
    [DependsUpon("PersonNachname")]
    [DependsUpon("PersonGeburtstag")]
    [DependsUpon("PersonIstSchüler")]
    [DependsUpon("PersonIstWeiblich")]
    public string PersonKurzinfo
    {
      get
      {
        var info = new StringBuilder();

        if (string.IsNullOrEmpty(this.Model.Nachname))
        {
          return string.Empty;
        }

        info.Append(this.PersonVorname);
        info.Append(" ");
        info.Append(this.PersonNachname);
        var dateTime = this.PersonGeburtstag;
        if (dateTime != null)
        {
          info.Append(", Jahrgang ");
          info.Append(dateTime.Value.Year.ToString(CultureInfo.InvariantCulture));
        }

        info.Append(this.PersonIstSchüler ? ", Schüler" : ", Lehrer");

        if (this.PersonIstWeiblich)
        {
          info.Append("in");
        }

        return info.ToString();
      }
    }

    /// <summary>
    /// Holt eine Kurzbeschreibung der Person
    /// </summary>
    [DependsUpon("PersonVorname")]
    [DependsUpon("PersonNachname")]
    public string PersonKurzform
    {
      get
      {
        var info = new StringBuilder();

        if (string.IsNullOrEmpty(this.Model.Nachname))
        {
          return string.Empty;
        }

        info.Append(this.PersonVorname);
        info.Append(" ");
        info.Append(this.PersonNachname.Substring(0, 1));
        info.Append(".");

        return info.ToString();
      }
    }

    /// <summary>
    /// Gets the age of eg. a person from its birthday.
    /// </summary>
    /// <param name="wochentag">
    /// The wochentag.
    /// </param>
    /// <param name="birthday">
    /// The birthday.
    /// </param>
    /// <returns>
    /// The age in years.
    /// </returns>
    public static int GetAgeFromDate(DateTime wochentag, DateTime birthday)
    {
      var years = wochentag.Year - birthday.Year;
      var birthdayToday = birthday.AddYears(years);
      if (wochentag.CompareTo(birthdayToday) < 0)
      {
        years--;
      }

      return years;
    }

    /// <summary>
    /// Holt das aktuelle Alter der Person in Jahren
    /// </summary>
    /// <param name="wochentag"> The Tag, an dem das Alter der Person festgestellt werden soll.</param>
    /// <returns>
    /// Das Alter das Person am gegebenen Tag erreicht als <see cref="int"/>.
    /// </returns>
    public int PersonAlter(DateTime wochentag)
    {
      return this.PersonGeburtstag.HasValue ? GetAgeFromDate(wochentag, this.PersonGeburtstag.Value) : 0;
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="obj">An object to compare with this object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared. </returns>
    public int CompareTo(object obj)
    {
      var otherPersonViewModel = obj as PersonViewModel;
      if (otherPersonViewModel != null)
      {
        return StringLogicalComparer.Compare(this.PersonVorname, otherPersonViewModel.PersonVorname);
      }

      throw new ArgumentException("Object is not a PersonViewModel");
    }

    public override string ToString()
    {
      return this.PersonKurzinfo;
    }

    public void SavePersonImage(string filename)
    {
      var image = new BitmapImage();

      image.BeginInit();
      image.UriSource = new Uri(filename);
      image.DecodePixelWidth = 80;
      image.EndInit();
      this.PersonBild = image;
    }

    // TODO Check this
    //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    //{
    //  base.OnPropertyChanged(e);
    //  this.CheckForDuplicate();
    //}

    public void DragOver(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data;
      var dataObject = sourceItem as DataObject;
      if (dataObject != null)
      {
        var dataFormats = dataObject.GetFormats();
        var validSource = dataFormats != null && (dataFormats.Contains("FileName") || dataFormats.Contains("Bitmap") || dataFormats.Contains("System.Windows.Media.Imaging.BitmapSource"));

        var targetItem = dropInfo.TargetItem;
        if (validSource)
        {
          dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
          dropInfo.Effects = DragDropEffects.Copy;
        }

        if (dataFormats.Contains("OwnBitmapSource"))
        {
          dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
          dropInfo.Effects = DragDropEffects.Move;
        }
      }
    }

    public void Drop(IDropInfo dropInfo)
    {
      var sourceItem = dropInfo.Data;
      var dataObject = sourceItem as DataObject;
      if (dataObject == null)
      {
        return;
      }

      if (dataObject.GetDataPresent("System.Drawing.Bitmap"))
      {
        var data = dataObject.GetData("System.Drawing.Bitmap");
        var bmp = data as System.Drawing.Bitmap;
        this.PersonBild = ImageTools.CreateBitmapSourceFromBitmap(ImageTools.ScaleImage(bmp, 80, 100));
      }
      else if (dataObject.GetDataPresent("FileName"))
      {
        var data = dataObject.GetData("FileName");
        var filenames = data as string[];
        if (filenames != null)
        {
          var filename = filenames[0];
          this.SavePersonImage(filename);
        }
      }
      else if (dataObject.GetDataPresent("OwnBitmapSource"))
      {
        // we wan´t to get rid of the foto
        this.PersonBild = null;
        this.PersonFoto = null;
      }
    }

    /// <summary>
    /// Handles deletion of the current person
    /// </summary>
    private void DeletePerson()
    {
    }

    private void CheckForDuplicate()
    {
      var count = App.MainViewModel.Personen.Count(o => o.PersonKurzinfo == this.PersonKurzinfo);
      if (count > 1)
      {
        InformationDialog.Show(
          "Diese Person existiert bereits",
          "Die Person, die sie gerade anlegen existiert bereits" + " in der Datenbank, bitte kontrollieren Sie Name,"
          + " Vorname und Geburtstag.",
          false);
      }
    }
  }
}
