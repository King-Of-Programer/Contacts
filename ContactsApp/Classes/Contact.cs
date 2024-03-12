using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Classes
{
    public class Contact
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public Contact(string id, string name, string phoneNumber)
        {
            Id = id;
            Name = name;
            Number = phoneNumber;
        }
    }
}
