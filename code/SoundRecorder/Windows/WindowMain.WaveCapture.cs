    /*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Windows {
    using System;
    using System.Windows.Media;
    using System.Threading;
    using System.Windows.Threading;

    public partial class WindowMain {

        static class DefinitionSet {
            internal const int defaultMaximumLevel = 128; //why?
            internal const int getLevelRepeatCount = 3;
            internal const int getLevelAverageSleepMS = 1;
            internal const int getLevelRepeatSleepMS = 1;
            internal static string AdjustButtonText(string initialText, double ratio) =>
                $"{initialText} {Math.Round(ratio * 1000) / 10}%";
        } //DefinitionSet

        class VolumeIndicatorEventArgs : EventArgs {
            internal VolumeIndicatorEventArgs(double volume, bool maximumReached, double newMaximum = double.NaN) {
                Volume = volume;
                MaximumReached = maximumReached;
                NewMaximum = newMaximum;
            } //VolumeIndicatorEventArgs
            internal double Volume { get; private set; }
            internal bool MaximumReached { get; private set; }
            internal double NewMaximum { get; private set; }
        } //class VolumeIndicatorEventArgs

        class IndicatorThreadWrapper {
            internal IndicatorThreadWrapper() {
                thread = new Thread(Body);
            } //IndicatorThreadWrapper
            internal void Start(double initialMaximum) {
                existingMaximum = initialMaximum;
                thread.Start();
            } //Start
            internal void Stop() { lock (lockObject) exit = true; }
            internal void AdjustIndicatorValue() {
                double newMaximumValue;
                lock (lockObject)
                    newMaximumValue = newMaximum;
                VolumeLevelMeasured.Invoke(this, new VolumeIndicatorEventArgs(double.NaN, false, newMaximumValue));
            } //AdjustIndicatorValue
            internal string AdjustButtonText(string initialText) {
                double newMaximumValue, existingMaximumValue;
                lock (lockObject) {
                    newMaximumValue = newMaximum;
                    existingMaximumValue = existingMaximum;
                } //lock
                return DefinitionSet.AdjustButtonText(initialText, newMaximumValue/existingMaximumValue);
            } //AdjustButtonText
            internal void StartIndicatorLevelTest(double existingMaximum) {
                lock (lockObject) {
                    newMaximum = double.NegativeInfinity;
                    this.existingMaximum = existingMaximum;
                    measuringMaximumLevel = true;
                } //lock
            } //StartIndicatorLevelText
            internal void StopIndicatorLevelTest() {
                lock (lockObject) measuringMaximumLevel = false;
            } //StopIndicatorLevelText
            internal EventHandler<VolumeIndicatorEventArgs> VolumeLevelMeasured;
            void Body() {
                bool mustExit() { lock (lockObject) return exit; }
                Wave.Mci.StartLevelMeter();
                while (!mustExit()) {
                    if (VolumeLevelMeasured != null) {
                        (double level, double singleMaximum) =
                            Wave.Mci.GetLevel(DefinitionSet.getLevelRepeatCount, DefinitionSet.getLevelAverageSleepMS);
                        if (measuringMaximumLevel && singleMaximum > newMaximum)
                            newMaximum = singleMaximum;
                        bool maximumReached = level >= existingMaximum;
                        VolumeLevelMeasured.Invoke(this, new VolumeIndicatorEventArgs(level, maximumReached));
                    } //if
                    Thread.Sleep(DefinitionSet.getLevelRepeatSleepMS);
                } //loop
            } //Body
            double newMaximum = double.NegativeInfinity;
            double existingMaximum = double.NegativeInfinity;
            bool measuringMaximumLevel, exit;
            readonly Thread thread;
            readonly Lock lockObject = new();
        } //class IndicatorThreadWrapper

        void SetupIndicator() {
            indicatorThreadWrapper.VolumeLevelMeasured += (sender, eventArgs) => {
                Dispatcher.Invoke(new Action<WindowMain>(wnd => {
                    if (!double.IsNaN(eventArgs.NewMaximum))
                        wnd.volumeIndicator.Maximum = eventArgs.NewMaximum;
                    if (double.IsNaN(eventArgs.Volume))
                        return;
                    wnd.volumeIndicator.Value = eventArgs.Volume;
                    if (eventArgs.MaximumReached)
                        wnd.volumeIndicator.Foreground = overflowIndicator;
                    else
                        wnd.volumeIndicator.Foreground = normalIndicator;
                    if (wnd.checkBoxUseSoundActivation.IsChecked == true && wnd.state == State.Waiting && eventArgs.Volume > wnd.volumeIndicator.Threshold) {
                        if (int.TryParse(wnd.textBoxDelay.Text, out int delay))
                            Thread.Sleep(delay);
                        wnd.StartRecording();
                    } //if
                }), this);
            }; //indicatorThreadWrapper.VolumeLevelMeasured
            overflowIndicator = this.volumeIndicator.Background;
            volumeIndicator.Background = Brushes.Transparent;
            normalIndicator = this.volumeIndicator.Foreground;
            DispatcherTimer recordingBlinkingTimer = new() {
                Interval = new TimeSpan(0, 0, 0, 0, 280)
            };
            recordingBlinkingTimer.Tick += (sender, eventArgs) => { this.activityIndicator.Flash(); };
            recordingBlinkingTimer.Start();
            DispatcherTimer watchTimer = new() {
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };
            watchTimer.Tick += (sender, eventArgs) => { this.watchBox.Refresh(); };
            watchTimer.Start();
        } //SetupCapture

        protected override void OnContentRendered(EventArgs e) {
            LoadDefaultPreferences();
            base.OnContentRendered(e);
            MinHeight = MaxHeight = ActualHeight;
            MinWidth = ActualWidth;
            indicatorThreadWrapper.Start(volumeIndicator.Maximum);
        } //OnContentRendered

        protected override void OnClosed(EventArgs e) {
            indicatorThreadWrapper.Stop();
            Wave.Mci.Close();
            base.OnClosed(e);
        } //OnClosed

        Brush normalIndicator, overflowIndicator;
        readonly IndicatorThreadWrapper indicatorThreadWrapper = new();

    } //class WindowMain

} //namespace SoundRecorder.Windows
