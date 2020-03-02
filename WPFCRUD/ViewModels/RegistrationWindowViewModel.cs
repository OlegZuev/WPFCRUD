using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Database;
using WPFCRUD.Models;
using WPFCRUD.Views;

namespace WPFCRUD.ViewModels {
    public class RegistrationWindowViewModel : ModifyingWindowViewModel {
        public override string WindowTitle { get; } = "Форма регистрации";

        public override string ModifyUserCommandTitle { get; } = "Зарегистрироваться";

        public RegistrationWindowViewModel() {
            ModifyUserCommand = new DelegateCommand<PasswordBox>(RegisterUser, CanRegisterUser);

            DatabaseInstance.ErrorInfoChanged += (type, o) => {
                switch (type) {
                    case EventStringTypes.Login:
                        LoginErrorProvider.ErrorName = o as string;
                        break;
                    case EventStringTypes.Password:
                        PasswordErrorProvider.ErrorName = o as string;
                        break;
                }

                CommandManager.InvalidateRequerySuggested();
            };
        }

        public sealed override ICommand ModifyUserCommand { get; protected set; }

        private void RegisterUser(PasswordBox sender) {
            DatabaseInstance.CheckLoginTimer(TbLoginText);
            if (!CanRegisterUser(sender))
                return;

            try {
                MessageBox.Show(DatabaseInstance.RegisterUser(TbLoginText, sender.Password)
                                    ? "Вы успешно зарегистрированы!"
                                    : "Ошибка, некорректные данные. Запрос на регистрацию отклонён!", "Уведомление");
                DatabaseInstance.CheckLoginTimer(TbLoginText);
            } catch (Npgsql.PostgresException e) {
                MessageBox.Show(e.Message);
            }
        }

        private bool CanRegisterUser(PasswordBox sender) {
            return string.IsNullOrEmpty(LoginErrorProvider.ErrorName) && string.IsNullOrEmpty(PasswordErrorProvider.ErrorName) &&
                   !string.IsNullOrEmpty(TbLoginText) && !string.IsNullOrEmpty(sender.Password);
        }
    }
}