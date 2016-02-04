// <copyright file="Configuration.cs" company="Paul Natorp Gymnasium, Berlin">        
// LEUDA - Lehrerunterrichtsdatenbank
// Copyright (C) 2013 Dr. Adrian Voßkühler
// -----------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published  
// by the Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version. This program is distributed in the 
// hope that it will be useful, but WITHOUT ANY WARRANTY; without 
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A
// PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// ***********************************************************************
// </copyright>
// <author>Adrian Voßkühler</author>
// <email>adrian@vosskuehler.name</email>

namespace SoftTeach.Setting
{
  using System;
  using System.ComponentModel;
  using System.IO;
  using System.Runtime.Serialization;
  using System.Windows;
  using System.Windows.Navigation;

  using MahApps.Metro.Controls;

  using SoftTeach.Model.EntityFramework;

  /// <summary>
  /// The selection.
  /// </summary>
  public class Configuration : DependencyObject, INotifyPropertyChanged
  {
    #region Constants and Fields

    /// <summary>
    /// Navigate Target
    /// </summary>
    public static readonly DependencyProperty NavigateTargetProperty = DependencyProperty.Register(
      "NavigateTarget", typeof(NavigateTarget), typeof(Configuration), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    /// Metro mode
    /// </summary>
    public static readonly DependencyProperty IsMetroModeProperty = DependencyProperty.Register(
      "IsMetroMode", typeof(bool), typeof(Configuration), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    /// Metro window
    /// </summary>
    public static readonly DependencyProperty MetroWindowProperty = DependencyProperty.Register(
      "MetroWindow", typeof(MetroWindow), typeof(Configuration), new FrameworkPropertyMetadata(OnPropertyChanged));
   
   /// <summary>
    /// The navigation service to navigate in metro mode between pages
    /// </summary>
    public static readonly DependencyProperty NavigationServiceProperty = DependencyProperty.Register(
      "NavigationService", typeof(NavigationService), typeof(Configuration), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    /// Datenverzeichnis
    /// </summary>
    public static readonly DependencyProperty BasePathProperty = DependencyProperty.Register(
      "BasePath", typeof(string), typeof(Configuration), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    ///   Lehrername und Adresse
    /// </summary>
    public static readonly DependencyProperty LehrerProperty = DependencyProperty.Register(
      "Lehrer", typeof(Person), typeof(Configuration), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    ///   Schulname und Adresse
    /// </summary>
    public static readonly DependencyProperty SchuleProperty = DependencyProperty.Register(
      "Schule", typeof(Schule), typeof(Configuration), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    ///   Schulname und Adresse
    /// </summary>
    public static readonly DependencyProperty NotenProStundeProperty = DependencyProperty.Register(
      "NotenProStunde", typeof(int), typeof(Configuration), new FrameworkPropertyMetadata(OnPropertyChanged));

    /// <summary>
    ///   The instance.
    /// </summary>
    private static Configuration instance;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Verhindert, dass eine Standardinstanz der <see cref="Configuration"/> Klasse erstellt wird. 
    /// </summary>
    private Configuration()
    {
      this.Lehrer = new Person();
      this.Schule = new Schule();
      this.NotenProStunde = 5;
    }

    #endregion

    #region Public Events

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Public Properties

    /// <summary>
    ///   Holt den <see cref = "Configuration" /> singleton.
    ///   If the underlying instance is null, a instance will be created.
    /// </summary>
    public static Configuration Instance
    {
      get
      {
        // check again, if the underlying instance is null
        // return the existing/new instance
        return instance ?? (instance = new Configuration());
      }
    }

    /// <summary>
    ///   Holt oder setzt einen Wert, der angibt, ob das Programm im Metro Mode ist.
    /// </summary>
    public bool IsMetroMode
    {
      get
      {
        return (bool)this.GetValue(IsMetroModeProperty);
      }

      set
      {
        this.SetValue(IsMetroModeProperty, value);
      }
    }

    /// <summary>
    ///   Holt oder setzt einen Wert, der angibt, wohin das Programm nach
    /// der Auswahl der Schülerliste springen soll (im Metro mode)
    /// </summary>
    public NavigateTarget NavigateTarget
    {
      get
      {
        return (NavigateTarget)this.GetValue(NavigateTargetProperty);
      }

      set
      {
        this.SetValue(NavigateTargetProperty, value);
      }
    }

    public NavigationService NavigationService
    {
      get
      {
        return (NavigationService)this.GetValue(NavigationServiceProperty);
      }

      set
      {
        this.SetValue(NavigationServiceProperty, value);
      }
    }

    public MetroWindow MetroWindow
    {
      get
      {
        return (MetroWindow)this.GetValue(MetroWindowProperty);
      }

      set
      {
        this.SetValue(MetroWindowProperty, value);
      }
    }

    /// <summary>
    ///   Holt oder setzt die BasePath der Konfiguration.
    /// </summary>
    public string BasePath
    {
      get
      {
        return (string)this.GetValue(BasePathProperty);
      }

      set
      {
        this.SetValue(BasePathProperty, value);
      }
    }

    /// <summary>
    ///   Holt oder setzt die Lehrer der Konfiguration.
    /// </summary>
    public Person Lehrer
    {
      get
      {
        return (Person)this.GetValue(LehrerProperty);
      }

      set
      {
        this.SetValue(LehrerProperty, value);
      }
    }

    /// <summary>
    ///   Holt oder setzt die Schule der Konfiguration.
    /// </summary>
    public Schule Schule
    {
      get
      {
        return (Schule)this.GetValue(SchuleProperty);
      }

      set
      {
        this.SetValue(SchuleProperty, value);
      }
    }

    /// <summary>
    ///   Holt oder setzt die NotenProStunde
    /// </summary>
    public int NotenProStunde
    {
      get
      {
        return (int)this.GetValue(NotenProStundeProperty);
      }

      set
      {
        this.SetValue(NotenProStundeProperty, value);
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the path to the Leuda directory in the
    ///   local application data folder of the user, which is writable
    ///   even if the user has not admin rights.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> with the path to users
    ///   local application data e.g. C:\Users\%Username%\AppData\Roaming\SoftTeach
    /// </returns>
    public static string GetLocalApplicationDataPath()
    {
      var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      localApplicationData += Path.DirectorySeparatorChar + "SoftTeach";
      if (!Directory.Exists(localApplicationData))
      {
        Directory.CreateDirectory(localApplicationData);
      }

      return localApplicationData;
    }

    /// <summary>
    /// Returns the path to the SoftTeach directory in the
    ///   local application data folder of the user, which is writable
    ///   even if the user has not admin rights.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> with the path to users
    /// MyDocuments data e.g. C:\Users\%Username%\MyDocuments\SoftTeach
    /// </returns>
    public static string GetMyDocumentsPath()
    {
      var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      myDocuments += Path.DirectorySeparatorChar + "SoftTeach";
      if (!Directory.Exists(myDocuments))
      {
        Directory.CreateDirectory(myDocuments);
      }

      return myDocuments;
    }

    /// <summary>
    /// The deserialize.
    /// </summary>
    public static void Deserialize()
    {
      var path = GetLocalApplicationDataPath();

      var schuleFile = Path.Combine(path, "Schule.xml");

      if (!File.Exists(schuleFile))
      {
        return;
      }

      using (var fs = new FileStream(schuleFile, FileMode.OpenOrCreate))
      {
        var dcsSchule = new DataContractSerializer(typeof(Schule));
        Instance.Schule = (Schule)dcsSchule.ReadObject(fs);
      }

      using (var fs = new FileStream(Path.Combine(path, "Lehrer.xml"), FileMode.OpenOrCreate))
      {
        var dcsLehrer = new DataContractSerializer(typeof(Person));
        Instance.Lehrer = (Person)dcsLehrer.ReadObject(fs);
      }
    }

    /// <summary>
    /// The serialize.
    /// </summary>
    public void Serialize()
    {
      var path = GetLocalApplicationDataPath();

      var dcsLehrer = new DataContractSerializer(typeof(Person));
      using (var fs = new FileStream(Path.Combine(path, "Lehrer.xml"), FileMode.OpenOrCreate))
      {
        dcsLehrer.WriteObject(fs, Instance.Lehrer);
      }

      var dcsSchule = new DataContractSerializer(typeof(Schule));
      using (var fs = new FileStream(Path.Combine(path, "Schule.xml"), FileMode.OpenOrCreate))
      {
        dcsSchule.WriteObject(fs, Instance.Schule);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="propertyName">
    /// The property name.
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(args.Property.Name));
      }
    }

    /// <summary>
    /// The on property changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var configuration = obj as Configuration;
      if (configuration != null)
      {
        configuration.OnPropertyChanged(args);
      }
    }

    #endregion

  }
}