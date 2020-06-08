using Unigram.ViewModels.Settings;

namespace Unigram.Views.Settings
{
    public sealed partial class SettingsVoIPPage : HostedPage
    {
        public SettingsVoIPViewModel ViewModel => DataContext as SettingsVoIPViewModel;

        public SettingsVoIPPage()
        {
            InitializeComponent();
            DataContext = TLContainer.Current.Resolve<SettingsVoIPViewModel>();
        }

        #region Binding

        private string ConvertVolumeLabel(float value, bool output)
        {
            return string.Format("{0}: {1}", output ? Strings.Additional.OutputVolume : Strings.Additional.InputVolume, (int)(value * 100));
        }

        private double ConvertVolume(float value)
        {
            return (double)value * 100d;
        }

        private void ConvertVolumeOutput(double value)
        {
            ViewModel.OutputVolume = (float)value / 100f;
        }

        private void ConvertVolumeInput(double value)
        {
            ViewModel.InputVolume = (float)value / 100f;
        }

        #endregion

    }
}
