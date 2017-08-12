using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Windows.Threading;
using System.Windows.Markup;

namespace cmtviswpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XDocument xdoc = null;
        string filename = null;
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string pdf = null;
        GridLength pdfbackup = GridLength.Auto;
        List<Answer> answers = new List<Answer>();

        public MainWindow()
        {
            InitializeComponent();
            openFolder(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        }

        private void openFolder(string folder)
        {
            this.folder = folder;
            string[] files = Directory.GetFiles(folder, "*.xml");
            List<MetaInfo> metainfos = new List<MetaInfo>();
            foreach (string file in files)
            {
                List<MetaInfo> fmetainfos = MetaInfo.loadMetaInfos(file);
                foreach (MetaInfo meta in fmetainfos)
                {
                    metainfos.Add(meta);
                }
            }
            folderView.ItemsSource = metainfos;
            if (metainfos.Count < 1)
            {
                folderView.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                folderView.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void openXML(string fn, string childpaperid)
        {
            try
            {
                closeXML();
                rootpanel.Children.Clear();
                xdoc = XDocument.Load(fn);
                filename = fn;
                this.answers.Clear();
                string conference = xdoc.Root.Attribute("shortName").Value;
                string deadline = ""; // xdoc.Root.Attribute("ReviewDeadline").Value;
                var papers = xdoc.Root.Descendants("submission");
                XElement paperroot = null;
                if (childpaperid == null)
                {
                    paperroot = papers.ElementAt(0);
                }
                else
                {
                    foreach (XElement lpaper in papers)
                    {
                        if (lpaper.Attribute("id").Value == childpaperid)
                        {
                            paperroot = lpaper;
                        }
                    }
                }
                string paperid = paperroot.Attribute("id").Value;
                string title = paperroot.Attribute("title").Value;
                this.Title = String.Format("{0} : {2} ({1})", conference, paperid, title);

                var quests = paperroot.Descendants("question");
                int count = 0;
                foreach (XElement question in quests)
                {
                    int number = Int32.Parse(question.Attribute("number").Value);
                    string text = question.Attribute("text").Value;
                    var answersPresent = question.Descendants("options");
                    IEnumerable<XElement> answers = null;
                    if (answersPresent.Count() > 0)
                    {
                        answers = answersPresent.Descendants("option");
                    }

                    bool ischeckbox = answersPresent.Count() > 0 && answers.Count() == 2 && answers.ElementAt(0).Value == "Yes" && answers.ElementAt(1).Value == "No";
                    Answer myaw = new Answer();
                    StackPanel questpanel = new StackPanel();
                    TextBlock qtext = new TextBlock();
                    qtext.Text = text;
                    qtext.TextWrapping = TextWrapping.Wrap;
                    qtext.Margin = new Thickness(5, 5, 5, 5);
                    qtext.FontWeight = FontWeights.SemiBold;
                    qtext.FontSize = 14;
                    if (ischeckbox)
                    {
                        CheckBox cb = new CheckBox();
                        cb.Margin = new Thickness(5, 5, 5, 5);
                        qtext.Margin = new Thickness(0);
                        cb.Checked += cb_Checked;
                        cb.Content = qtext;
                        questpanel.Children.Add(cb);
                        myaw.checkboxes.Add(cb);
                    }
                    else
                    {
                        questpanel.Children.Add(qtext);
                    }
                    if (((count++) % 2) == 0)
                    {
                        questpanel.Background = new SolidColorBrush(Color.FromArgb(255, 235, 235, 235));
                    }
                    else
                    {

                    }

                    var answersElem = question.Descendants("answers");

                    XElement myanswer = null;

                    if (answersElem.Count() > 0)
                    {
                        myanswer = answersElem.Descendants("answer").ElementAt(0);
                    } else
                    {
                        myanswer = question.Descendants("answer").ElementAt(0);
                    }

                    myaw.element = myanswer;
                    if (answersPresent.Count() < 1 || answers.Count() < 1)
                    {
                        TextBox tb = new TextBox();
                        tb.TextWrapping = TextWrapping.Wrap;
                        tb.AcceptsReturn = true;
                        tb.Margin = new Thickness(5, 5, 5, 5);
                        tb.FontSize = 13;
                        tb.TextChanged += tb_TextChanged;
                        tb.Language = XmlLanguage.GetLanguage("en-US");
                        tb.SpellCheck.IsEnabled = true;
                        
                        questpanel.Children.Add(tb);
                        myaw.textboxes.Add(tb);
                    }
                    else if (!ischeckbox)
                    {
                        StackPanel grouptab = new StackPanel();
                        foreach (XElement answer in answers)
                        {
                            RadioButton rb = new RadioButton();
                            rb.Content = answer.Value;
                            rb.Margin = new Thickness(5, 5, 5, 5);
                            rb.FontSize = 13;
                            rb.Checked += rb_Checked;
                            grouptab.Children.Add(rb);
                            myaw.radiobuttons.Add(rb);
                        }
                        questpanel.Children.Add(grouptab);
                    }
                    rootpanel.Children.Add(questpanel);
                    this.answers.Add(myaw);
                    myaw.setControl();
                }
                try
                {
                    string[] pdfs = Directory.GetFiles(System.IO.Path.GetDirectoryName(filename), "Paper " + paperid + ".pdf");
                    if (pdfs.Length > 0)
                    {
                        pdf = pdfs[0];
                    }
                    else
                    {
                        pdfs = Directory.GetFiles(System.IO.Path.GetDirectoryName(filename), "Paper " + paperid + "(*).pdf");
                        if (pdfs.Length > 0)
                        {
                            pdf = pdfs[0];
                        }
                        else
                        {
                            pdfs = Directory.GetFiles(System.IO.Path.GetDirectoryName(filename) + System.IO.Path.DirectorySeparatorChar + "Assigned Papers", "Paper " + paperid + ".pdf");
                            if (pdfs.Length > 0)
                            {
                                pdf = pdfs[0];
                            }
                            else
                            {
                                pdf = null;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    pdf = null;
                }


                savebutton.IsEnabled = false;
                closebutton.IsEnabled = true;
                pdfbutton.IsEnabled = pdf != null;
                rootscroll.Visibility = System.Windows.Visibility.Visible;
                if (pdf != null)
                {
                    browser.Navigate("file:///" + pdf);
                    if (centergrid.ColumnDefinitions[2].Width.Value < 0.1)
                    {
                        centergrid.ColumnDefinitions[2].Width = pdfbackup;
                    }
                }
                else
                {
                    browser.Navigate("about:blank");
                    pdfbackup = centergrid.ColumnDefinitions[2].Width;
                    centergrid.ColumnDefinitions[2].Width = new GridLength(0);
                }
            }
            catch (Exception e)
            {
                closeXML();
                MessageBox.Show(e.Message);
            }
        }

        void cb_Checked(object sender, RoutedEventArgs e)
        {
            savebutton.IsEnabled = true;
        }

        void rb_Checked(object sender, RoutedEventArgs e)
        {
            savebutton.IsEnabled = true;
        }

        void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            savebutton.IsEnabled = true;
        }

        private void closeXML()
        {
            if (savebutton.IsEnabled)
            {
                MessageBoxResult mr = MessageBox.Show("Save review form ?", "Unsaved Content.", MessageBoxButton.YesNo);
                if (mr == MessageBoxResult.Yes)
                {
                    saveXML();
                }
            }
            if (pdf != null)
            {
                pdfbackup = centergrid.ColumnDefinitions[2].Width;
            }
            browser.Navigate("about:blank");
            centergrid.ColumnDefinitions[2].Width = new GridLength(0);
            xdoc = null;
            filename = null;
            pdf = null;
            rootpanel.Children.Clear();
            Title = "CMTVIS";
            savebutton.IsEnabled = false;
            closebutton.IsEnabled = false;
            pdfbutton.IsEnabled = false;
            rootscroll.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "CMT XML (*.xml)|*.xml";

            if (dlg.ShowDialog() != true)
            {
                return;
            }
            List<MetaInfo> metainfos = MetaInfo.loadMetaInfos(dlg.FileName);
            if (metainfos.Count() < 1)
            {
                MessageBox.Show("The file contains no valid review form.");
                return;
            }
            if (metainfos.Count() == 1)
            {
                openXML(dlg.FileName, null);
                return;
            }
            closeXML();
            folderView.ItemsSource = metainfos;
            MessageBox.Show("The file contains multiple review forms. Please choose your form from the folder view on the left.");
        }

        private void saveXML()
        {
            if (xdoc == null)
            {
                return;
            }
            foreach (Answer aw in this.answers)
            {
                aw.setXML();
            }
            xdoc.Save(filename, SaveOptions.None);
            savebutton.IsEnabled = false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            saveXML();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FolderSelect.FolderSelectDialog fdlg = new FolderSelect.FolderSelectDialog();
            fdlg.Title = "Change Folder";
            fdlg.InitialDirectory = this.folder;

            if (!fdlg.ShowDialog(IntPtr.Zero))
            {
                return;
            }
            openFolder(fdlg.FileName);
        }

        private void folderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MetaInfo item = (MetaInfo)folderView.SelectedValue;
            if (item == null)
            {
                return;
            }
            openXML(item.filename, item.paperid);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            closeXML();
        }

        private void pdfbutton_Click(object sender, RoutedEventArgs e)
        {
            if (pdf == null)
            {
                return;
            }
            System.Diagnostics.Process.Start(pdf);
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (savebutton.IsEnabled)
            {
                MessageBoxResult mr = MessageBox.Show("Do you want to save your work before closing the app ?", "Unsaved Content.", MessageBoxButton.YesNoCancel);
                if (mr == MessageBoxResult.Yes)
                {
                    saveXML();
                }
                if (mr == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }



    }
}
