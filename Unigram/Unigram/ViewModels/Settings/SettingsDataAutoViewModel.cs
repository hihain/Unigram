using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigram.Common;
using Unigram.Services;
using Unigram.Services.Settings;
using Windows.UI.Xaml.Navigation;

namespace Unigram.ViewModels.Settings
{
    public class SettingsDataAutoViewModel : TLViewModelBase
    {
        private AutoDownloadType _type;

        public SettingsDataAutoViewModel(IProtoService protoService, ICacheService cacheService, ISettingsService settingsService, IEventAggregator aggregator)
            : base(protoService, cacheService, settingsService, aggregator)
        {
            SendCommand = new RelayCommand(SendExecute);
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode navigationMode, IDictionary<string, object> state)
        {
            if (parameter is AutoDownloadType type)
            {
                _type = type;
                AutoDownloadMode mode;
                int limit;
                var preferences = Settings.AutoDownload;

                switch (type)
                {
                    case AutoDownloadType.Photos:
                        Title = Strings.Resources.AutoDownloadPhotos;
                        Header =  Strings.Resources.AutoDownloadPhotosTitle;
                        mode = preferences.Photos;
                        limit = 0;
                        break;
                    case AutoDownloadType.Videos:
                        Title = Strings.Resources.AutoDownloadVideos;
                        Header = Strings.Resources.AutoDownloadVideosTitle;
                        mode = preferences.Videos;
                        limit = preferences.MaximumVideoSize;
                        break;
                    case AutoDownloadType.VoiceMessages:
                        Title = Strings.Resources.AudioAutodownload;
                        Header = $"Auto-Download ({Strings.Resources.AudioAutodownload})"; //TODO
                        mode = preferences.VoiceMessages;
                        limit = 0;
                        break;
                    case AutoDownloadType.Documents:
                    default:
                        Title = Strings.Resources.AutoDownloadFiles;
                        Header = Strings.Resources.AutoDownloadFilesTitle;
                        mode = preferences.Documents;
                        limit = preferences.MaximumDocumentSize;
                        break;
                }

                Contacts = mode.HasFlag(AutoDownloadMode.WifiContacts);
                PrivateChats = mode.HasFlag(AutoDownloadMode.WifiPrivateChats);
                Groups = mode.HasFlag(AutoDownloadMode.WifiGroups);
                Channels = mode.HasFlag(AutoDownloadMode.WifiChannels);
                Limit = limit;
            }

            return Task.CompletedTask;
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _header;
        public string Header
        {
            get => _header;
            set => Set(ref _header, value);
        }

        private bool _contacts;
        public bool Contacts
        {
            get => _contacts;
            set => Set(ref _contacts, value);
        }

        private bool _privateChats;
        public bool PrivateChats
        {
            get => _privateChats;
            set => Set(ref _privateChats, value);
        }

        private bool _groups;
        public bool Groups
        {
            get => _groups;
            set => Set(ref _groups, value);
        }

        private bool _channels;
        public bool Channels
        {
            get => _channels;
            set => Set(ref _channels, value);
        }

        private int _limit;
        public int Limit
        {
            get => _limit;
            set => Set(ref _limit, value);
        }

        public bool IsLimitSupported => _type != AutoDownloadType.Photos && _type != AutoDownloadType.VoiceMessages;

        public RelayCommand SendCommand { get; }
        private void SendExecute()
        {
            var preferences = Settings.AutoDownload;
            var mode = (AutoDownloadMode)0;

            if (_contacts)
            {
                mode |= AutoDownloadMode.WifiContacts;
            }
            if (_privateChats)
            {
                mode |= AutoDownloadMode.WifiPrivateChats;
            }
            if (_groups)
            {
                mode |= AutoDownloadMode.WifiGroups;
            }
            if (_channels)
            {
                mode |= AutoDownloadMode.WifiChannels;
            }

            switch (_type)
            {
                case AutoDownloadType.Photos:
                    preferences = preferences.UpdatePhotosMode(mode);
                    break;
                case AutoDownloadType.VoiceMessages:
                    preferences = preferences.UpdateVoiceMessagesMode(mode);
                    break;
                case AutoDownloadType.Videos:
                    preferences = preferences.UpdateVideosMode(mode, _limit);
                    break;
                case AutoDownloadType.Documents:
                    preferences = preferences.UpdateDocumentsMode(mode, _limit);
                    break;
            }

            Settings.AutoDownload = preferences;
            NavigationService.GoBack();
        }
    }
}
