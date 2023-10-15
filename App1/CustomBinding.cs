using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;

namespace App1;
public class CustomBinding
{
    private readonly AppWindow _appWindow;
    public List<ElementData> Element { get; set; } = new();
    public CustomBinding(AppWindow appWindow)
    {
        _appWindow = appWindow;
    }
    public void SetFullScreen(FrameworkElement element)
    {
        if (_appWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
        {
            _appWindow.SetPresenter(AppWindowPresenterKind.Default);
            Element.ForEach(x => SetItemSize(x.Element, x.Width, x.Height, x.Thickness, true));
            Element.Clear();
        }
        else
        {
            _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            GetAllElements(element);
            Element.ForEach(x => SetItemSize(x.Element, double.NaN, double.NaN, new Thickness(0, 0, 0, 0), false));
        }
    }
    private static bool MediaElementIsDescendant(FrameworkElement element)
    {
        if (element is null)
        {
            Debug.WriteLine("Exiting recursiion");
            return false;
        }    
        for(var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            var child = VisualTreeHelper.GetChild(element, i) ?? null;
            if (child is null)
            {
                return false;
            }
            var type = child.GetType();
            if (type == typeof(MediaPlayerElement))
            {
                return true;
            }
            var item = MediaElementIsDescendant(child as FrameworkElement);
            if (item)
            {
                return true;
            }
        }
        return false;
    }
    private static void SetAllChildrenVisibility(FrameworkElement view, bool visible)
    {
        view = view.Parent as FrameworkElement ?? null;
        if (view is null)
        {
            return;
        }
        for(var i = 0; i < VisualTreeHelper.GetChildrenCount(view); i++)
        {
            var child = VisualTreeHelper.GetChild(view, i);
            if (child is not FrameworkElement item)
            {
                return;
            }
            if (visible)
            {
                item.Visibility = Visibility.Visible;
            }
            else if (item.GetType() != typeof(MediaPlayerElement) && !MediaElementIsDescendant(item))
            {
                item.Visibility = Visibility.Collapsed;
            }
        }
    }
    private void GetAllElements(FrameworkElement element)
    {
        ElementData temp = new()
        {
            Element = element,
            Thickness = new Thickness(element.Margin.Left, element.Margin.Top, element.Margin.Right, element.Margin.Bottom),
            Width = GetWidth(element),
            Height = GetHeight(element)
        };
        Element.Add(temp);
        while (true)
        {
            ElementData item = new();
            element = element.Parent as FrameworkElement ?? null;
            if (element is null)
            {
                return;
            }
            item.Element = element;
            item.Thickness = new Thickness(element.Margin.Left, element.Margin.Top, element.Margin.Right, element.Margin.Bottom);
            item.Width = GetWidth(element);
            item.Height = GetHeight(element);
            Element.Add(item);
        }
    }
    private static double GetWidth(FrameworkElement frameworkElement)
    {
        return frameworkElement.ActualWidth;
    }
    private static double GetHeight(FrameworkElement frameworkElement)
    {
        return frameworkElement.ActualHeight;
    }
    private static void BindWidth(FrameworkElement bindMe, FrameworkElement toMe)
    {
        Binding b = new()
        {
            Mode = BindingMode.OneWay,
            Source = toMe.Width
        };
        bindMe.SetBinding(FrameworkElement.WidthProperty, b);
    }
    private static void BindHeight(FrameworkElement bindMe, FrameworkElement toMe)
    {
        Binding b = new()
        {
            Mode = BindingMode.OneWay,
            Source = toMe.Height
        };
        bindMe.SetBinding(FrameworkElement.HeightProperty, b);
    }
    private static void SetItems(FrameworkElement frameworkElement, double width, double height)
    {
        Binding bWidth = new()
        {
            Mode = BindingMode.OneWay,
            Source = width
        };
        Binding bHeight = new()
        {
            Mode = BindingMode.OneWay,
            Source = height
        };
        frameworkElement.SetBinding(FrameworkElement.WidthProperty, bWidth);
        frameworkElement.SetBinding(FrameworkElement.HeightProperty, bHeight);
    }
    private static void SetItemSize(FrameworkElement frameworkElement, double width, double height, Thickness thickness, bool visibility)
    {
        SetItems(frameworkElement, width, height);
        BindWidth(frameworkElement, frameworkElement);
        BindHeight(frameworkElement, frameworkElement);
        frameworkElement.Margin = new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        SetAllChildrenVisibility(frameworkElement, visibility);
    }
}
