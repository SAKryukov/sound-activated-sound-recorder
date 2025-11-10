/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Windows {
    using System.Windows;
    using System.Windows.Threading;
    using Action = System.Action;

    public partial class WindowMain : Window {

        public WindowMain() {
            InitializeComponent();
            volumeIndicator.Maximum = DefinitionSet.defaultMaximumLevel;
            help.ShowActivated = true;
            Title = SoundRecorder.Application.SoundRecorderApplication.Current.ProductName;
            SetupIndicator();
            SetButtons();
            SetupMenu();
            void afterExpandingCollapsing() {
                MinHeight = 0; MaxHeight = int.MaxValue;
                this.SizeToContent = SizeToContent.Manual;
                this.SizeToContent = SizeToContent.Height;
                Dispatcher.BeginInvoke(new Action(() => {
                    MinHeight = MaxHeight = ActualHeight;
                }), DispatcherPriority.Input);
            } //afterExpandingCollapsing
            this.expanderOutput.Expanded += (sender, eventArgs) => { afterExpandingCollapsing(); };
            this.expanderOutput.Collapsed += (sender, eventArgs) => { afterExpandingCollapsing(); };
            this.expanderActivation.Expanded += (sender, eventArgs) => { afterExpandingCollapsing(); };
            this.expanderActivation.Collapsed += (sender, eventArgs) => { afterExpandingCollapsing(); };
            textBoxDelay.PreviewTextInput += (sender, eventArgs) => { eventArgs.Handled = !char.IsDigit(eventArgs.Text[0]); };
            this.menuItemAbout.Click += (sender, eventArgs) => about.ShowAbout(this);
        } //WindowMain

        readonly WindowAbout about = new();
        readonly WindowHelp help = new();
        readonly Window hiddenOwner = new();

    } //class WindowMain

} //namespace SoundRecorder.Windows
