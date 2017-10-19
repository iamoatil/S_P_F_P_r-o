using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace XLY.XDD.Control
{
    public class FolderItems
    {
        public string Folder { get; set; }
        public ImageSource Image { get; set; }

        public FolderItems()
            : base()
        {
            ImageSourceConverter isc = new ImageSourceConverter();
            //Image = isc.ConvertFrom("openfolderHS.png") as ImageSource;
        }
    }
}
