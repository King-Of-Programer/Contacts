using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp.Interfaces
{
    public interface IToast
    {
        void ShortMessage(string message);
        void LongMessage(string message);
    }
}
