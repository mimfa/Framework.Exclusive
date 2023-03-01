using MiMFa.Exclusive.ProgramingTechnology.ReportLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.Archive.Reports
{
    public class Address : MiMFa.Config
    {
        public virtual string ReportsStyle { get; internal set; }
        public string ReportsArchive;
        public string ReportStyleArchivePath;

        public Address() : base()
        {
            DefaultValues();
        }
        public new virtual void DefaultValues()
        {
            Config.DefaultValues();
            ReportsStyle = ThisDirectory + @"ReportsStyle\";
            ReportsArchive = ThisDirectory + "ReportsArchive.db";
            ReportStyleArchivePath = ReportsStyle + @"ReportsArchive" + ReportStyle.Extension;
        }
    }
}