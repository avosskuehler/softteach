namespace Liduv.ViewModel.Helper.PushBinding
{
  using System.ComponentModel;
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Data;
  using System.Globalization;
  using System.Windows.Controls;

  public class FreezableBinding : Freezable
    {
        #region Properties

        private Binding _binding;
        protected Binding Binding
        {
            get
            {
                if (this._binding == null)
                {
                    this._binding = new Binding();
                }
                return this._binding;
            }
        }

        [DefaultValue(null)]
        public object AsyncState
        {
            get { return this.Binding.AsyncState; }
            set { this.Binding.AsyncState = value; }
        }

        [DefaultValue(false)]
        public bool BindsDirectlyToSource
        {
            get { return this.Binding.BindsDirectlyToSource; }
            set { this.Binding.BindsDirectlyToSource = value; }
        }

        [DefaultValue(null)]
        public IValueConverter Converter
        {
            get { return this.Binding.Converter; }
            set { this.Binding.Converter = value; }
        }

        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter)), DefaultValue(null)]
        public CultureInfo ConverterCulture
        {
            get { return this.Binding.ConverterCulture; }
            set { this.Binding.ConverterCulture = value; }
        }

        [DefaultValue(null)]

        public object ConverterParameter
        {
            get { return this.Binding.ConverterParameter; }
            set { this.Binding.ConverterParameter = value; }
        }

        [DefaultValue(null)]
        public string ElementName
        {
            get { return this.Binding.ElementName; }
            set { this.Binding.ElementName = value; }
        }

        [DefaultValue(null)]
        public object FallbackValue
        {
            get { return this.Binding.FallbackValue; }
            set { this.Binding.FallbackValue = value; }
        }

        [DefaultValue(false)]
        public bool IsAsync
        {
            get { return this.Binding.IsAsync; }
            set { this.Binding.IsAsync = value; }
        }

        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode
        {
            get { return this.Binding.Mode; }
            set { this.Binding.Mode = value; }
        }

        [DefaultValue(false)]
        public bool NotifyOnSourceUpdated
        {
            get { return this.Binding.NotifyOnSourceUpdated; }
            set { this.Binding.NotifyOnSourceUpdated = value; }
        }

        [DefaultValue(false)]
        public bool NotifyOnTargetUpdated
        {
            get { return this.Binding.NotifyOnTargetUpdated; }
            set { this.Binding.NotifyOnTargetUpdated = value; }
        }

        [DefaultValue(false)]
        public bool NotifyOnValidationError
        {
            get { return this.Binding.NotifyOnValidationError; }
            set { this.Binding.NotifyOnValidationError = value; }
        }

        [DefaultValue(null)]
        public PropertyPath Path
        {
            get { return this.Binding.Path; }
            set { this.Binding.Path = value; }
        }

        [DefaultValue(null)]
        public RelativeSource RelativeSource
        {
            get { return this.Binding.RelativeSource; }
            set { this.Binding.RelativeSource = value; }
        }

        [DefaultValue(null)]
        public object Source
        {
            get { return this.Binding.Source; }
            set { this.Binding.Source = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
        {
            get { return this.Binding.UpdateSourceExceptionFilter; }
            set { this.Binding.UpdateSourceExceptionFilter = value; }
        }

        [DefaultValue(UpdateSourceTrigger.PropertyChanged)]
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return this.Binding.UpdateSourceTrigger; }
            set { this.Binding.UpdateSourceTrigger = value; }
        }

        [DefaultValue(false)]
        public bool ValidatesOnDataErrors
        {
            get { return this.Binding.ValidatesOnDataErrors; }
            set { this.Binding.ValidatesOnDataErrors = value; }
        }

        [DefaultValue(false)]
        public bool ValidatesOnExceptions
        {
            get { return this.Binding.ValidatesOnExceptions; }
            set { this.Binding.ValidatesOnExceptions = value; }
        }

        [DefaultValue(null)]
        public string XPath
        {
            get { return this.Binding.XPath; }
            set { this.Binding.XPath = value; }
        }

        [DefaultValue(null)]
        public Collection<ValidationRule> ValidationRules
        {
            get { return this.Binding.ValidationRules; }
        }

        #endregion // Properties

        #region Freezable overrides

        protected override void CloneCore(Freezable sourceFreezable)
        {
            FreezableBinding freezableBindingClone = sourceFreezable as FreezableBinding;
            if (freezableBindingClone.ElementName != null)
            {
                this.ElementName = freezableBindingClone.ElementName;
            }
            else if (freezableBindingClone.RelativeSource != null)
            {
                this.RelativeSource = freezableBindingClone.RelativeSource;
            }
            else if (freezableBindingClone.Source != null)
            {
                this.Source = freezableBindingClone.Source;
            }
            this.AsyncState = freezableBindingClone.AsyncState;
            this.BindsDirectlyToSource = freezableBindingClone.BindsDirectlyToSource;
            this.Converter = freezableBindingClone.Converter;
            this.ConverterCulture = freezableBindingClone.ConverterCulture;
            this.ConverterParameter = freezableBindingClone.ConverterParameter;
            this.FallbackValue = freezableBindingClone.FallbackValue;
            this.IsAsync = freezableBindingClone.IsAsync;
            this.Mode = freezableBindingClone.Mode;
            this.NotifyOnSourceUpdated = freezableBindingClone.NotifyOnSourceUpdated;
            this.NotifyOnTargetUpdated = freezableBindingClone.NotifyOnTargetUpdated;
            this.NotifyOnValidationError = freezableBindingClone.NotifyOnValidationError;
            this.Path = freezableBindingClone.Path;
            this.UpdateSourceExceptionFilter = freezableBindingClone.UpdateSourceExceptionFilter;
            this.UpdateSourceTrigger = freezableBindingClone.UpdateSourceTrigger;
            this.ValidatesOnDataErrors = freezableBindingClone.ValidatesOnDataErrors;
            this.ValidatesOnExceptions = freezableBindingClone.ValidatesOnExceptions;
            this.XPath = this.XPath;
            foreach (ValidationRule validationRule in freezableBindingClone.ValidationRules)
            {
                this.ValidationRules.Add(validationRule);
            }
            base.CloneCore(sourceFreezable);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new FreezableBinding();
        }

        #endregion // Freezable overrides
    }
}
