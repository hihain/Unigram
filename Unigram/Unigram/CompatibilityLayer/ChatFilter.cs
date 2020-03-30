using System.Collections.Generic;
namespace Telegram.Td.Api
{
    /// <summary>
    /// Revert commit. Just a tryout to see if it gets somewhere as regarding to the Unigram developer the current version is based on develop + cherry-picked commits from vnext...
    /// </summary>
    public class ChatFilter
    {
        public int? Id { get; set; }
        public string Title { get; set; }

        public bool IncludePrivate { get; set; } = true;
        public bool IncludeSecret { get; set; } = true;
        public bool IncludePrivateGroups { get; set; } = true;
        public bool IncludePublicGroups { get; set; } = true;
        public bool IncludeChannels { get; set; } = true;
        public bool IncludeBots { get; set; } = true;

        public bool ExcludeMuted { get; set; }
        public bool ExcludeRead { get; set; }

        public IEnumerable<long> IncludeChats { get; private set; }
    }
}