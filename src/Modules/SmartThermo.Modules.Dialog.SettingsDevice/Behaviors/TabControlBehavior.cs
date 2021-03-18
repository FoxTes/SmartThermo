using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SmartThermo.Modules.Dialog.SettingsDevice.Behaviors
{
    public class TabControlBehavior : Behavior<TabControl>
    {
        private TabControl AssociatedTabControl { get { return AssociatedObject; } }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedTabControl.Loaded += AssociatedTabControl_Loaded;
        }

        private void AssociatedTabControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is TabControl tabControl))
                return;

            double contentWidth = ((FrameworkElement)((TabItem)tabControl.SelectedItem).Content).ActualWidth;
            double max = 0;

            foreach (TabItem tab in tabControl.Items)
            {
                ((FrameworkElement)tab.Content).Measure(new Size(contentWidth, double.PositiveInfinity));
                max = Math.Max(((FrameworkElement)tab.Content).DesiredSize.Height, max);
            }
            foreach (TabItem tab in tabControl.Items)
                ((FrameworkElement)tab.Content).Height = max;

            AssociatedTabControl.Loaded -= AssociatedTabControl_Loaded;
        }
    }
}
