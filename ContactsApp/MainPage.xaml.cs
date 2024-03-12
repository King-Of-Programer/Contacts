using ContactsApp.Classes;
using ContactsApp.Interfaces;
using System;
using Xamarin.Forms;

namespace ContactsApp
{
    public partial class MainPage : ContentPage
    {
        IContactManager contactManager;
        IToast toast;
        public MainPage()
        {
            InitializeComponent();
            contactManager = DependencyService.Get<IContactManager>();
            toast = DependencyService.Get<IToast>();
            contactManager.ContactHandler += ContactHandler_Event;
            contactManager.GetAllContacts();
        }
        private Label createLabel(string text, double fontSize = 20, bool isBold = false)
        {
            return new Label()
            {
                TextColor = Color.Black,
                Text = text,
                FontAttributes = isBold ? FontAttributes.Bold : FontAttributes.None,
                FontSize = fontSize
            };

        }
        private Frame createContactCard(Contact contact)
        {
            Frame card = new Frame()
            {
                BorderColor = Color.Black,
                Padding = 5,
            };
            Grid grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition(),
                }
            };

            StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var nameLabel = createLabel($"Имя:", 20);
            var nameValueLabel = createLabel(contact.Name);
            stack.Children.Add(nameLabel);
            stack.Children.Add(nameValueLabel);

            Grid.SetRow(stack, 0);
            grid.Children.Add(stack);

            stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var numberLabel = createLabel($"Номер:", 20);
            var numberValueLabel = createLabel(contact.Number);
            stack.Children.Add(numberLabel);
            stack.Children.Add(numberValueLabel);

            Grid.SetRow(stack, 1);
            grid.Children.Add(stack);

            card.Content = grid;
            return card;
        }
        private void ContactHandler_Event(object sender, EventArgs e)
        {
            var args = (ContactEventArgs)e;
            var contacts = args.contacts;
            if (contacts != null)
            {
                if (contacts.Count != 0)
                {
                    contactsStack.Children.Clear();
                    foreach (var contact in args.contacts)
                    {
                        var contactCard = createContactCard(contact);
                        contactsStack.Children.Add(contactCard);
                    }
                }
            }
        }

        //private void addNewContact_Click(object sender, EventArgs e)
        //{
        //    addContactForm.IsVisible = false;
        //    switchFormVisible.IsVisible = true;
        //    if(!String.IsNullOrEmpty(newName.Text)&& !String.IsNullOrEmpty(newNumber.Text))
        //    {
        //        contactManager.AddNewContact(newName.Text, newNumber.Text);
        //        contactManager.GetAllContacts();
        //        newNumber.Text = newName.Text = "";
        //    }
        //    else
        //    {
        //        toast.LongMessage("Contact not added");
        //    }
        //}
        //private void addNewContactView_Click(object sender, EventArgs e)
        //{
        //    addContactForm.IsVisible = true;
        //    (sender as Button).IsVisible = false;
        //}
        private void searchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = searchTextbox.Text;
            if (!String.IsNullOrEmpty(query))
            {
                contactManager.SearchContact(query);
            }
            else
            {
                contactManager.GetAllContacts();
            }
        }
    }
}
