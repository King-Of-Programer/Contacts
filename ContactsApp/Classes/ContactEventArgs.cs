using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Classes
{
    public class ContactEventArgs:EventArgs
    {
        public string Action { get; set; }
        public List<Contact> contacts { get; set; }
    }
}
