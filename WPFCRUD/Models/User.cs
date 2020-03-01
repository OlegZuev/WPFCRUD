using System;
using PropertyChanged;

namespace WPFCRUD.Models {
    [AddINotifyPropertyChangedInterface]
    public class User {
        public long Id { get; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string RegistrationDate { get; set; }

        public User(long id, string login, string password, string registrationDate) {
            Id = id;
            Login = login;
            Password = password;
            RegistrationDate = registrationDate;
        }
    }
}