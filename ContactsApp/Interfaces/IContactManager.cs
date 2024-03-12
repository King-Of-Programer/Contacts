using ContactsApp.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Interfaces
{
    public interface IContactManager
    {
        event EventHandler<ContactEventArgs> ContactHandler;
        void GetAllContacts();
        void AddNewContact(string name, string phoneNumber);
        void SearchContact(string query);
    }
}
