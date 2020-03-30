using System;
using System.Collections;
using System.Collections.Generic;

namespace Telegram.Td.Api
{
    /// <summary>
    /// Revert commit. Just a tryout to see if it gets somewhere as regarding to the Unigram developer the current version is based on develop + cherry-picked commits from vnext...
    /// </summary>
    public class ChatListFilters
    {
        public IEnumerable<ChatListFilter> Filters { get; internal set; }
    }
}