using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Airlines.DataSetAirlanesTableAdapters;
using System.Windows.Threading;

namespace Airlines
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataSetAirlanes dataSetAirlines;
        UsersTableAdapter usersTableAdapter;
        Int32 seconds = 0;
        Int32 errorCount = 0;
        String captcha = "";
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            dataSetAirlines = new DataSetAirlanes();
            usersTableAdapter = new UsersTableAdapter();
            generateCaptcha();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
        }

        //метод для разблокировки панели
        private void timerTick(object sender, EventArgs e)
        {
            seconds++;
            if (seconds > 9)
            {
                timer.Stop();
                seconds = 0;
                lockInterface(true);
            }
        }

        //функция переключения статуса 
        private void lockInterface(bool state)
        {
            username.IsEnabled = state;
            password.IsEnabled = state;
            tb_Captcha.IsEnabled = state;
        }
       
        //метод для блокировки
        private void error()
        {
            errorCount++;
            if (errorCount > 4)
            {
                errorCount = 0;
                timer.Start();
                generateCaptcha();
                lockInterface(false);
                MessageBox.Show("Вы превысили количество попыток входа. Подождите 10 секунд.");
            }
        }
        /// <summary>
        /// метод для капчи
        /// </summary>
        private void generateCaptcha()
        {
            var chars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789";
            var random = new Random();
            char[] charsString = new char[8];
            for (int i = 0; i < 8; i++)
            {
                charsString[i] = chars[random.Next(0, chars.Length)];
            }
            captcha = new String(charsString);
            lblCaptcha.Content = $"Введите капчу: {captcha}";
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (username.Text != "" && password.Password != "")
            {
                if (tb_Captcha.Text == captcha)
                {
                    int? id_user = usersTableAdapter.Login(username.Text, password.Password);
                    if (id_user != null)
                    {
                        errors.Text = "Вы вошли в учетную запись";
                    }
                    else
                    {
                        errors.Text = "Логин или паролоь неверны, попробуйте еще раз.";
                        username.Clear();
                        password.Clear();
                        error();
                    }
                }
                else
                {
                    errors.Text = "Вы ввели неверно капчу";
                    error();
                    generateCaptcha();
                    username.Clear();
                    password.Clear();
                    tb_Captcha.Clear();
                }
            }
            else
            {
                errors.Text = "Введите логин и/или пароль";
                error();
                generateCaptcha();
            }
        }

        
    }
}
