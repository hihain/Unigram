//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------
// <auto-generatedInfo>
// 	This code was generated by ResW File Code Generator (http://bit.ly/reswcodegen)
// 	ResW File Code Generator was written by Christian Resma Helle
// 	and is under GNU General Public License version 2 (GPLv2)
// 
// 	This code contains a helper class exposing property representations
// 	of the string resources defined in the specified .ResW file
// 
// 	Generated: 12/28/2019 03:06:59
// </auto-generatedInfo>
// --------------------------------------------------------------------------------------------------
namespace Unigram.Strings
{
    using Windows.ApplicationModel.Resources;
    
    
    public sealed partial class Additional
    {
        
        private static ResourceLoader resourceLoader;
        
        /// <summary>
        /// Get or set ResourceLoader implementation
        /// </summary>
        public static ResourceLoader Resource
        {
            get
            {
                if ((resourceLoader == null))
                {
                    Additional.Initialize();
                }
                return resourceLoader;
            }
            set
            {
                resourceLoader = value;
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Bots"
        /// </summary>
        public static string ChatFilterBots
        {
            get
            {
                return Resource.GetString("ChatFilterBots");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Channels"
        /// </summary>
        public static string ChatFilterChannels
        {
            get
            {
                return Resource.GetString("ChatFilterChannels");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Groups"
        /// </summary>
        public static string ChatFilterGroups
        {
            get
            {
                return Resource.GetString("ChatFilterGroups");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Unmuted Chats"
        /// </summary>
        public static string ChatFilterUnmuted
        {
            get
            {
                return Resource.GetString("ChatFilterUnmuted");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Unread Chats"
        /// </summary>
        public static string ChatFilterUnread
        {
            get
            {
                return Resource.GetString("ChatFilterUnread");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Unread & Unmuted Chats"
        /// </summary>
        public static string ChatFilterUnreadAndUnmuted
        {
            get
            {
                return Resource.GetString("ChatFilterUnreadAndUnmuted");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Users"
        /// </summary>
        public static string ChatFilterUsers
        {
            get
            {
                return Resource.GetString("ChatFilterUsers");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Chats"
        /// </summary>
        public static string Chats
        {
            get
            {
                return Resource.GetString("Chats");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Copy Image"
        /// </summary>
        public static string CopyImage
        {
            get
            {
                return Resource.GetString("CopyImage");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Ausschneiden"
        /// </summary>
        public static string Cut
        {
            get
            {
                return Resource.GetString("Cut");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Deleted message"
        /// </summary>
        public static string DeletedMessage
        {
            get
            {
                return Resource.GetString("DeletedMessage");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Edit Link"
        /// </summary>
        public static string EditLink
        {
            get
            {
                return Resource.GetString("EditLink");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Formatting"
        /// </summary>
        public static string Formatting
        {
            get
            {
                return Resource.GetString("Formatting");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "News"
        /// </summary>
        public static string News
        {
            get
            {
                return Resource.GetString("News");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "All Chats"
        /// </summary>
        public static string NoChatFilter
        {
            get
            {
                return Resource.GetString("NoChatFilter");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "original"
        /// </summary>
        public static string OriginalMessage
        {
            get
            {
                return Resource.GetString("OriginalMessage");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Paste"
        /// </summary>
        public static string Paste
        {
            get
            {
                return Resource.GetString("Paste");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Redo"
        /// </summary>
        public static string Redo
        {
            get
            {
                return Resource.GetString("Redo");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Save as..."
        /// </summary>
        public static string SaveAs
        {
            get
            {
                return Resource.GetString("SaveAs");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Select"
        /// </summary>
        public static string Select
        {
            get
            {
                return Resource.GetString("Select");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Select All"
        /// </summary>
        public static string SelectAll
        {
            get
            {
                return Resource.GetString("SelectAll");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Show in Folder"
        /// </summary>
        public static string ShowInFolder
        {
            get
            {
                return Resource.GetString("ShowInFolder");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Undo"
        /// </summary>
        public static string Undo
        {
            get
            {
                return Resource.GetString("Undo");
            }
        }
        
        public static void Initialize()
        {
            string executingAssemblyName;
            executingAssemblyName = Windows.UI.Xaml.Application.Current.GetType().AssemblyQualifiedName;
            string[] executingAssemblySplit;
            executingAssemblySplit = executingAssemblyName.Split(',');
            executingAssemblyName = executingAssemblySplit[1];
            string currentAssemblyName;
            currentAssemblyName = typeof(Additional).AssemblyQualifiedName;
            string[] currentAssemblySplit;
            currentAssemblySplit = currentAssemblyName.Split(',');
            currentAssemblyName = currentAssemblySplit[1];
            if (executingAssemblyName.Equals(currentAssemblyName))
            {
                resourceLoader = ResourceLoader.GetForCurrentView("Additional");
            }
            else
            {
                resourceLoader = ResourceLoader.GetForCurrentView(currentAssemblyName + "/Additional");
            }
        }
    }
}
