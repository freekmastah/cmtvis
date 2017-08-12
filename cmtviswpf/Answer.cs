using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace cmtviswpf
{
    public class Answer
    {
        const string PLACEHOLDER = "REPLACE THIS WITH YOUR ANSWER";

        public List<TextBox> textboxes = new List<TextBox>();
        public List<RadioButton> radiobuttons = new List<RadioButton>();
        public List<CheckBox> checkboxes = new List<CheckBox>();
        public XElement element = null;

        public void setXML()
        {
            if (textboxes.Count > 0)
            {
                element.Value = textboxes[0].Text;
                return;
            }
            if (checkboxes.Count > 0)
            {
                element.Value = checkboxes[0].IsChecked == true ? "Yes" : "No";
                return;
            }
            bool found = false;
            foreach (RadioButton cb in radiobuttons)
            {
                if (cb.IsChecked == true)
                {
                    element.Value = (string)cb.Content;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                element.Value = PLACEHOLDER;
            }
        }


        public void setControl()
        {
            if (textboxes.Count > 0)
            {
                if (element.Value == PLACEHOLDER)
                {
                    textboxes[0].Text = "";
                }
                else
                {
                    textboxes[0].Text = element.Value;
                }
                return;
            }
            if (checkboxes.Count > 0)
            {
                checkboxes[0].IsChecked = element.Value == "Yes";
                return;
            }
            foreach (RadioButton cb in radiobuttons)
            {
                if (element.Value == PLACEHOLDER)
                {
                    cb.IsChecked = false;
                }
                else
                {
                    cb.IsChecked = element.Value == (string) cb.Content;
                }
            }
        }
    }
}
