﻿using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

using System.ComponentModel;

namespace TestAutoCompleteBehavior.Behaviors;

public class AutoCompleteZeroMinimumPrefixLengthDropdownBehavior : Behavior<AutoCompleteBox>
{
    protected override void OnAttached()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.KeyUp += OnKeyUp;
            AssociatedObject.DropDownOpening += DropDownOpening;
            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.PointerReleased += PointerReleased;
        }

        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.KeyUp -= OnKeyUp;
            AssociatedObject.DropDownOpening -= DropDownOpening;
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.PointerReleased -= PointerReleased;
        }

        base.OnDetaching();
    }

    //have to use KeyUp as AutoCompleteBox eats some of the KeyDown events
    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down || e.Key == Key.F4)
        {
            if (string.IsNullOrEmpty(AssociatedObject?.Text))
                ShowDropdown();
        }
    }

    private void DropDownOpening(object? sender, CancelEventArgs e)
    {
        var prop = AssociatedObject?.GetType().GetProperty("TextBox", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var tb = (TextBox?)prop?.GetValue(AssociatedObject);
        if (tb is not null && tb.IsReadOnly)
        {
            e.Cancel = true;
            return;
        }
    }

    private void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (string.IsNullOrEmpty(AssociatedObject?.Text))
            ShowDropdown();
    }

    private void ShowDropdown()
    {
        if (AssociatedObject is not null && !AssociatedObject.IsDropDownOpen)
        {
            typeof(AutoCompleteBox).GetMethod("PopulateDropDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(AssociatedObject, [AssociatedObject, EventArgs.Empty]);
            typeof(AutoCompleteBox).GetMethod("OpeningDropDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(AssociatedObject, [false]);

            if (!AssociatedObject.IsDropDownOpen)
            {
                //We *must* set the field and not the property as we need to avoid the changed event being raised (which prevents the dropdown opening).
                var ipc = typeof(AutoCompleteBox).GetField("_ignorePropertyChange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if ((bool?)ipc?.GetValue(AssociatedObject) == false)
                    ipc?.SetValue(AssociatedObject, true);

                AssociatedObject.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, true);
            }
        }
    }

    private void OnGotFocus(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject != null)
            ShowDropdown();
    }
}
