using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Unigram.Common;
using Unigram.Entities;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Unigram.Collections
{
    public class MediaLibraryCollection : IncrementalCollection<StorageMedia>, ISupportIncrementalLoading
    {
        private readonly CoreDispatcher _dispatcher;
        private readonly DisposableMutex _loadMoreLock;

        private StorageFileQueryResult _query;
        private uint _startIndex;

        private MediaLibraryCollection()
        {
            _dispatcher = Window.Current.Dispatcher;
            _loadMoreLock = new DisposableMutex();
        }

        private static Dictionary<int, WeakReference<MediaLibraryCollection>> _windowContext = new Dictionary<int, WeakReference<MediaLibraryCollection>>();
        public static MediaLibraryCollection GetForCurrentView()
        {
            var id = ApplicationView.GetApplicationViewIdForWindow(Window.Current.CoreWindow);
            if (_windowContext.TryGetValue(id, out WeakReference<MediaLibraryCollection> reference) && reference.TryGetTarget(out MediaLibraryCollection value))
            {
                return value;
            }

            var context = new MediaLibraryCollection();
            _windowContext[id] = new WeakReference<MediaLibraryCollection>(context);

            return context;
        }

        private async void OnContentsChanged(IStorageQueryResultBase sender, object args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _startIndex = 0;
                Clear();
            });
        }

        public override Task<IList<StorageMedia>> LoadDataAsync()
        {
            return Task.Run<IList<StorageMedia>>(async () =>
            {
                using (await _loadMoreLock.WaitAsync())
                {
                    if (_query == null)
                    {
                        await KnownFolders.PicturesLibrary.TryGetItemAsync("yolo");

                        var queryOptions = new QueryOptions(CommonFileQuery.OrderByDate, Constants.MediaTypes);
                        queryOptions.FolderDepth = FolderDepth.Deep;

                        _query = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions);
                        _query.ContentsChanged += OnContentsChanged;
                        _startIndex = 0;
                    }

                    var items = new List<StorageMedia>();
                    uint resultCount = 0;
                    var result = await _query.GetFilesAsync(_startIndex, 10);
                    _startIndex += (uint)result.Count;

                    resultCount = (uint)result.Count;

                    foreach (var file in result)
                    {
                        var storage = await StorageMedia.CreateAsync(file, false);
                        if (storage != null)
                        {
                            items.Add(storage);
                        }
                    }

                    return items;
                }
            });
        }
    }
}
