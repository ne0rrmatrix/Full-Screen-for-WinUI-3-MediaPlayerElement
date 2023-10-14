using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace App1;
public sealed class CustomTransportControls : MediaTransportControls
{
    public event EventHandler<EventArgs> FullScreen;

    public CustomTransportControls()
    {
        this.DefaultStyleKey = typeof(CustomTransportControls);
    }

    protected override void OnApplyTemplate()
    {
        // This is where you would get your custom button and create an event handler for its click method.
        Button fsButton = GetTemplateChild("fullscreen") as Button;
        fsButton.Click += FullScreenButton_Click;

        base.OnApplyTemplate();
    }

    private void FullScreenButton_Click(object sender, RoutedEventArgs e)
    {
        // Raise an event on the custom control when 'like' is clicked
        FullScreen?.Invoke(this, EventArgs.Empty);
    }
}
