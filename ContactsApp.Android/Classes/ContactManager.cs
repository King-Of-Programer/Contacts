using Android;
using AndroidApp = Android.App.Application;
using ContactsApp.Classes;
using ContactsApp.Interfaces;
using System;
using Xamarin.Forms;
using AndroidX.Core.App;
using Android.Provider;
using System.Collections.Generic;
using static Android.App.DownloadManager;
using Android.Widget;
using System.Linq;
using Android.Content;
using System.Runtime.Remoting.Contexts;

[assembly: Dependency(typeof(ContactsApp.Droid.Classes.ContactManager))]
namespace ContactsApp.Droid.Classes
{
    public class ContactManager : IContactManager
    {
        public event EventHandler<ContactEventArgs> ContactHandler;
        private const int RequestReadContactsCode = 567895;
        readonly Android.Content.Context context;
        private List<Contact> currentList;
        public ContactManager()
        {
            context = AndroidApp.Context;
            currentList = new List<Contact>();
        }
        private bool CheckPermissions()
        {
            if (AndroidApp.Context.CheckSelfPermission(Manifest.Permission.ReadContacts) == Android.Content.PM.Permission.Granted)
            {
                return true;
            }
            return false;
        }
        public void AddNewContact(string name, string phoneNumber)
        {
            if (CheckPermissions())
            {
                currentList.Clear();
                var values = new ContentValues();
                values.Put(ContactsContract.Contacts.InterfaceConsts.DisplayName, name);

                var uri = context.ContentResolver.Insert(ContactsContract.RawContacts.ContentUri, values);

                long rawContactId = long.Parse(uri.LastPathSegment);
                Toast.MakeText(context, rawContactId.ToString(), ToastLength.Long).Show();
                values.Clear();
                values.Put(ContactsContract.Data.InterfaceConsts.RawContactId, rawContactId);
                values.Put(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Phone.ContentItemType);
                values.Put(ContactsContract.CommonDataKinds.Phone.Number, phoneNumber);

                context.ContentResolver.Insert(Android.Provider.ContactsContract.Data.ContentUri, values);
                UpdateContactName(rawContactId.ToString(), name);
                Toast.MakeText(context, "Contact successfully added", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(context, "You don't have a permission for this", ToastLength.Long).Show();
            }
        }
        public void UpdateContactName(string contactId, string newName)
        {
            var uri = ContactsContract.Data.ContentUri;

            string where = string.Format("{0} = ? AND {1} = ?",
                ContactsContract.Data.InterfaceConsts.ContactId,
                ContactsContract.Data.InterfaceConsts.Mimetype);

            string[] whereParameters = new string[] 
            {
        contactId,
        ContactsContract.CommonDataKinds.StructuredName.ContentItemType
    };

            var values = new ContentValues();
            values.Put(ContactsContract.CommonDataKinds.StructuredName.DisplayName, newName);

            context.ContentResolver.Update(uri, values, where, whereParameters);
        }

        public void GetAllContacts()
        {
            if (CheckPermissions())
            {
                if (currentList.Count == 0)
                {
                    var cursor = context.ContentResolver.Query(ContactsContract.Contacts.ContentUri, null, null, null, null);

                    if (cursor != null)
                    {
                        while (cursor.MoveToNext())
                        {
                            string contactId = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Id));
                            string contactName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                            string phoneNumber = GetPhoneNumber(contactId);

                            currentList.Add(new Contact(contactId, contactName, phoneNumber));
                        }
                        cursor.Close();
                    }
                }
                ContactHandler.Invoke(this, new ContactEventArgs() { Action = "GET", contacts = currentList });
            }
            else
            {
                ActivityCompat.RequestPermissions(MainActivity.mainActivity, new string[] { Manifest.Permission.ReadContacts }, RequestReadContactsCode);
            }
        }
        private string GetPhoneNumber(string contactId)
        {
            var cursor = context.ContentResolver.Query(
                ContactsContract.CommonDataKinds.Phone.ContentUri,
                null,
                ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?",
                new string[] { contactId },
                null);

            string phoneNumber = null;

            if (cursor != null && cursor.MoveToFirst())
            {
                phoneNumber = cursor.GetString(cursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));
                cursor.Close();
            }

            return phoneNumber;
        }

        public void SearchContact(string query)
        {
            if (CheckPermissions())
            {
                if (currentList.Count != 0)
                {
                    var searchResult = currentList.Where(c => c.Name != null && c.Name.ToLower().Contains(query.ToLower())).ToList();
                    searchResult.AddRange(currentList.Where(c => c.Number != null && c.Number.Replace(" ", "").Contains(query)).ToList());
                    if (searchResult != null)
                    {
                        ContactHandler.Invoke(this, new ContactEventArgs()
                        {
                            Action = "SEARCH",
                            contacts = searchResult
                        });
                    }
                }

            }

        }
        private string GetContactName(string contactId)
        {
            var cursor = context.ContentResolver.Query(
                ContactsContract.Contacts.ContentUri,
                null,
                ContactsContract.Contacts.InterfaceConsts.Id + " = ?",
                new string[] { contactId },
                null);

            string contactName = null;

            if (cursor != null && cursor.MoveToFirst())
            {
                contactName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                cursor.Close();
            }

            return contactName;
        }

        public void SearchContactByPhoneNumber(string phoneNumber)
        {
            throw new NotImplementedException();
        }
    }
}