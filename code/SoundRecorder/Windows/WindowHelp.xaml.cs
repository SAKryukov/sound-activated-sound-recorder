/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Windows {
    using System;
    using Application;
    using System.Windows;

    public partial class WindowHelp : Window {

        public WindowHelp() {
            InitializeComponent();
            helpContent = new Resources.HelpSource().HelpHTML;
            SoundRecorderApplication app = SoundRecorderApplication.Current;
            Version version = app.AssemblyVersion;
            string thinSpace = char.ConvertFromUtf32(0x2009);
            statusBarTitle.Content = app.ProductName;
            statusVersion.Content = $"v.{thinSpace}{version.Major}.{version.Minor}";
            statusCopyright.Content = app.Copyright;
        } //WindowHelp

        internal void ShowHelp(Window owner) {
            Owner = owner;
            Show();
        } //ShowHelp

        readonly string helpContent;

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            Hide();
            e.Cancel = true;
        } //OnClosing

        protected override void OnContentRendered(EventArgs e) {
            base.OnContentRendered(e);
            browser.NavigateToString(helpContent);
            browser.Focus();
        } //OnContentRendered

    } //WindowHelp

} //namespace SoundRecorder.Windows
