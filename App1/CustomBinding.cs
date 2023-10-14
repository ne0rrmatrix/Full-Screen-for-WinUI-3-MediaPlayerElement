using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System.Collections.Generic;
using System.Diagnostics;

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
            RestoreAllElements();
           
        }
        else
        {
            _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            GetAllElements(element);
            SetallElementsMaxSize();
        }
    }
    private void GetAllElements(FrameworkElement element)
    {
        ElementData temp = new()
        {
            Element = element,
            Width = GetWidth(element),
            Height = GetHeight(element),
            Thickness = new Thickness(element.Margin.Left, element.Margin.Top, element.Margin.Right, element.Margin.Bottom)
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
    private static void SetItemSize(FrameworkElement frameworkElement, double width, double height, Thickness thickness)
    {
        SetItems(frameworkElement, width, height);
        BindWidth(frameworkElement, frameworkElement);
        BindHeight(frameworkElement, frameworkElement);
        frameworkElement.Margin = new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
    }
    private void SetallElementsMaxSize()
    {
        Element.ForEach(x => SetItemSize(x.Element, double.NaN, double.NaN, new Thickness(0,0,0,0)));
    }
    private void RestoreAllElements()
    {
        Element.ForEach(x => SetItemSize(x.Element, x.Width, x.Height, x.Thickness));
        Element.ForEach(x => Debug.WriteLine(x.Width + " " + x.Height));
    }
}
