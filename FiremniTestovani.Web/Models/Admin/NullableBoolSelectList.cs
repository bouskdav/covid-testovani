using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiremniTestovani.Web.Models.Admin
{
    public class NullableBoolSelectList
    {
        public NullableBoolSelectList()
        {
            this.Items = new List<SelectListItem>();

            this.Items.Add(new SelectListItem(null, "Nevybráno"));
            this.Items.Add(new SelectListItem(true, "Zapnuto"));
            this.Items.Add(new SelectListItem(false, "Vypnuto"));
        }

        public List<SelectListItem> Items { get; set; }
    }

    public class SelectListItem
    {
        public SelectListItem()
        {
        }

        public SelectListItem(object value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        public object Value { get; set; }

        public string Text { get; set; }
    }
}
