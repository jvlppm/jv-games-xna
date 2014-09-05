namespace Sample.XForms
{
    using System;
    using Xamarin.Forms;

    public class ImageButton : View
    {
        public static BindableProperty ImageProperty = BindableProperty.Create<ImageButton, string>(p => p.Image, defaultValue: null);

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public event EventHandler OnClick;

        public void FireClicked()
        {
            if (OnClick != null)
                OnClick(this, EventArgs.Empty);
        }
    }
}

