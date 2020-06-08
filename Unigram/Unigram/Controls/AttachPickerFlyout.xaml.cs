using System;
using Unigram.Collections;
using Unigram.Entities;
using Unigram.ViewModels;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Unigram.Controls
{
    public sealed partial class AttachPickerFlyout : UserControl
    {
        public DialogViewModel ViewModel => DataContext as DialogViewModel;

        public AttachPickerFlyout()
        {
            InitializeComponent();
            SelectedItems = new System.Collections.Generic.List<StorageMedia>();
            Library.ItemsSource = MediaLibraryCollection.GetForCurrentView();
        }

        private System.Collections.Generic.IList<StorageMedia> SelectedItems
        {
            get;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SelectedItems.Clear();
            Library.MaxWidth = Window.Current.Bounds.Width - 24;
            Library.MinWidth = Library.MaxWidth;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is StorageMedia sm)
                SelectedItems.Remove(sm);
            ItemSelectionChanged?.Invoke(this, new MediaSelectedItemsEventArgs(SelectedItems));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is StorageMedia sm)
                SelectedItems.Add(sm);
            ItemSelectionChanged?.Invoke(this, new MediaSelectedItemsEventArgs(SelectedItems));
        }

        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
                cb.IsChecked = false;
        }

        private void Library_ItemClick(object sender, ItemClickEventArgs e)
        {
            ItemClick?.Invoke(this, new MediaSelectedEventArgs((StorageMedia)e.ClickedItem, true));
        }

        private async void Camera_Click(object sender, RoutedEventArgs e)
        {
            var capture = new CameraCaptureUI();
            //capture.PhotoSettings.AllowCropping = true;
            capture.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            capture.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;
            capture.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;
            capture.VideoSettings.MaxResolution = CameraCaptureUIMaxVideoResolution.HighestAvailable;

            var file = await capture.CaptureFileAsync(CameraCaptureUIMode.PhotoOrVideo);
            if (file != null)
            {
                if (file.ContentType.Equals("video/mp4"))
                {
                    if (Services.SettingsService.Current.SaveCameraMediaInGallery) await file.CopyAsync(KnownFolders.CameraRoll, DateTime.Now.ToString("UM_yyyyMMdd_HH_mm_ss") + ".mp4", NameCollisionOption.GenerateUniqueName);
                    ItemClick?.Invoke(this, new MediaSelectedEventArgs(await StorageVideo.CreateAsync(file, true), false));
                }
                else
                {
                    if (Services.SettingsService.Current.SaveCameraMediaInGallery) await file.CopyAsync(KnownFolders.CameraRoll, DateTime.Now.ToString("UM_yyyyMMdd_HH_mm_ss") + ".jpg", NameCollisionOption.GenerateUniqueName);
                    ItemClick?.Invoke(this, new MediaSelectedEventArgs(await StoragePhoto.CreateAsync(file, true), false));
                }
            }
        }

        public event EventHandler<MediaSelectedEventArgs> ItemClick;
        public event EventHandler<MediaSelectedItemsEventArgs> ItemSelectionChanged;
    }

    public class MediaSelectedEventArgs
    {
        public StorageMedia Item { get; private set; }

        public bool IsLocal { get; private set; }

        public MediaSelectedEventArgs(StorageMedia item, bool local)
        {
            Item = item;
            IsLocal = local;
        }
    }

    public class MediaSelectedItemsEventArgs
    {
        public System.Collections.Generic.IList<StorageMedia> SelectedItems { get; private set; }

        public MediaSelectedItemsEventArgs(System.Collections.Generic.IList<StorageMedia> selectedItems)
        {
            SelectedItems = selectedItems;
        }
    }
}
