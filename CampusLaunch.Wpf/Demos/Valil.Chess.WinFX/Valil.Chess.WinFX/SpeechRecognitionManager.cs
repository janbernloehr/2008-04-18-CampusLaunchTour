using System;
using System.Speech.Recognition;
using System.ComponentModel;
using System.Windows.Input;
using Valil.Chess.Model;
using System.Windows;

namespace Valil.Chess.WinFX
{
    public class SpeechRecognitionManager : INotifyPropertyChanged
    {
        /// <summary>
        /// The recognizer.
        /// </summary>
        private SpeechRecognizer recognizer;

        /// <summary>
        /// True if speech recognition is available, false otherwise.
        /// </summary>
        private bool isRecognitionAvailable;

        /// <summary>
        /// True if the recognizer is enabled and the recognition is available, false otherwise.
        /// </summary>
        public bool Enabled
        {
            get { return isRecognitionAvailable && recognizer.Enabled; }
            set
            {
                if (isRecognitionAvailable)
                {
                    recognizer.Enabled = value;
                    LastCommandText = value ? "Speech recognition enabled" : "Speech recognition disabled";
                }
                else
                {
                    LastCommandText = "Speech recognition not available";
                }
            }
        }

        /// <summary>
        /// Commands target.
        /// </summary>
        private IInputElement target;

        /// <summary>
        /// Commands target.
        /// </summary>
        public IInputElement Target
        {
            get { return target; }
            set { target = value; }
        }

        /// <summary>
        /// Last command text.
        /// </summary>
        private string lastCommandText;

        /// <summary>
        /// Last command text.
        /// </summary>
        public string LastCommandText
        {
            get { return lastCommandText; }
            private set
            {
                lastCommandText = value;

                OnPropertyChanged("LastCommandText");
            }
        }

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises property changed event.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpeechRecognitionManager()
        {
            try
            {
                recognizer = new SpeechRecognizer();
                recognizer.LoadGrammar(BuildGrammar());

                recognizer.Enabled = false;
                recognizer.SpeechRecognized += recognizer_SpeechRecognized;
                recognizer.SpeechDetected += recognizer_SpeechDetected;
                recognizer.SpeechRecognitionRejected += recognizer_SpeechRecognitionRejected;

                isRecognitionAvailable = true;
            }
            catch
            {
                isRecognitionAvailable = false;
            }
        }

        private void recognizer_SpeechRecognitionRejected(object sender, RecognitionEventArgs e)
        {
            if (target == null) { return; }

            LastCommandText = "???";
        }

        private void recognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            if (target == null) { return; }

            LastCommandText = String.Empty;
        }

        private void recognizer_SpeechRecognized(object sender, RecognitionEventArgs e)
        {
            if (target == null) { return; }

            if (!e.Result.Text.StartsWith("select"))
            {
                // this is a toolbar command
                switch (e.Result.Text)
                {
                    case "new":
                        ApplicationCommands.New.Execute(null, target);
                        break;
                    case "first":
                        CustomCommands.First.Execute(null, target);
                        break;
                    case "previous":
                        CustomCommands.Previous.Execute(null, target);
                        break;
                    case "next":
                        CustomCommands.Next.Execute(null, target);
                        break;
                    case "last":
                        CustomCommands.Last.Execute(null, target);
                        break;
                    case "move":
                        CustomCommands.Move.Execute(null, target);
                        break;
                    case "rotate":
                        CustomCommands.Rotate.Execute(null, target);
                        break;
                }

                LastCommandText = e.Result.Text;
            }
            else
            {
                // this is a select square command

                // get the square position
                char f = e.Result.Text[7];
                char r = '\0';
                switch (e.Result.Text.Substring(9))
                {
                    case "one":
                        r = '1';
                        break;
                    case "two":
                        r = '2';
                        break;
                    case "three":
                        r = '3';
                        break;
                    case "four":
                        r = '4';
                        break;
                    case "five":
                        r = '5';
                        break;
                    case "six":
                        r = '6';
                        break;
                    case "seven":
                        r = '7';
                        break;
                    case "eight":
                        r = '8';
                        break;
                }
                int position = Utils.GetPosition(f, r);

                LastCommandText = "select " + f + r;

                CustomCommands.Select.Execute(position, target);
            }
        }

        /// <summary>
        /// Builds the grammar.
        /// </summary>
        /// <returns></returns>
        private Grammar BuildGrammar()
        {
            // toolbar commands
            Choices commands = new Choices("new", "first", "previous", "next", "last", "move", "rotate");

            // select square command - "select a one", "select a two" etc.
            GrammarBuilder selectCommands = new GrammarBuilder();
            selectCommands.Append("select");
            selectCommands.Append(new Choices("a", "b", "c", "d", "e", "f", "g", "h"));
            selectCommands.Append(new Choices("one", "two", "three", "four", "five", "six", "seven", "eight"));

            commands.Add(selectCommands);

            return new Grammar(commands);
        }

        public void Dispose()
        {
            if (isRecognitionAvailable) { recognizer.Dispose(); }
        }
    }
}
