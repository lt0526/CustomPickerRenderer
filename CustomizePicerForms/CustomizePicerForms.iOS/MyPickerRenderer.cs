using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CoreGraphics;
using CustomizePicerForms;
using CustomizePicerForms.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

[assembly: ExportRenderer(typeof(MyPicker), typeof(MyPickerRenderer))]
namespace CustomizePicerForms.iOS
{
    public class MyPickerRenderer : PickerRenderer
    {
        UIPickerView pickerView;
        UIColor _defaultTextColor;
        bool _disposed;

        IElementController ElementController => Element as IElementController;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);
          
            var width = UIScreen.MainScreen.Bounds.Width;

            var entry = Control;

            entry.EditingDidBegin += OnStarted;
            entry.EditingDidEnd += OnEnded;
            entry.EditingChanged += OnEditing;

            pickerView = new UIPickerView();
            pickerView.Frame = new CGRect(0, 0, width, 320);

            var accessoryView = new UIView(new CGRect(0, 0, width, 144));
            UIButton btn = new UIButton(UIButtonType.System);
            btn.SetTitle("Done", UIControlState.Normal);
            btn.AddTarget((sender, args) =>
            {
                var s = (PickerSource)pickerView.Model;
                if (s.SelectedIndex == -1 && Element.Items != null && Element.Items.Count > 0)
                    UpdatePickerSelectedIndex(0);
                UpdatePickerFromModel(s);

                entry.ResignFirstResponder();
            }, UIControlEvent.TouchUpInside);
            accessoryView.AddSubview(btn);
            btn.TranslatesAutoresizingMaskIntoConstraints = false;
            accessoryView.AddConstraint(NSLayoutConstraint.Create(btn, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, accessoryView, NSLayoutAttribute.Trailing, 1, -10));
            accessoryView.AddConstraint(NSLayoutConstraint.Create(btn, NSLayoutAttribute.Top, NSLayoutRelation.Equal, accessoryView, NSLayoutAttribute.Top, 1, 5));


            entry.InputView = pickerView;
            entry.InputAccessoryView = accessoryView;

            _defaultTextColor = entry.TextColor;

            pickerView.Model = new PickerSource(this);

            UpdatePicker();
            UpdateTextColor();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Xamarin.Forms.Picker.TitleProperty.PropertyName)
                UpdatePicker();
            else if (e.PropertyName == Xamarin.Forms.Picker.SelectedIndexProperty.PropertyName)
                UpdatePicker();
            else if (e.PropertyName == Xamarin.Forms.Picker.TextColorProperty.PropertyName || e.PropertyName == Xamarin.Forms.VisualElement.IsEnabledProperty.PropertyName)
                UpdateTextColor();
           
        }

        void OnEditing(object sender, EventArgs eventArgs)
        {
            var selectedIndex = Element.SelectedIndex;
            var items = Element.Items;
            Control.Text = selectedIndex == -1 || items == null ? "" : items[selectedIndex];
        }

        void OnEnded(object sender, EventArgs eventArgs)
        {
            var s = (PickerSource)pickerView.Model;
            if (s.SelectedIndex == -1)
                return;
            if (s.SelectedIndex != pickerView.SelectedRowInComponent(0))
            {
                pickerView.Select(s.SelectedIndex, 0, false);
            }
            ElementController.SetValueFromRenderer(Xamarin.Forms.VisualElement.IsFocusedPropertyKey, false);
        }

        void OnStarted(object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer(Xamarin.Forms.VisualElement.IsFocusedPropertyKey, true);
        }

        void RowsCollectionChanged(object sender, EventArgs e)
        {
            UpdatePicker();
        }

        void UpdatePicker()
        {
            var selectedIndex = Element.SelectedIndex;
            var items = Element.Items;
            Control.Placeholder = Element.Title;
            var oldText = Control.Text;
            Control.Text = selectedIndex == -1 || items == null ? "" : items[selectedIndex];
            UpdatePickerNativeSize(oldText);
            pickerView.ReloadAllComponents();
            if (items == null || items.Count == 0)
                return;

            UpdatePickerSelectedIndex(selectedIndex);
        }

        void UpdatePickerFromModel(PickerSource s)
        {
            if (Element != null)
            {
                var oldText = Control.Text;
                Control.Text = s.SelectedItem;
                UpdatePickerNativeSize(oldText);
                ElementController.SetValueFromRenderer(Xamarin.Forms.Picker.SelectedIndexProperty, s.SelectedIndex);
            }
        }

        void UpdatePickerNativeSize(string oldText)
        {
            if (oldText != Control.Text)
                ((IVisualElementController)Element).NativeSizeChanged();
        }

        void UpdatePickerSelectedIndex(int formsIndex)
        {
            var source = (PickerSource)pickerView.Model;
            source.SelectedIndex = formsIndex;
            source.SelectedItem = formsIndex >= 0 ? Element.Items[formsIndex] : null;
            pickerView.Select(Math.Max(formsIndex, 0), 0, true);
        }

        void UpdateTextColor()
        {
            var textColor = Element.TextColor;

            if (textColor.IsDefault || !Element.IsEnabled)
                Control.TextColor = _defaultTextColor;
            else
                Control.TextColor = textColor.ToUIColor();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                _defaultTextColor = null;

                if (pickerView != null)
                {
                    if (pickerView.Model != null)
                    {
                        pickerView.Model.Dispose();
                        pickerView.Model = null;
                    }

                    pickerView.RemoveFromSuperview();
                    pickerView.Dispose();
                    pickerView = null;
                }

                if (Control != null)
                {
                    Control.EditingDidBegin -= OnStarted;
                    Control.EditingDidEnd -= OnEnded;
                    Control.EditingChanged -= OnEditing;
                }

                if (Element != null)
                    ((INotifyCollectionChanged)Element.Items).CollectionChanged -= RowsCollectionChanged;
            }

            base.Dispose(disposing);
        }


        class PickerSource : UIPickerViewModel
        {
            //Define the Font size or style
            //public override NSAttributedString GetAttributedTitle(UIPickerView pickerView, nint row, nint component)
            //{
            //    var text = new NSAttributedString(
            //        itemList[(int)row],
            //        font: UIFont.SystemFontOfSize(12),
            //        foregroundColor: UIColor.Black,
            //        strokeWidth: 1
            //    );

            //    return text;
            //}

            //Define the row height
            public override nfloat GetRowHeight(UIPickerView pickerView, nint component)
            {
                return 20;
            }

            //Customize more flexible (including placement) use following method:

            public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
            {
                UIView contentView = new UIView(new CGRect(
                    0, 0, UIScreen.MainScreen.Bounds.Size.Width, UIScreen.MainScreen.Bounds.Size.Height));

                UILabel label = new UILabel();
                label.Frame = contentView.Bounds;
                contentView.AddSubview(label);

                label.Text = _renderer.Element.Items[(int)row];
                //Change the label style
                label.Font = UIFont.SystemFontOfSize(12);
                label.TextColor = UIColor.Black;
                label.TextAlignment = UIKit.UITextAlignment.Center;

                return contentView;
            }


            MyPickerRenderer _renderer;
            bool _disposed;

            public PickerSource(MyPickerRenderer renderer)
            {
                _renderer = renderer;
            }

            public int SelectedIndex { get; internal set; }

            public string SelectedItem { get; internal set; }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 1;
            }
            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                return _renderer.Element.Items != null ? _renderer.Element.Items.Count : 0;
            }

            //public override string GetTitle(UIPickerView picker, nint row, nint component)
            //{
            //    return _renderer.Element.Items[(int)row];
            //}

            public override void Selected(UIPickerView picker, nint row, nint component)
            {
                if (_renderer.Element.Items.Count == 0)
                {
                    SelectedItem = null;
                    SelectedIndex = -1;
                }
                else
                {
                    SelectedItem = _renderer.Element.Items[(int)row];
                    SelectedIndex = (int)row;
                }

                if (_renderer.Element.On<Xamarin.Forms.PlatformConfiguration.iOS>().UpdateMode() == UpdateMode.Immediately)
                    _renderer.UpdatePickerFromModel(this);
            }

            protected override void Dispose(bool disposing)
            {
                if (_disposed)
                    return;

                _disposed = true;

                if (disposing)
                    _renderer = null;

                base.Dispose(disposing);
            }
        }
    }

   
}