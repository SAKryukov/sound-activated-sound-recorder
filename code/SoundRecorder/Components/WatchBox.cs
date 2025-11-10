/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Components {
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;
    using Stopwatch = System.Diagnostics.Stopwatch;
    using TimeSpan = System.TimeSpan;

    public class WatchBox : TextBox {

        static class DefinitionSet {
            internal const string Format = "{0:d2}:{1:d2}:{2:d2}";
            internal static Brush DefaultForegrond = Brushes.Navy;
        } //DefinitionSet

        public WatchBox() {
            TextAlignment = TextAlignment.Left;
            Foreground = DefinitionSet.DefaultForegrond;
            Refresh();
        } //WatchBox

        internal void Start() {
            stopwatch.Reset();
            stopwatch.Start();
            Refresh();
        } //Start
        internal void Pause() {
            stopwatch.Stop();
            Refresh();
        } //Pause
        internal void Resume() {
            stopwatch.Start();
            Refresh();
        } //Resume

        internal void Refresh() {
            TimeSpan elapsed = stopwatch.Elapsed;
            this.Text = string.Format(DefinitionSet.Format, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
        } //Refresh

        readonly Stopwatch stopwatch = new();

    } //WatchBox

} //namespace SoundRecorder.Components
