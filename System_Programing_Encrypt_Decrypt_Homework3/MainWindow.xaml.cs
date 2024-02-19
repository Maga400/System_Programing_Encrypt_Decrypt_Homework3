using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace System_Programing_Encrypt_Decrypt_Homework3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private int maxValue;
        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                OnPropertyChanged();

            }
        }

        private int fileValue;
        public int FileValue
        {
            get { return fileValue; }
            set
            {
                fileValue = value;
                OnPropertyChanged();
            }
        }
        public Thread WorkerThread { get; set; }
        public string SC { get; set; }
        public string srcPath { get; set; }
        public string destPath { get; set; }
        public byte[]? Key { get; set; }
        public byte[]? IV { get; set; }
        private bool CheckCancel { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
                    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public MainWindow()
        {
            InitializeComponent();
            MaxValue = 100;

            DataContext = this;
        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "butun fayllar (*.*)|*.*";


            if (fileDialog.ShowDialog() == true)
            {
                fileTextBox.Text = fileDialog.FileName;
                WorkerThread = new Thread(f);
                WorkerThread.Start();

                void f()
                {
                    srcPath = fileDialog.FileName;

                }

            }
        }


        private void StartEncryptOrDecrypt(object sender, RoutedEventArgs e)
        {
            if(encryptRadioButton.IsChecked == true) 
            {
                WorkerThread = new Thread(EncryptCode);
                WorkerThread.Start();
                pb.Value = 0;

                
            }
            else 
            {
                WorkerThread = new Thread(DecryptCode);
                WorkerThread.Start();
                pb.Value = 0;

                
            }

            fileTextBox2.Text = "";
            fileTextBox.Text = "";
            

        }
        public void EncryptCode()
        {
            if (!File.Exists(srcPath))
            {
                MessageBox.Show("From File not exists", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            if (!File.Exists(destPath))
            {
                MessageBox.Show("To File not exists", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            try
            {
                using (FileStream fsInput = new FileStream(srcPath, FileMode.Open))
                {
                    using (FileStream fsOutput = new FileStream(destPath, FileMode.Create))
                    {
                        using (AesManaged aes = new AesManaged())
                        {
                            aes.GenerateKey();
                            aes.GenerateIV();
                            Key = aes.Key;
                            IV = aes.IV;

                            
                            ICryptoTransform encryptor = aes.CreateEncryptor();
                            using (CryptoStream cs = new CryptoStream(fsOutput, encryptor, CryptoStreamMode.Write))
                            {
                                
                                var len = 10;
                                var fileSize = fsInput.Length;
                                MaxValue = (int)fileSize;
                                byte[] buffer = new byte[len];


                                do
                                {
                                    if (CheckCancel)
                                    {
                                        FileValue = 0;
                                        CheckCancel = false;
                                        fsOutput.Dispose();
                                        
                                        
                                        File.WriteAllText(SC, string.Empty);
                                        break;
                                    }

                                    Thread.Sleep(10);
                                    len = fsInput.Read(buffer, 0, buffer.Length); 
                                    cs.Write(buffer, 0, len);

                                    Console.WriteLine(fileSize);
                                    fileSize -= len;
                                    FileValue += len;

                                } while (len != 0);

                                srcPath = "";
                                destPath = "";
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "INFO", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void DecryptCode()
        {
            if (!File.Exists(srcPath))
            {
                MessageBox.Show("From File not exists", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            if (!File.Exists(destPath))
            {
                MessageBox.Show("To File not exists", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            try
            {
                using (FileStream fsInput = new FileStream(srcPath, FileMode.Open))
                {
                    using (FileStream fsOutput = new FileStream(destPath, FileMode.Create))
                    {
                        using (AesManaged aes = new AesManaged())
                        {
                            
                            aes.Key = Key;
                            aes.IV = IV;

                            
                            ICryptoTransform decryptor = aes.CreateDecryptor();
                            using (CryptoStream cs = new CryptoStream(fsOutput, decryptor, CryptoStreamMode.Write))
                            {

                                var len = 10;
                                var fileSize = fsInput.Length;
                                MaxValue = (int)fileSize;

                                byte[] buffer = new byte[len];


                                do
                                {
                                    if (CheckCancel)
                                    {
                                        FileValue = 0;
                                        CheckCancel = false;
                                        fsOutput.Dispose();

                                        File.WriteAllText(SC, string.Empty);
                                        break;
                                    }

                                    Thread.Sleep(10);
                                    len = fsInput.Read(buffer, 0, buffer.Length); // 8
                                    cs.Write(buffer, 0, len);

                                    Console.WriteLine(fileSize);
                                    fileSize -= len;
                                    FileValue += len;


                                } while (len != 0);

                                srcPath = "";
                                destPath = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "INFO", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void OpenFileDialog2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "butun fayllar (*.*)|*.*";


            if (fileDialog.ShowDialog() == true)
            {
                fileTextBox2.Text = fileDialog.FileName;
                SC = fileDialog.FileName;
                WorkerThread = new Thread(f);
                WorkerThread.Start();

                void f()
                {
                    destPath = fileDialog.FileName;

                }

            }

        }

        private void CancelWork(object sender, RoutedEventArgs e)
        {
            
            CheckCancel = true;
        }
    }

}