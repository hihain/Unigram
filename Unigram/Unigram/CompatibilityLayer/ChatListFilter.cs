using System;
using System.Collections;

namespace Telegram.Td.Api
{
    /// <summary>
    /// Revert commit. Just a tryout to see if it gets somewhere as regarding to the Unigram developer the current version is based on develop + cherry-picked commits from vnext...
    /// </summary>
    public class ChatListFilter
    {
        public IList IncludeChats { get; internal set; }
        public IList ExcludeChats { get; internal set; }
        public bool ExcludeMuted { get; internal set; }
        public bool ExcludeRead { get; internal set; }
        public bool IncludeBots { get; internal set; }
        public bool IncludeContacts { get; internal set; }
        public bool IncludeNonContacts { get; internal set; }
        public bool IncludeLargeGroups { get; internal set; }
        public bool IncludeSmallGroups { get; internal set; }
        public bool IncludeChannels { get; internal set; }
        public int Id { get; internal set; }
        public object Title { get; internal set; }

        internal bool IncludeAll() // should be a property?
        {
            throw new NotImplementedException();
        }
    }
}