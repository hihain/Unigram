namespace Telegram.Td.Api
{
    /// <summary>
    /// Revert commit. Just a tryout to see if it gets somewhere as regarding to the Unigram developer the current version is based on develop + cherry-picked commits from vnext...
    /// </summary>
    public class ChatListFilterFlags // Enum?
    {
        public static ChatListFilterFlags ExcludeMuted { get; internal set; }
        public static ChatListFilterFlags ExcludeRead { get; internal set; }
        public static ChatListFilterFlags IncludeContacts { get; internal set; }
        public static ChatListFilterFlags IncludeNonContacts { get; internal set; }
        public static ChatListFilterFlags IncludeLargeGroups { get; internal set; }
        public static ChatListFilterFlags IncludeSmallGroups { get; internal set; }
        public static ChatListFilterFlags IncludeChannels { get; internal set; }
        public static ChatListFilterFlags IncludeBots { get; internal set; }
    }
}