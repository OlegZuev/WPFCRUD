using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Database;
using Registration.ViewModel;
using WPFCRUD.Views;

namespace WPFCRUD.ViewModels {
    public class EditingWindowViewModel : BaseViewModel {
        private readonly string _initialLogin;
        private readonly DateTime _initialRegistrationDate;

        private readonly DatabaseInteraction _database;
        private readonly EditingWindow _parentWindow;

        private readonly Dictionary<int, string> _imgStrengthPaths = new Dictionary<int, string> {
            {0, "../Images/PasswordStrength0.png"},
            {1, "../Images/PasswordStrength1.png"},
            {2, "../Images/PasswordStrength2.png"},
            {3, "../Images/PasswordStrength3.png"},
            {4, "../Images/PasswordStrength4.png"}
        };

        private string _tbLoginText;

        public string TbLoginText {
            get => _tbLoginText;
            set {
                _tbLoginText = value;
                _database.CheckLogin(_tbLoginText);
                OnPropertyChanged(nameof(TbLoginText));
            }
        }

        public string ImgPasswordStrengthPath { get; set; }

        public Visibility ImgPasswordVisibility { get; set; }

        public DateTime RegistrationDate { get; set; }

        public EditingWindowViewModel() {
            try {
                _database = new DatabaseInteraction();
            } catch (Npgsql.PostgresException e) {
                MessageBox.Show(e.Message);
                throw;
            }

            _parentWindow = EditingWindow.Instance;

            _initialLogin = _parentWindow.CurrentUser.Login;
            _initialRegistrationDate = DateTime.Parse(_parentWindow.CurrentUser.RegistrationDate);
            TbLoginText = _initialLogin;
            RegistrationDate = _initialRegistrationDate;

            ChangeUserCommand = new DelegateCommand(ChangeUser, CanChangeUser);
            _database.PasswordStrengthChanged += (type, o) => {
                if (type == EventStringTypes.PasswordStrengthIndex) {
                    ImgPasswordStrengthPath = _imgStrengthPaths[(int) o];
                }
            };

            _database.ErrorInfoChanged += (type, o) => {
                switch (type) {
                    case EventStringTypes.Login:
                        ((ErrorProviderViewModel) _parentWindow.TbLogin.DataContext).ErrorName =
                            _tbLoginText != _initialLogin ? o as string : string.Empty;
                        break;
                    case EventStringTypes.Password:
                        ((ErrorProviderViewModel) _parentWindow.TbPassword.DataContext).ErrorName = o as string;
                        break;
                }
                CommandManager.InvalidateRequerySuggested();
            };
        }

        public void TbPassword_OnLostFocus(object sender, RoutedEventArgs e) {
            ImgPasswordVisibility = Visibility.Hidden;
        }

        public void TbPassword_OnPasswordChanged(object sender, RoutedEventArgs e) {
            _database.CheckPassword(((PasswordBox) sender).Password);

            if (ImgPasswordVisibility != Visibility.Visible) {
                ImgPasswordVisibility = Visibility.Visible;
            }
        }

        public ICommand ChangeUserCommand { get; }

        private void ChangeUser(object sender) {
            try {
                long id = _parentWindow.CurrentUser.Id;
                bool passwordChanged = !string.IsNullOrEmpty(_parentWindow.TbPassword.Password);
                string password = passwordChanged
                    ? _parentWindow.TbPassword.Password
                    : _parentWindow.CurrentUser.Password;
                if (_database.ChangeUser(id, TbLoginText, password, passwordChanged, RegistrationDate)) {
                    MessageBox.Show(_parentWindow, "Пользователь успешно изменён!", "Уведомление");
                    _parentWindow.Close();
                } else {
                    MessageBox.Show( _parentWindow,"Ошибка, некорректные данные. Запрос на изменение отклонён!", "Уведомление");
                }
            } catch (Npgsql.PostgresException e) {
                MessageBox.Show(e.Message);
            }
        }

        private bool CanChangeUser(object sender) {
            var tbLoginEp = (ErrorProviderViewModel) _parentWindow.TbLogin.DataContext;
            var tbPasswordEp = (ErrorProviderViewModel) _parentWindow.TbPassword.DataContext;
            return string.IsNullOrEmpty(tbLoginEp.ErrorName) && string.IsNullOrEmpty(tbPasswordEp.ErrorName) &&
                   !string.IsNullOrEmpty(TbLoginText) &&
                   (_initialLogin != TbLoginText || !string.IsNullOrEmpty(_parentWindow.TbPassword.Password) ||
                    _initialRegistrationDate != RegistrationDate);
        }
    }
}