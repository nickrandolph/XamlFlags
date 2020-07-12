using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Uno.Extensions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace XamlFlags
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();
        public MainPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        public static Visibility ToVisbility(bool isVisible) => isVisible ? Visibility.Visible : Visibility.Collapsed;

        public static Brush SelectedOrEnabledBackgroundColor(bool isEnabled, bool isSelected) => new SolidColorBrush(isEnabled ? (isSelected ? Colors.DarkBlue : Colors.White) : Colors.DarkGray);

        public static Brush SelectedOrEnabledForegroundColor(bool isEnabled, bool isSelected) => new SolidColorBrush(isEnabled ? (isSelected ? Colors.White : Colors.Black) : Colors.LightGray);

    }
    public class MainPageViewModel : BindableBase
    {
        public List<OptionViewModel> Options { get; }

        public MainPageViewModel()
        {
            Options = new List<OptionViewModel>
                        {
                            new OptionViewModel { Value = "Option 1-A", Category = "1", Variety = "A", Select = OnSelectType},
                            new OptionViewModel { Value = "Option 1-B", Category = "1", Variety = "B", Select = OnSelectType },
                            new OptionViewModel { Value = "Option 2-A", Category = "2", Variety = "A", Select = OnSelectType },
                            new OptionViewModel { Value = "Option 2-B", Category = "2", Variety = "B", Select = OnSelectType },
                            new OptionViewModel { Value = "Option 3-A", Category = "3", Variety = "A" , Select = OnSelectType},
                            new OptionViewModel { Value = "Option 3-B", Category = "3", Variety = "B" , Select = OnSelectType},
            };

            OnSelectType(Options.First());

        }

        private void OnSelectType(OptionViewModel option)
        {
            if (option is null) return;

            // reset all options
            Options.ForEach(o => { o.IsEnabled = false; o.IsSelected = false; });

            // enable options of the same variety (ie. A,B)
            Options.Where(o => o.Variety == option.Variety)
                .ForEach(o => { o.IsEnabled = true; });

            // enable options of the same category (ie. 1,2,3)
            Options.Where(o => o.Category == option.Category)
                .ForEach(o => { o.IsEnabled = true; });

            // select the current option
            option.IsSelected = true;
        }
    }

    public class OptionViewModel : BindableBase
    {
        public string Value { get; set; }
        public string Variety { get; set; }
        public string Category { get; set; }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }

        public Action<OptionViewModel> Select { get; set; }

        public void OnSelect() => Select(this);
    }

    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null, Action onChanged = null, Action<T> onChanging = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            onChanging?.Invoke(value);

            backingStore = value;

            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            return true;
        }
    }
}
