/*
Sound Recorder

Copyright (C) by Sergey A Kryukov, 2014, 2025

https://www.SAKryukov.org
https://sakryukov.github.io/publications/2014-09-03.Practical-Sound-Recorder-with-Sound-Activation.html
*/

namespace SoundRecorder.Resources {
    public partial class HelpSource : System.Windows.FrameworkContentElement {
        public HelpSource() {
            InitializeComponent();
        }
        internal string HelpHTML => (string)Resources[typeof(string)];

    } //class HelpSource
}
