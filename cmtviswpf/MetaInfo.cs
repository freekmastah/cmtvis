using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace cmtviswpf
{
    public class MetaInfo
    {
        public string filename { get; set; }
        public string conference { get; set; }
        public string deadline { get; set; }
        public string paperid { get; set; }
        public string title { get; set; }
        public string purefilename { get; set; }
        public bool valid { get; set; }
        public string pdf { get; set; }
        public string info { get; set; }
        public string pdfbrowser { get; set; }

        public static List<MetaInfo> loadMetaInfos(string filename)
        {
            List<MetaInfo> rt = new List<MetaInfo>();
            MetaInfo mi = null;
            try
            {
                XDocument xdoc = XDocument.Load(filename);
                string conference = xdoc.Root.Attribute("shortName").Value;
                string deadline = ""; // xdoc.Root.Attribute("ReviewDeadline").Value;
                string purefilename = Path.GetFileNameWithoutExtension(filename);
                var papers = xdoc.Root.Descendants("submission");
                foreach (XElement paper in papers)
                {
                    mi = new MetaInfo();
                    mi.valid = true;
                    mi.filename = filename;
                    mi.conference = conference;
                    mi.deadline = deadline;
                    mi.purefilename = purefilename;
                    mi.paperid = paper.Attribute("id").Value;
                    mi.title = paper.Attribute("title").Value;

                    try
                    {
                        string[] pdfs = Directory.GetFiles(System.IO.Path.GetDirectoryName(filename), "Paper " + mi.paperid + ".pdf");
                        if (pdfs.Length > 0)
                        {
                            mi.pdf = pdfs[0];
                        }
                        else
                        {
                            pdfs = Directory.GetFiles(System.IO.Path.GetDirectoryName(filename), "Paper " + mi.paperid + "(*).pdf");
                            if (pdfs.Length > 0)
                            {
                                mi.pdf = pdfs[0];
                            }
                            else
                            {
                                pdfs = Directory.GetFiles(System.IO.Path.GetDirectoryName(filename) + System.IO.Path.DirectorySeparatorChar + "Assigned Papers", "Paper " + mi.paperid + ".pdf");
                                if (pdfs.Length > 0)
                                {
                                    mi.pdf = pdfs[0];
                                }
                                else
                                {
                                    mi.pdf = null;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        mi.pdf = null;
                    }
                    if (mi.pdf != null)
                    {
                        mi.info = "PDF";
                        mi.pdfbrowser = "file:///" + mi.pdf;
                    }
                    else
                    {
                        mi.info = "";
                        mi.pdfbrowser = "about:blank";
                    }
                    rt.Add(mi);
                }
            }
            catch (Exception)
            {
                
            }
            return rt;
        }

        public MetaInfo()
        {
            
        }

    }
}
