﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code ert generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftTeach.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.2.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2019/2020")]
        public string Schuljahr {
            get {
                return ((string)(this["Schuljahr"]));
            }
            set {
                this["Schuljahr"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Winter")]
        public string Halbjahr {
            get {
                return ((string)(this["Halbjahr"]));
            }
            set {
                this["Halbjahr"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Physik")]
        public string Fach {
            get {
                return ((string)(this["Fach"]));
            }
            set {
                this["Fach"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("7")]
        public string Jahrgang {
            get {
                return ((string)(this["Jahrgang"]));
            }
            set {
                this["Jahrgang"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("7D")]
        public string Lerngruppe {
            get {
                return ((string)(this["Lerngruppe"]));
            }
            set {
                this["Lerngruppe"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Modul {
            get {
                return ((string)(this["Modul"]));
            }
            set {
                this["Modul"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("c:\\temp")]
        public string LogfilePath {
            get {
                return ((string)(this["LogfilePath"]));
            }
            set {
                this["LogfilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF006400")]
        public global::System.Windows.Media.SolidColorBrush FerienColor {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["FerienColor"]));
            }
            set {
                this["FerienColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FF90EE90")]
        public global::System.Windows.Media.SolidColorBrush WeekendColor {
            get {
                return ((global::System.Windows.Media.SolidColorBrush)(this["WeekendColor"]));
            }
            set {
                this["WeekendColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("80")]
        public int Wochenbreite {
            get {
                return ((int)(this["Wochenbreite"]));
            }
            set {
                this["Wochenbreite"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("40")]
        public int Stundenbreite {
            get {
                return ((int)(this["Stundenbreite"]));
            }
            set {
                this["Stundenbreite"] = value;
            }
        }
    }
}
