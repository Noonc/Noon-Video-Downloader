using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Threading;
using System;

namespace Video_Downloader
{
    public class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
             base.Write(value);
            _output.AppendText(value.ToString()); // When character data is written, append it to the text box.
            _output.ScrollToEnd();
            //Thread.Sleep(1000);
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
