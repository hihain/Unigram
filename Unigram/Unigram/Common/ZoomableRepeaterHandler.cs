﻿using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Td.Api;
using Unigram.ViewModels.Drawers;
using Unigram.Views.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Unigram.Common
{
    public class ZoomableRepeaterHandler
    {
        private readonly ItemsRepeater _listView;
        private readonly DispatcherTimer _throttler;

        private FrameworkElement _element;
        private Pointer _pointer;

        private ZoomableMediaPopup _popupPanel;
        private Popup _popupHost;
        private object _popupContent;

        public ZoomableRepeaterHandler(ItemsRepeater listView)
        {
            _listView = listView;
            _listView.Loaded += OnLoaded;
            _listView.Unloaded += OnUnloaded;

            _popupHost = new Popup();
            _popupHost.IsHitTestVisible = false;
            _popupHost.Child = _popupPanel = new ZoomableMediaPopup();

            _throttler = new DispatcherTimer();
            _throttler.Interval = TimeSpan.FromMilliseconds(Constants.HoldingThrottle);
            _throttler.Tick += (s, args) =>
            {
                _throttler.Stop();
                DoSomething(_popupContent);
            };
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _listView.PointerMoved += OnPointerMoved;
            _listView.PointerReleased += OnPointerCaptureLost;
            _listView.PointerCanceled += OnPointerCaptureLost;
            _listView.PointerCaptureLost += OnPointerCaptureLost;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _listView.Loaded -= OnLoaded;
            _listView.Unloaded -= OnUnloaded;
            _listView.PointerMoved -= OnPointerMoved;
            _listView.PointerReleased -= OnPointerCaptureLost;
            _listView.PointerCanceled -= OnPointerCaptureLost;
            _listView.PointerCaptureLost -= OnPointerCaptureLost;
        }

        public Action<int> DownloadFile
        {
            get => _popupPanel.DownloadFile;
            set => _popupPanel.DownloadFile = value;
        }

        public Func<int, Task<BaseObject>> GetEmojisAsync
        {
            get => _popupPanel.GetEmojisAsync;
            set => _popupPanel.GetEmojisAsync = value;
        }

        public Action Opening { get; set; }
        public Action Closing { get; set; }

        public void UpdateFile(File file)
        {
            _popupPanel.UpdateFile(file);
        }

        private PointerEventHandler _handlerPressed;
        private PointerEventHandler _handlerReleased;
        private PointerEventHandler _handlerExited;

        public void ElementPrepared(UIElement container)
        {
            container.AddHandler(UIElement.PointerPressedEvent, _handlerPressed = _handlerPressed ?? new PointerEventHandler(OnPointerPressed), true);
            container.AddHandler(UIElement.PointerReleasedEvent, _handlerReleased = _handlerReleased ?? new PointerEventHandler(OnPointerReleased), true);
            container.AddHandler(UIElement.PointerExitedEvent, _handlerExited = _handlerExited ?? new PointerEventHandler(OnPointerExited), true);
        }

        public void ElementClearing(UIElement container)
        {
            if (_handlerPressed == null || _handlerReleased == null || _handlerExited == null)
            {
                return;
            }

            container.RemoveHandler(UIElement.PointerPressedEvent, _handlerPressed);
            container.RemoveHandler(UIElement.PointerReleasedEvent, _handlerReleased);
            container.RemoveHandler(UIElement.PointerExitedEvent, _handlerExited);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _element = sender as FrameworkElement;
            _pointer = e.Pointer;

            _popupContent = ItemFromContainer(_element);
            _throttler.Stop();
            _throttler.Start();
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _popupContent = null;
            _throttler.Stop();

            if (_popupHost.IsOpen)
            {
                _popupHost.IsOpen = false;

                Closing?.Invoke();
                e.Handled = true;
            }
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            _throttler.Stop();
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_popupHost.IsOpen)
            {
                var pointer = e.GetCurrentPoint(Window.Current.Content);
                var children = VisualTreeHelper.FindElementsInHostCoordinates(pointer.Position, _listView);

                var container = children?.FirstOrDefault(x => x is Button) as Button;
                if (container != null)
                {
                    var content = ItemFromContainer(container);
                    if (content is Sticker sticker && _popupContent != content)
                    {
                        _popupPanel.SetSticker(sticker);
                    }
                    else if (content is Animation animation && _popupContent != content)
                    {
                        _popupPanel.SetAnimation(animation);
                    }

                    _popupContent = content;
                }
            }
        }

        private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            _throttler.Stop();

            if (_popupHost.IsOpen)
            {
                _popupHost.IsOpen = false;

                Closing?.Invoke();
                e.Handled = true;
            }
        }

        private object ItemFromContainer(FrameworkElement container)
        {
            return GetContent(container.DataContext);
        }

        private object GetContent(object content)
        {
            if (content is StickerViewModel stickerViewModel)
            {
                return (Sticker)stickerViewModel;
            }
            else if (content is InlineQueryResultAnimation resultAnimation)
            {
                return resultAnimation.Animation;
            }
            else if (content is InlineQueryResultSticker resultSticker)
            {
                return resultSticker.Sticker;
            }
            else if (content is Sticker || content is Animation)
            {
                return content;
            }

            return null;
        }

        private void DoSomething(object item)
        {
            var content = GetContent(item);
            if (content == null)
            {
                return;
            }

            //if (_pointer != null)
            //{
            //    _listView.CapturePointer(_pointer);
            //    _pointer = null;
            //}

            if (_pointer != null)
            {
                _element.ReleasePointerCapture(_pointer);
                _listView.CapturePointer(_pointer);
                _pointer = null;
            }

            if (_element != null)
            {
                VisualStateManager.GoToState(_element as Control, "Normal", false);
                _element = null;
            }

            if (content is Sticker sticker)
            {
                _popupPanel.SetSticker(sticker);
            }
            else if (content is Animation animation)
            {
                _popupPanel.SetAnimation(animation);
            }

            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            if (bounds != Window.Current.Bounds)
            {
                _popupPanel.Margin = new Thickness(bounds.X, bounds.Y, Window.Current.Bounds.Width - bounds.Right, Window.Current.Bounds.Height - bounds.Bottom);
            }
            else
            {
                _popupPanel.Margin = new Thickness();
            }

            //if (item is TLDocument content && content.StickerSet != null)
            //{
            //    Debug.WriteLine(string.Join(" ", UnigramContainer.Current.ResolveType<IStickersService>().GetEmojiForSticker(content.Id)));
            //}

            Opening?.Invoke();

            _popupPanel.Width = bounds.Width;
            _popupPanel.Height = bounds.Height;
            _popupContent = content;
            _popupHost.IsOpen = true;

            //_scrollingHost.CancelDirectManipulations();
        }
    }
}
