namespace Unigram.Services.Settings
{
    public class DiagnosticsSettings : SettingsServiceBase
    {
        public DiagnosticsSettings()
            : base("Diagnostics")
        {
        }

        private bool? _showFilesInFolder;
        public bool ShowFilesInFolder
        {
            get
            {
                if (_showFilesInFolder == null)
                    _showFilesInFolder = GetValueOrDefault("ShowFilesInFolder", false);

                return _showFilesInFolder ?? false;
            }
            set
            {
                _showFilesInFolder = value;
                AddOrUpdateValue("ShowFilesInFolder", value);
            }
        }

        private bool? _bubbleMeasureAlpha;
        public bool BubbleMeasureAlpha
        {
            get
            {
                if (_bubbleMeasureAlpha == null)
                    _bubbleMeasureAlpha = GetValueOrDefault("BubbleMeasureAlpha", true);

                return _bubbleMeasureAlpha ?? true;
            }
            set
            {
                _bubbleMeasureAlpha = value;
                AddOrUpdateValue("BubbleMeasureAlpha", value);
            }
        }

        private bool? _bubbleKnockout;
        public bool BubbleKnockout
        {
            get
            {
                if (_bubbleKnockout == null)
                    _bubbleKnockout = GetValueOrDefault("BubbleKnockout", false);

                return _bubbleKnockout ?? false;
            }
            set
            {
                _bubbleKnockout = value;
                AddOrUpdateValue("BubbleKnockout", value);
            }
        }
    }
}
