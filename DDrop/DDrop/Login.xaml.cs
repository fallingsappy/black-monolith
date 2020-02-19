using DDrop.BE.Models;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DDrop
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public static readonly DependencyProperty UserLoginProperty = DependencyProperty.Register("UserLogin", typeof(User), typeof(Login));
        public User UserLogin
        {
            get { return (User)GetValue(UserLoginProperty); }
            set
            {
                SetValue(UserLoginProperty, value);
            }
        }

        public Login()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxEmail.Text.Length == 0)
            {
                ErrorMessage.Text = "Введите электронную почту.";
                TextBoxEmail.Focus();
            }
            else if (!Regex.IsMatch(TextBoxEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                ErrorMessage.Text = "Электронная почта имеет не верный формат.";
                TextBoxEmail.Select(0, TextBoxEmail.Text.Length);
                TextBoxEmail.Focus();
            }
            else
            {
                //TODO запрос в БД 

                UserLogin = new User()
                {
                    Email = TextBoxEmail.Text,
                    //Avatar = 
                    //UserSeries = new ObservableCollection<Series>(),
                    //FirstName =
                    //LastName = 
                    IsLoggedIn = true,
                    //Password = 
                    //UserId =
                };

                Close();
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            Registration registrationWindow = new Registration();
            registrationWindow.Owner = this;
            registrationWindow.ShowDialog();
            Visibility = Visibility.Visible;

            if (registrationWindow.RegistrationSucceeded)
            {
                UserLogin = registrationWindow.UserLogin;
                Close();
            }
            else
            {
                ShowDialog();
            }
        }

        private void TextBoxLogin_OnTextChanged(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            ErrorMessage.Text = "";
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            LoginPasswordBox.Visibility = Visibility.Hidden;
            PasswordUnmask.Visibility = Visibility.Visible;
            PasswordUnmask.Text = LoginPasswordBox.Password;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            LoginPasswordBox.Visibility = Visibility.Visible;
            PasswordUnmask.Visibility = Visibility.Hidden;
            LoginPasswordBox.Password = PasswordUnmask.Text;
        }
    }
}
