/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Windows {
    using System.Windows;
    using Application;
    using Version = System.Version;

    public partial class WindowAbout : Window {

        public WindowAbout() {
            InitializeComponent();
            SoundRecorderApplication app = SoundRecorderApplication.Current;
            Version version = app.AssemblyVersion;
            Title = $"About {app.ProductName}";
            string thinSpace = char.ConvertFromUtf32(0x2009);
            this.textBlockProduct.Text = $"{app.ProductName} v.{thinSpace}{version.Major}.{version.Minor}";
            this.texBlockCopyright.Text = app.Copyright;
            this.buttonOk.Click += (sender, eventArgs) => { Close(); };
        } //WindowAbout

        internal void ShowAbout(Window owner) {
            Owner = owner;
            Show();
        } //ShowAbout

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            Hide();
            e.Cancel = true;
        } //OnClosing

    } //class WindowAbout

} //namespace SoundRecorder.Windows
