using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace SmartThermo.Modules.Dialog.SettingsSensor.Behaviors
{
    public class ListBoxBehavior : Behavior<ListBox>
    {
        private ScrollViewer _scrollViewer;
        private bool _autoScroll = true;

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
            AssociatedObject.Unloaded += AssociatedObjectOnUnloaded;
        }

        private void AssociatedObjectOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
            AssociatedObject.ItemContainerGenerator.ItemsChanged -= ItemContainerGeneratorItemsChanged;

            _scrollViewer = null;
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _scrollViewer = GetScrollViewer(AssociatedObject);
            if (_scrollViewer == null)
                return;

            _scrollViewer.ScrollChanged += ScrollViewerOnScrollChanged;

            AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
            AssociatedObject.ItemContainerGenerator.ItemsChanged += ItemContainerGeneratorItemsChanged;
        }

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Math.Abs(e.VerticalOffset - (e.ExtentHeight - e.ViewportHeight)) <= 0)
                _autoScroll = true;
        }

        private static ScrollViewer GetScrollViewer(DependencyObject root)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < childCount;)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                return child is ScrollViewer sv ? sv : GetScrollViewer(child);
            }
            return null;
        }

        private void ItemContainerGeneratorItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add &&
                e.Action != NotifyCollectionChangedAction.Reset)
                return;

            if (_autoScroll)
                _scrollViewer.ScrollToBottom();
        }

        private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            _autoScroll = false;
        }
    }
}
