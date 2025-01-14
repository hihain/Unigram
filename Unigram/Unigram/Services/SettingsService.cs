﻿using System;
using System.Linq;
using System.Numerics;
using Unigram.Services.Settings;
using Windows.Globalization;
using Windows.Storage;

namespace Unigram.Services
{
    public interface ISettingsService
    {
        int Session { get; }
        ulong VersionLastStart { get; }

        void UpdateVersion();

        ChatSettingsBase Chats { get; }
        NotificationsSettings Notifications { get; }
        StickersSettings Stickers { get; }
        EmojiSettings Emoji { get; }
        AutoDownloadSettings AutoDownload { get; set; }
        AppearanceSettings Appearance { get; }
        FiltersSettings Filters { get; }
        PasscodeLockSettings PasscodeLock { get; }
        PlaybackSettings Playback { get; }

        DiagnosticsSettings Diagnostics { get; }

        long UserId { get; set; }
        long PushReceiverId { get; set; }

        string FilesDirectory { get; set; }

        int VerbosityLevel { get; set; }
        bool UseTestDC { get; set; }

        bool UseThreeLinesLayout { get; set; }
        bool CollapseArchivedChats { get; set; }
        bool IsAdaptiveWideEnabled { get; set; }
        bool ShowRecentFiles { get; set; }
        bool IsTrayVisible { get; set; }
        bool IsLaunchMinimized { get; set; }
        bool IsSendByEnterEnabled { get; set; }
        bool IsTextFormattingVisible { get; set; }
        bool IsReplaceEmojiEnabled { get; set; }
        bool IsLargeEmojiEnabled { get; set; }
        bool IsContactsSyncEnabled { get; set; }
        bool IsContactsSyncRequested { get; set; }
        bool IsContactsSortedByEpoch { get; set; }
        bool IsSecretPreviewsEnabled { get; set; }
        bool IsAutoPlayAnimationsEnabled { get; set; }
        bool IsAutoPlayVideosEnabled { get; set; }
        bool IsSendGrouped { get; set; }
        bool IsAccountsSelectorExpanded { get; set; }
        bool IsAllAccountsNotifications { get; set; }
        bool SaveCameraMediaInGallery { get; set; }

        bool IsLeftTabsEnabled { get; set; }

        Vector2 Pencil { get; set; }

        DistanceUnits DistanceUnits { get; set; }

        bool AutocorrectWords { get; set; }
        bool HighlightWords { get; set; }

        bool IsStreamingEnabled { get; set; }
        double VolumeLevel { get; set; }

        int LastMessageTtl { get; set; }

        string LanguagePackId { get; set; }
        string LanguagePluralId { get; set; }
        string LanguageShownId { get; set; }

        string PushToken { get; set; }

        libtgvoip.DataSavingMode UseLessData { get; set; }

        void SetChatPinnedMessage(long chatId, long messageId);
        long GetChatPinnedMessage(long chatId);

        void Clear();
    }

    public enum DistanceUnits
    {
        Automatic,
        Kilometers,
        Miles
    }

    public class SettingsServiceBase
    {
        protected readonly ApplicationDataContainer _container;

        public SettingsServiceBase(string key)
            : this(ApplicationData.Current.LocalSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always))
        {

        }

        public SettingsServiceBase(ApplicationDataContainer container = null)
        {
            _container = container ?? ApplicationData.Current.LocalSettings;
        }



        public bool AddOrUpdateValue(string key, Object value)
        {
            return AddOrUpdateValue(_container, key, value);
        }

        protected bool AddOrUpdateValue(ApplicationDataContainer container, string key, Object value)
        {
            bool valueChanged = false;

            if (container.Values.ContainsKey(key))
            {
                if (container.Values[key] != value)
                {
                    container.Values[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                container.Values.Add(key, value);
                valueChanged = true;
            }

            return valueChanged;
        }


        public valueType GetValueOrDefault<valueType>(string key, valueType defaultValue)
        {
            return GetValueOrDefault<valueType>(_container, key, defaultValue);
        }

        protected valueType GetValueOrDefault<valueType>(ApplicationDataContainer container, string key, valueType defaultValue)
        {
            valueType value;

            if (container.Values.ContainsKey(key))
            {
                value = (valueType)container.Values[key];
            }
            else
            {
                value = defaultValue;
            }

            return value;
        }

        public virtual void Clear()
        {
            _container.Values.Clear();
        }
    }

    public class SettingsService : SettingsServiceBase, ISettingsService
    {
        private static SettingsService _current;
        public static SettingsService Current
        {
            get
            {
                if (_current == null)
                    _current = new SettingsService();

                return _current;
            }
        }

        private readonly int _session;
        private readonly ApplicationDataContainer _local;
        private readonly ApplicationDataContainer _own;

        private SettingsService()
        {
            _local = ApplicationData.Current.LocalSettings;
        }

        public SettingsService(int session)
            : base(session > 0 ? ApplicationData.Current.LocalSettings.CreateContainer($"{session}", ApplicationDataCreateDisposition.Always) : null)
        {
            _session = session;
            _local = ApplicationData.Current.LocalSettings;
            _own = ApplicationData.Current.LocalSettings.CreateContainer($"{session}", ApplicationDataCreateDisposition.Always);
        }

        public ApplicationDataContainer Container => _container;

        #region App version

        //public const ulong CurrentVersion = (4UL << 48) | (0UL << 32) | (5072UL << 16);
        //public const string CurrentChangelog = "• Chat folders.\r\n• Stream Videos and Audio files.\r\n• Improved stickers, GIFs and emojis.\r\n\r\nRead more: https://telegra.ph/Unigram-40-05-28";
        public static readonly string CurrentChangelog = $"• Pinned Messages 2.0: Pin multiple messages in any chat and see them all on a separate page\r\n• Comment on posts in channels that have a discussion group.\r\n• Get notified about replies to your comments via the new \"Replies\" chat (if you are not a member in the discussion group).\r\n• Anonymous group admins: Turn on \"Remain Anonymous\" in an admin's permissions (edit group -> admins -> tap on an admin) to let them post on behalf of the group and become invisible in the list of members.\r\n• Added a context menu button to the chat (in touch mode) for easier access to some context menus\r\n• Added in-app volume control.\r\n• Increased music player slider.\r\n• Send music playlists\r\n• Improved live locations";

        public int Session => _session;

        private ulong? _versionLastStart;
        public ulong VersionLastStart
        {
            get
            {
                if (_versionLastStart == null)
                    _versionLastStart = GetValueOrDefault("LongVersion", 0UL);

                return _versionLastStart ?? 0;
            }
            private set
            {
                _versionLastStart = value;
                AddOrUpdateValue("LongVersion", value);
            }
        }

        public void UpdateVersion()
        {
            var version = GetAppVersion();
            VersionLastStart = ((ulong)version.Major << 48) | ((ulong)version.Minor << 32);
        }

        public static ulong CurrentVersion
        {
            get
            {
                var version = GetAppVersion();
                return ((ulong)version.Major << 48) | ((ulong)version.Minor << 32);
            }
        }

        public static Windows.ApplicationModel.PackageVersion GetAppVersion()
        {
            Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
            Windows.ApplicationModel.PackageId packageId = package.Id;
            return packageId.Version;
        }

        #endregion

        private ChatSettingsBase _chats;
        public ChatSettingsBase Chats
        {
            get
            {
                return _chats = _chats ?? new ChatSettingsBase(_own);
            }
        }

        private NotificationsSettings _notifications;
        public NotificationsSettings Notifications
        {
            get
            {
                return _notifications = _notifications ?? new NotificationsSettings(_container);
            }
        }

        private static StickersSettings _stickers;
        public StickersSettings Stickers
        {
            get
            {
                return _stickers = _stickers ?? new StickersSettings(_local);
            }
        }

        private static EmojiSettings _emoji;
        public EmojiSettings Emoji
        {
            get
            {
                return _emoji = _emoji ?? new EmojiSettings();
                // c# 8.0: return _emoji ??= new EmojiSettings();
            }
        }

        private AutoDownloadSettings _autoDownload;
        public AutoDownloadSettings AutoDownload
        {
            get
            {
                return _autoDownload = _autoDownload ?? new AutoDownloadSettings(_own.CreateContainer("AutoDownload", ApplicationDataCreateDisposition.Always));
            }
            set
            {
                _autoDownload = value ?? AutoDownloadSettings.Default;
                _autoDownload.Save(_own.CreateContainer("AutoDownload", ApplicationDataCreateDisposition.Always));
            }
        }

        private static AppearanceSettings _appearance;
        public AppearanceSettings Appearance
        {
            get
            {
                return _appearance = _appearance ?? new AppearanceSettings();
            }
        }

        private static DiagnosticsSettings _diagnostics;
        public DiagnosticsSettings Diagnostics
        {
            get
            {
                return _diagnostics = _diagnostics ?? new DiagnosticsSettings();
            }
        }

        private FiltersSettings _filters;
        public FiltersSettings Filters
        {
            get
            {
                return _filters = _filters ?? new FiltersSettings(_own);
            }
        }

        private static PasscodeLockSettings _passcodeLock;
        public PasscodeLockSettings PasscodeLock
        {
            get
            {
                return _passcodeLock = _passcodeLock ?? new PasscodeLockSettings();
            }
        }

        private static PlaybackSettings _playback;
        public PlaybackSettings Playback
        {
            get
            {
                return _playback = _playback ?? new PlaybackSettings(_local);
            }
        }

        private static VoIPSettings _voip;
        public VoIPSettings VoIP
        {
            get
            {
                return _voip = _voip ?? new VoIPSettings();
            }
        }

        private string _filesDirectory;
        public string FilesDirectory
        {
            get
            {
                if (_filesDirectory == null)
                    _filesDirectory = GetValueOrDefault("FilesDirectory", null as string);

                return _filesDirectory;
            }
            set
            {
                _filesDirectory = value;
                AddOrUpdateValue("FilesDirectory", value);
            }
        }

        private int? _verbosityLevel;
        public int VerbosityLevel
        {
            get
            {
                if (_verbosityLevel == null)
#if DEBUG
                    _verbosityLevel = GetValueOrDefault(_local, "VerbosityLevel", 5);

                return _verbosityLevel ?? 5;
#else
                    _verbosityLevel = GetValueOrDefault(_local, "VerbosityLevel", 0);

                return _verbosityLevel ?? 0;
#endif
            }
            set
            {
                _verbosityLevel = value;
                AddOrUpdateValue(_local, "VerbosityLevel", value);
            }
        }

        private bool? _useTestDC;
        public bool UseTestDC
        {
            get
            {
                if (_useTestDC == null)
                    _useTestDC = GetValueOrDefault(_own, "UseTestDC", false);

                return _useTestDC ?? false;
            }
            set
            {
                _useTestDC = value;
                AddOrUpdateValue(_own, "UseTestDC", value);
            }
        }

        private long? _userId;
        public long UserId
        {
            get
            {
                if (_userId == null)
                    _userId = GetValueOrDefault(_own, "UserId", 0);

                return _userId ?? 0;
            }
            set
            {
                _userId = value;
                AddOrUpdateValue(_local, $"User{value}", Session);
                AddOrUpdateValue(_own, "UserId", value);
            }
        }

        private long? _pushReceiverId;
        public long PushReceiverId
        {
            get
            {
                if (_pushReceiverId == null)
                    _pushReceiverId = GetValueOrDefault(_own, "PushReceiverId", 0L);

                return _pushReceiverId ?? 0L;
            }
            set
            {
                _pushReceiverId = value;
                AddOrUpdateValue(_local, $"PushReceiverId{value}", Session);
                AddOrUpdateValue(_own, "PushReceiverId", value);
            }
        }

        private static int? _distanceUnits;
        public DistanceUnits DistanceUnits
        {
            get
            {
                if (_distanceUnits == null)
                    _distanceUnits = GetValueOrDefault("DistanceUnits", 0);

                return (DistanceUnits)(_distanceUnits ?? 0);
            }
            set
            {
                _distanceUnits = (int)value;
                AddOrUpdateValue("DistanceUnits", (int)value);
            }
        }

        private static double? _dialogsWidthRatio;
        public double DialogsWidthRatio
        {
            get
            {
                if (_dialogsWidthRatio == null)
                    _dialogsWidthRatio = GetValueOrDefault(_local, "DialogsWidthRatio", 5d / 14d);

                return _dialogsWidthRatio ?? 5d / 14d;
            }
            set
            {
                _dialogsWidthRatio = value;
                AddOrUpdateValue(_local, "DialogsWidthRatio", value);
            }
        }

        private bool? _isSidebarOpen;
        public bool IsSidebarOpen
        {
            get
            {
                if (_isSidebarOpen == null)
                    _isSidebarOpen = GetValueOrDefault(_local, "IsSidebarOpen", true);

                return _isSidebarOpen ?? true;
            }
            set
            {
                _isSidebarOpen = value;
                AddOrUpdateValue(_local, "IsSidebarOpen", value);
            }
        }

        private static bool? _isAdaptiveWideEnabled;
        public bool IsAdaptiveWideEnabled
        {
            get
            {
                if (_isAdaptiveWideEnabled == null)
                    _isAdaptiveWideEnabled = GetValueOrDefault(_local, "IsAdaptiveWideEnabled", false);

                return _isAdaptiveWideEnabled ?? false;
            }
            set
            {
                _isAdaptiveWideEnabled = value;
                AddOrUpdateValue(_local, "IsAdaptiveWideEnabled", value);
            }
        }
        private static bool? _showRecentFiles;
        public bool ShowRecentFiles
        {
            get
            {
                if (_showRecentFiles == null)
                    _showRecentFiles = GetValueOrDefault(_local, "ShowRecentFiles", true);

                return _showRecentFiles ?? true;
            }
            set
            {
                _showRecentFiles = value;
                AddOrUpdateValue(_local, "ShowRecentFiles", value);
            }
        }

        private static bool? _isTrayVisible;
        public bool IsTrayVisible
        {
            get
            {
                if (_isTrayVisible == null)
                    _isTrayVisible = GetValueOrDefault(_local, "IsTrayVisible", true);

                return _isTrayVisible ?? true;
            }
            set
            {
                _isTrayVisible = value;
                AddOrUpdateValue(_local, "IsTrayVisible", value);
            }
        }

        private static bool? _isLaunchMinimized;
        public bool IsLaunchMinimized
        {
            get
            {
                if (_isLaunchMinimized == null)
                    _isLaunchMinimized = GetValueOrDefault(_local, "IsLaunchMinimized", false);

                return _isLaunchMinimized ?? false;
            }
            set
            {
                _isLaunchMinimized = value;
                AddOrUpdateValue(_local, "IsLaunchMinimized", value);
            }
        }

        private static bool? _useThreeLinesLayout;
        public bool UseThreeLinesLayout
        {
            get
            {
                if (_useThreeLinesLayout == null)
                    _useThreeLinesLayout = GetValueOrDefault(_local, "UseThreeLinesLayout", false);

                return _useThreeLinesLayout ?? false;
            }
            set
            {
                _useThreeLinesLayout = value;
                AddOrUpdateValue(_local, "UseThreeLinesLayout", value);
            }
        }

        private static bool? _collapseArchivedChats;
        public bool CollapseArchivedChats
        {
            get
            {
                if (_collapseArchivedChats == null)
                    _collapseArchivedChats = GetValueOrDefault(_local, "CollapseArchivedChats", false);

                return _collapseArchivedChats ?? false;
            }
            set
            {
                _collapseArchivedChats = value;
                AddOrUpdateValue(_local, "CollapseArchivedChats", value);
            }
        }

        private static bool? _isAccountsSelectorExpanded;
        public bool IsAccountsSelectorExpanded
        {
            get
            {
                if (_isAccountsSelectorExpanded == null)
                    _isAccountsSelectorExpanded = GetValueOrDefault(_local, "IsAccountsSelectorExpanded", false);

                return _isAccountsSelectorExpanded ?? false;
            }
            set
            {
                _isAccountsSelectorExpanded = value;
                AddOrUpdateValue(_local, "IsAccountsSelectorExpanded", value);
            }
        }

        private static bool? _isAllAccountsNotifications;
        public bool IsAllAccountsNotifications
        {
            get
            {
                if (_isAllAccountsNotifications == null)
                    _isAllAccountsNotifications = GetValueOrDefault(_local, "IsAllAccountsNotifications", true);

                return _isAllAccountsNotifications ?? true;
            }
            set
            {
                _isAllAccountsNotifications = value;
                AddOrUpdateValue(_local, "IsAllAccountsNotifications", value);
            }
        }

        private static bool? _isLeftTabsEnabled;
        public bool IsLeftTabsEnabled
        {
            get
            {
                if (_isLeftTabsEnabled == null)
                    _isLeftTabsEnabled = GetValueOrDefault(_local, "IsLeftTabsEnabled", false);

                return _isLeftTabsEnabled ?? false;
            }
            set
            {
                _isLeftTabsEnabled = value;
                AddOrUpdateValue(_local, "IsLeftTabsEnabled", value);
            }
        }

        private static bool? _autocorrectWords;
        public bool AutocorrectWords
        {
            get
            {
                if (_autocorrectWords == null)
                    _autocorrectWords = GetValueOrDefault(_local, "AutocorrectWords", true);

                return _autocorrectWords ?? true;
            }
            set
            {
                _autocorrectWords = value;
                AddOrUpdateValue(_local, "AutocorrectWords", value);
            }
        }

        private static bool? _highlightWords;
        public bool HighlightWords
        {
            get
            {
                if (_highlightWords == null)
                    _highlightWords = GetValueOrDefault(_local, "HighlightWords", true);

                return _highlightWords ?? true;
            }
            set
            {
                _highlightWords = value;
                AddOrUpdateValue(_local, "HighlightWords", value);
            }
        }

        private bool? _isSendByEnterEnabled;
        public bool IsSendByEnterEnabled
        {
            get
            {
                if (_isSendByEnterEnabled == null)
                    _isSendByEnterEnabled = GetValueOrDefault("IsSendByEnterEnabled", true);

                return _isSendByEnterEnabled ?? true;
            }
            set
            {
                _isSendByEnterEnabled = value;
                AddOrUpdateValue("IsSendByEnterEnabled", value);
            }
        }

        private bool? _isTextFormattingVisible;
        public bool IsTextFormattingVisible
        {
            get
            {
                if (_isTextFormattingVisible == null)
                    _isTextFormattingVisible = GetValueOrDefault("IsTextFormattingVisible", false);

                return _isTextFormattingVisible ?? false;
            }
            set
            {
                _isTextFormattingVisible = value;
                AddOrUpdateValue("IsTextFormattingVisible", value);
            }
        }

        private bool? _isReplaceEmojiEnabled;
        public bool IsReplaceEmojiEnabled
        {
            get
            {
                if (_isReplaceEmojiEnabled == null)
                    _isReplaceEmojiEnabled = GetValueOrDefault("IsReplaceEmojiEnabled", true);

                return _isReplaceEmojiEnabled ?? true;
            }
            set
            {
                _isReplaceEmojiEnabled = value;
                AddOrUpdateValue("IsReplaceEmojiEnabled", value);
            }
        }

        private static bool? _isLargeEmojiEnabled;
        public bool IsLargeEmojiEnabled
        {
            get
            {
                if (_isLargeEmojiEnabled == null)
                    _isLargeEmojiEnabled = GetValueOrDefault(_local, "IsLargeEmojiEnabled", true);

                return _isLargeEmojiEnabled ?? true;
            }
            set
            {
                _isLargeEmojiEnabled = value;
                AddOrUpdateValue(_local, "IsLargeEmojiEnabled", value);
            }
        }

        private bool? _isContactsSyncEnabled;
        public bool IsContactsSyncEnabled
        {
            get
            {
                if (_isContactsSyncEnabled == null)
                    _isContactsSyncEnabled = GetValueOrDefault("IsContactsSyncEnabled", true);

                return _isContactsSyncEnabled ?? true;
            }
            set
            {
                _isContactsSyncEnabled = value;
                AddOrUpdateValue("IsContactsSyncEnabled", value);
            }
        }

        private bool? _isContactsSyncRequested;
        public bool IsContactsSyncRequested
        {
            get
            {
                if (_isContactsSyncRequested == null)
                    _isContactsSyncRequested = GetValueOrDefault("IsContactsSyncRequested", false);

                return _isContactsSyncRequested ?? false;
            }
            set
            {
                _isContactsSyncRequested = value;
                AddOrUpdateValue("IsContactsSyncRequested", value);
            }
        }

        private bool? _isContactsSortedByEpoch;
        public bool IsContactsSortedByEpoch
        {
            get
            {
                if (_isContactsSortedByEpoch == null)
                    _isContactsSortedByEpoch = GetValueOrDefault("IsContactsSortedByEpoch", true);

                return _isContactsSortedByEpoch ?? true;
            }
            set
            {
                _isContactsSortedByEpoch = value;
                AddOrUpdateValue("IsContactsSortedByEpoch", value);
            }
        }

        private bool? _isSecretPreviewsEnabled;
        public bool IsSecretPreviewsEnabled
        {
            get
            {
                if (_isSecretPreviewsEnabled == null)
                    _isSecretPreviewsEnabled = GetValueOrDefault("IsSecretPreviewsEnabled", false);

                return _isSecretPreviewsEnabled ?? true;
            }
            set
            {
                _isSecretPreviewsEnabled = value;
                AddOrUpdateValue("IsSecretPreviewsEnabled", value);
            }
        }

        private bool? _isAutoPlayEnabled;
        public bool IsAutoPlayAnimationsEnabled
        {
            get
            {
                if (_isAutoPlayEnabled == null)
                    _isAutoPlayEnabled = GetValueOrDefault("IsAutoPlayEnabled", true);

                return _isAutoPlayEnabled ?? true;
            }
            set
            {
                _isAutoPlayEnabled = value;
                AddOrUpdateValue("IsAutoPlayEnabled", value);
            }
        }

        private bool? _isAutoPlayVideosEnabled;
        public bool IsAutoPlayVideosEnabled
        {
            get
            {
                if (_isAutoPlayVideosEnabled == null)
                    _isAutoPlayVideosEnabled = GetValueOrDefault("IsAutoPlayVideosEnabled", true);

                return _isAutoPlayVideosEnabled ?? true;
            }
            set
            {
                _isAutoPlayVideosEnabled = value;
                AddOrUpdateValue("IsAutoPlayVideosEnabled", value);
            }
        }

        private bool? _isSendGrouped;
        public bool IsSendGrouped
        {
            get
            {
                if (_isSendGrouped == null)
                    _isSendGrouped = GetValueOrDefault("IsSendGrouped", true);

                return _isSendGrouped ?? true;
            }
            set
            {
                _isSendGrouped = value;
                AddOrUpdateValue("IsSendGrouped", value);
            }
        }

        private bool? _isStreamingEnabled;
        public bool IsStreamingEnabled
        {
            get
            {
                if (_isStreamingEnabled == null)
                    _isStreamingEnabled = GetValueOrDefault("IsStreamingEnabled", true);

                return _isStreamingEnabled ?? true;
            }
            set
            {
                _isStreamingEnabled = value;
                AddOrUpdateValue("IsStreamingEnabled", value);
            }
        }

        private static double? _volumeLevel;
        public double VolumeLevel
        {
            get
            {
                if (_volumeLevel == null)
                    _volumeLevel = GetValueOrDefault("VolumeLevel", 1d);

                return _volumeLevel ?? 1d;
            }
            set
            {
                _volumeLevel = value;
                AddOrUpdateValue("VolumeLevel", value);
            }
        }

        private static Vector2? _pencil;
        public Vector2 Pencil
        {
            get
            {
                if (_pencil == null)
                {
                    var offset = GetValueOrDefault(_local, "PencilOffset", 1f);
                    var thickness = GetValueOrDefault(_local, "PencilThickness", 0.22f);

                    _pencil = new Vector2(offset, thickness);
                }

                return _pencil ?? new Vector2(1f, 0.22f);
            }
            set
            {
                _pencil = value;
                AddOrUpdateValue(_local, "PencilOffset", value.X);
                AddOrUpdateValue(_local, "PencilThickness", value.Y);
            }
        }

        private int? _lastMessageTtl;
        public int LastMessageTtl
        {
            get
            {
                if (_lastMessageTtl == null)
                    _lastMessageTtl = GetValueOrDefault("LastMessageTtl", 7);

                return _lastMessageTtl ?? 7;
            }
            set
            {
                _lastMessageTtl = value;
                AddOrUpdateValue("LastMessageTtl", value);
            }
        }

        private int? _previousSession;
        public int PreviousSession
        {
            get
            {
                if (_previousSession == null)
                    _previousSession = GetValueOrDefault(_local, "PreviousSession", 0);

                return _activeSession ?? 0;
            }
            set
            {
                _previousSession = value;
                AddOrUpdateValue(_local, "PreviousSession", value);
            }
        }

        private int? _activeSession;
        public int ActiveSession
        {
            get
            {
                if (_activeSession == null)
                    _activeSession = GetValueOrDefault(_local, "SelectedAccount", 0);

                return _activeSession ?? 0;
            }
            set
            {
                _activeSession = value;
                AddOrUpdateValue(_local, "SelectedAccount", value);
            }
        }

        private string _languagePackId;
        public string LanguagePackId
        {
            get
            {
                if (_languagePackId == null)
                    _languagePackId = GetValueOrDefault(_local, "LanguagePackId", ApplicationLanguages.Languages[0].Split('-').First());

                return _languagePackId;
            }
            set
            {
                _languagePackId = value;
                AddOrUpdateValue(_local, "LanguagePackId", value);
            }
        }

        private string _languagePluralId;
        public string LanguagePluralId
        {
            get
            {
                if (_languagePluralId == null)
                    _languagePluralId = GetValueOrDefault(_local, "LanguagePluralId", ApplicationLanguages.Languages[0].Split('-').First());

                return _languagePluralId;
            }
            set
            {
                _languagePluralId = value;
                AddOrUpdateValue(_local, "LanguagePluralId", value);
            }
        }

        private string _languageShownId;
        public string LanguageShownId
        {
            get
            {
                if (_languageShownId == null)
                    _languageShownId = GetValueOrDefault<string>(_local, "LanguageShownId", null);

                return _languageShownId;
            }
            set
            {
                _languageShownId = value;
                AddOrUpdateValue(_local, "LanguageShownId", value);
            }
        }

        private string _pushToken;
        public string PushToken
        {
            get
            {
                if (_pushToken == null)
                    _pushToken = GetValueOrDefault<string>("ChannelUri", null);

                return _pushToken;
            }
            set
            {
                _pushToken = value;
                AddOrUpdateValue("ChannelUri", value);
            }
        }

        private libtgvoip.DataSavingMode? _useLessData;
        public libtgvoip.DataSavingMode UseLessData
        {
            get
            {
                if (_useLessData == null)
                    _useLessData = (libtgvoip.DataSavingMode)GetValueOrDefault("UseLessData", 0);

                return _useLessData ?? libtgvoip.DataSavingMode.Never;
            }
            set
            {
                _useLessData = value;
                AddOrUpdateValue("UseLessData", (int)value);
            }
        }

        private bool? _createAndSaveGifThumbnails;
        public bool CreateAndSaveGifThumbnails
        {
            get
            {
                if (_createAndSaveGifThumbnails == null)
                    _createAndSaveGifThumbnails = GetValueOrDefault("CreateAndSaveGifThumbnails", true);

                return _createAndSaveGifThumbnails ?? true;
            }
            set
            {
                _createAndSaveGifThumbnails = value;
                AddOrUpdateValue("CreateAndSaveGifThumbnails", value);
            }
        }

        private bool? _saveCameraMediaInGallery;
        public bool SaveCameraMediaInGallery
        {
            get
            {
                if (_saveCameraMediaInGallery == null)
                    _saveCameraMediaInGallery = GetValueOrDefault("SaveCameraMediaInGallery", true);

                return _saveCameraMediaInGallery ?? true;
            }
            set
            {
                _saveCameraMediaInGallery = value;
                AddOrUpdateValue("SaveCameraMediaInGallery", value);
            }
        }

        public void SetChatPinnedMessage(long chatId, long messageId)
        {
            var container = _own.CreateContainer("PinnedMessages", ApplicationDataCreateDisposition.Always);
            AddOrUpdateValue(container, $"{chatId}", messageId);
        }

        public long GetChatPinnedMessage(long chatId)
        {
            var container = _own.CreateContainer("PinnedMessages", ApplicationDataCreateDisposition.Always);
            return GetValueOrDefault(container, $"{chatId}", 0L);
        }

        public void CleanUp()
        {
            // Here should be cleaned up all the settings that are shared with background tasks.
            _useLessData = null;
        }

        public new void Clear()
        {
            _container.Values.Clear();

            if (_own != null)
            {
                _own.Values.Clear();
            }

            if (_local != null)
            {
                _local.Values.Remove($"User{UserId}");
            }
        }
    }

    public class ChatSettingsBase : SettingsServiceBase
    {
        public ChatSettingsBase(ApplicationDataContainer container = null)
            : base(container)
        {
        }

        public object this[long chatId, long threadId, ChatSetting key]
        {
            //get => GetValueOrDefault<object>(chatId + key, null);
            set => AddOrUpdateValue(ConvertToKey(chatId, threadId, key), value);
        }

        public bool TryRemove<T>(long chatId, long threadId, ChatSetting key, out T value)
        {
            if (_container.Values.ContainsKey(ConvertToKey(chatId, threadId, key)))
            {
                value = (T)_container.Values[ConvertToKey(chatId, threadId, key)];
                _container.Values.Remove(ConvertToKey(chatId, threadId, key));
                return true;
            }

            value = default;
            return false;
        }

        private string ConvertToKey(long chatId, long threadId, ChatSetting setting)
        {
            if (threadId != 0)
            {
                return $"{chatId}{threadId}{setting}";
            }

            return $"{chatId}{setting}";
        }
    }

    public enum ChatSetting
    {
        Index,
        Pixel,
        ReadInboxMaxId
    }
}
