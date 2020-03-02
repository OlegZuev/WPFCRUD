using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Database;
using WPFCRUD.Models;
using WPFCRUD.Views;

namespace WPFCRUD.ViewModels {
    public class EditingWindowViewModel : ModifyingWindowViewModel {
        private readonly User _initialUser;

        public override string WindowTitle { get; } = "Форма редактирования";

        public override string ModifyUserCommandTitle { get; } = "Изменить";

        public EditingWindowViewModel(User user) {
            _initialUser = user;
            TbLoginText = _initialUser.Login;
            RegistrationDate = DateTime.Parse(_initialUser.RegistrationDate);

            ModifyUserCommand = new DelegateCommand<PasswordBox>(ChangeUser, CanChangeUser);

            DatabaseInstance.ErrorInfoChanged += (type, o) => {
                switch (type) {
                    case EventStringTypes.Login:
                        LoginErrorProvider.ErrorName = TbLoginText != _initialUser.Login ? o as string : string.Empty;
                        break;
                    case EventStringTypes.Password:
                        PasswordErrorProvider.ErrorName = o as string;
                        break;
                }
                CommandManager.InvalidateRequerySuggested();
            };
        }

        public sealed override ICommand ModifyUserCommand { get; protected set; }

        private void ChangeUser(PasswordBox sender) {
            DatabaseInstance.CheckLoginTimer(TbLoginText);
            if (!CanChangeUser(sender))
                return;

            try {
                long id = _initialUser.Id;
                bool passwordChanged = !string.IsNullOrEmpty(sender.Password);
                string password = passwordChanged
                    ? sender.Password
                    : _initialUser.Password;
                if (DatabaseInstance.ChangeUser(id, TbLoginText, password, passwordChanged, RegistrationDate)) {
                    MessageBox.Show("Пользователь успешно изменён!", "Уведомление");
                    CloseAction();
                } else {
                    MessageBox.Show("Ошибка, некорректные данные. Запрос на изменение отклонён!", "Уведомление");
                }
            } catch (Npgsql.PostgresException e) {
                MessageBox.Show(e.Message);
            }
        }

        private bool CanChangeUser(PasswordBox sender) {
            return string.IsNullOrEmpty(LoginErrorProvider.ErrorName) && string.IsNullOrEmpty(PasswordErrorProvider.ErrorName) &&
                   !string.IsNullOrEmpty(TbLoginText) &&
                   (_initialUser.Login != TbLoginText || !string.IsNullOrEmpty(sender.Password) ||
                    DateTime.Parse(_initialUser.RegistrationDate) != RegistrationDate);
        }
    }
}