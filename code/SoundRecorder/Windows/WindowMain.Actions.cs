/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Windows {
    using System.Windows.Media;
    using System.IO;
    using Environment = System.Environment;
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
    using Visibility = System.Windows.Visibility;
    using UIElement = System.Windows.UIElement;

    public partial class WindowMain {

        class StateDefinitionSet {
            internal const string DefaultExtension = "wav";
            internal const string FileNumberFormat = "{{0:D{0}}}";
            internal const string DefaultBaseFileName = "SoundRecorder-";
            internal const string DoPause = "\u275A\u275A Paus_e";
            internal const string DoResume = "\u25BC Resum_e";
            internal const string DoRecord = "\u25BC Rec_ord";
            internal const string DoStop = "\u25A0 St_op";
            internal const string BaseFileNameDialogTitle = "Base File Name for Output Files";
            internal static readonly Brush runningBrush = Brushes.Navy;
            internal static readonly Brush waitingBrush = Brushes.DarkRed;
        } //class StateDefinitionSet

        ulong currentName = 1;
        readonly SaveFileDialog dialog = new();

        enum State { Waiting, Paused, Recording, }
        // next two fields define state:
        State state = State.Waiting;
        string buttonAdjustIndicatorInitialText;
        bool hasData;

        void StartRecording() {
            buttonAdjustIndicator.Visibility = Visibility.Hidden;
            indicatorThreadWrapper.StartIndicatorLevelTest(volumeIndicator.Maximum);
            Wave.Mci.Close();
            Wave.Mci.Open();
            watchBox.Start();
            Wave.Mci.Record();
            UpdateState(State.Recording, true);
            buttonSave.Focus();
        } //StartRecord

        void StopRecording(bool firstTime) {
            if (!firstTime) {
                buttonAdjustIndicator.Content = indicatorThreadWrapper.AdjustButtonText(buttonAdjustIndicatorInitialText);
                buttonAdjustIndicator.Visibility = Visibility.Visible;
                watchBox.Pause();
                indicatorThreadWrapper.StopIndicatorLevelTest(); //SA???
            } //if
            Wave.Mci.Stop();
            UpdateState(State.Waiting, true);
        } //StopRecording

        void SetButtons() {
            buttonAdjustIndicatorInitialText = buttonAdjustIndicator.Content.ToString();
            for (int width = 2; width < 10; ++width)
                comboBoxWidth.Items.Add(width);
            StopRecording(true);
            UpdateState(State.Waiting, false);
            void showNext() {
                string fileName = GetNextFileName();
                this.textBoxNext.Text = Path.GetFileName(fileName);
            } //showNext
            this.textBoxBaseFileName.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), StateDefinitionSet.DefaultBaseFileName);
            showNext();
            dialog.Title = StateDefinitionSet.BaseFileNameDialogTitle;
            dialog.DefaultExt = StateDefinitionSet.DefaultExtension;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;
            dialog.OverwritePrompt = false;
            this.buttonRecordStop.Click += (sender, eventArgs) => {
                if (state == State.Recording || state == State.Paused)
                    StopRecording(false);
                else
                    StartRecording();
            }; //this.buttonRecord.Click
            this.buttonPauseResume.Click += (sender, eventArgs) => {
                if (state == State.Recording) {
                    watchBox.Pause();
                    Wave.Mci.Pause();
                    UpdateState(State.Paused, true);
                } else if (state == State.Paused) {
                    watchBox.Resume();
                    UpdateState(State.Recording, true);
                    Wave.Mci.Record();
                }
                ;
                activityIndicator.IsActive = state == State.Recording;
            }; //buttonPauseResume.Click
            this.buttonResetNumbering.Click += (sender, eventArgs) => {
                currentName = 1;
                showNext();
            }; //buttonResetNumbering.Click
            this.buttonSave.Click += (sender, eventArgs) => {
                string fileName = GetNextFileName();
                Wave.Mci.Stop();
                Wave.Mci.SaveRecording(fileName);
                this.textBoxWritten.Text = Path.GetFileName(fileName);
                currentName++;
                showNext();
                if (checkBoxAutoRestart.IsChecked == true)
                    StartRecording();
                else
                    StopRecording(false);
            }; //this.buttonSave
            this.buttonBaseFileNameDialog.Click += (sender, eventArgs) => {
                if (this.textBoxBaseFileName.Text.Trim().Length > 0)
                    dialog.InitialDirectory = Path.GetDirectoryName(this.textBoxBaseFileName.Text);
                if (dialog.ShowDialog() == true)
                    this.textBoxBaseFileName.Text = dialog.FileName;
            }; //buttonBaseFileNameDialog.Click
            buttonAdjustIndicator.Click += (sender, eventArgs) => {
                (sender as UIElement).Visibility = Visibility.Hidden;
                indicatorThreadWrapper.AdjustIndicatorValue();
            }; //buttonAdjustIndicator.Click
        } // SetButtons

        string GetNextFileName() {
            string name = string.Empty;
            for (ulong nameIndex = currentName; nameIndex <= ulong.MaxValue; ++nameIndex) {
                string baseFileName = this.textBoxBaseFileName.Text;
                string ext = Path.GetExtension(baseFileName);
                string path = Path.GetDirectoryName(baseFileName);
                name = Path.GetFileNameWithoutExtension(baseFileName);
                string numberFormat = string.Format(StateDefinitionSet.FileNumberFormat, comboBoxWidth.SelectedItem);
                name += string.Format(numberFormat, nameIndex);
                name = Path.Combine(path, name);
                name = Path.ChangeExtension(name, StateDefinitionSet.DefaultExtension);
                if (File.Exists(name))
                    continue;
                else
                    return name;
            } //loop
            return null;
        } //GetNextFileName

        void UpdateState(State newState, bool dataAvailabity) {
            this.state = newState;
            this.hasData = dataAvailabity;
            activityIndicator.IsActive = state == State.Recording;
            if (this.state == State.Waiting) {
                buttonRecordStop.Foreground = StateDefinitionSet.waitingBrush;
                buttonRecordStop.Content = StateDefinitionSet.DoRecord;
            } else {
                buttonRecordStop.Foreground = StateDefinitionSet.runningBrush;
                buttonRecordStop.Content = StateDefinitionSet.DoStop;
            } //if
            buttonSave.IsEnabled = hasData;
            buttonPauseResume.IsEnabled = this.state != State.Waiting;
            if ((!buttonPauseResume.IsEnabled) || this.state == State.Recording)
                buttonPauseResume.Content = StateDefinitionSet.DoPause;
            else
                buttonPauseResume.Content = StateDefinitionSet.DoResume;
        } //UpdateState

    } //class WindowMain

} //namespace SoundRecorder.Windows
