using System;
using System.Management;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.IO;
using OpenQA.Selenium.Interactions;
using System.Xml;



namespace YoutubePanel
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }
        static float NextFloat(double min, double max)
        {
            System.Random random = new System.Random();
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }
        public void SpotifyWatcher(String userMail, String password, String watchUrl)
        {
            ChromeDriver driver;
            ChromeDriverService chromeDriverService;
            chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--mute-audio");
            chromeOptions.AddArgument("--start-maximized");
            chromeOptions.AddArgument("--headless");
            label3.Text = "Tarayıcı başlatılıyor...";
            
            //random geo
            float latitude = NextFloat(36.7, 41.3);
            float longitude = NextFloat(26.5, 43.5);
            var coordinates = new Dictionary<string, object>();
            coordinates.Add("latitude", latitude);
            coordinates.Add("longitude", longitude);
            coordinates.Add("accuracy", 1);

            driver = new ChromeDriver(chromeDriverService,chromeOptions);
            driver.ExecuteCdpCommand("Emulation.setGeolocationOverride", coordinates);
            driver.Manage().Window.Maximize();
            Thread.Sleep(1432);

            try
            {
                label3.Text = "Spotify'a giriş yapılıyor...";

                driver.Navigate().GoToUrl("https://accounts.spotify.com/tr/login/?continue=https%3A//open.spotify.com/__noul__%3Fl2l%3D1%26nd%3D1&_locale=tr-TR");
                Thread.Sleep(9000);

                var mail = driver.FindElement(By.Id("login-username"));
                mail.SendKeys(userMail);
                Thread.Sleep(2432);


                var pass = driver.FindElement(By.Id("login-password"));
                pass.SendKeys(password);
                Thread.Sleep(2432);

                var nextButton = driver.FindElement(By.Id("login-button"));
                nextButton.Click();
                Thread.Sleep(2122);
                label3.Text = "Şarkıya gidiliyor...";

                //watch url
                driver.Navigate().GoToUrl(watchUrl);
                Thread.Sleep(13000);

                String checkAlbum = watchUrl.Substring(25, 5);

                int minute = 0;
                int second = 0;
                if (checkAlbum == "track")
                {
                    try
                    {
                        label3.Text = "Şarkı başlatılıyor...";

                        var playButton = driver.FindElements(By.CssSelector("button.Button-qlcn5g-0.fGLwlk"));
                        playButton[0].Click();
                        label3.Text = "Şarkı dinleniyor..";

                    }
                    catch
                    {

                        driver.Navigate().GoToUrl(watchUrl);
                        Thread.Sleep(12000);
                        label3.Text = "Şarkı başlatılıyor...";

                        var playButton = driver.FindElements(By.CssSelector("button.Button-qlcn5g-0.fGLwlk"));
                        playButton[0].Click();
                        label3.Text = "Şarkı dinleniyor...";

                    }
                    var allDuration = driver.FindElements(By.CssSelector("span.Type__TypeElement-goli3j-0.bWzOVV.RANLXG3qKB61Bh33I0r2"));
                    String[] cute = allDuration[1].Text.Split(':');
                    minute += int.Parse(cute[0]);
                    second += int.Parse(cute[1]);
                
                }
                else
                {
                    label3.Text = "Albüm başlatılıyor...";
                    var playButton = driver.FindElements(By.CssSelector("span.Type__TypeElement-goli3j-0.dascyV.VrRwdIZO0sRX1lsWxJBe"));
                    playButton[0].Click();
                    
                    var allDuration = driver.FindElements(By.CssSelector("div.Type__TypeElement-goli3j-0.fcehhQ.Btg2qHSuepFGBG6X0yEN"));
                    foreach (var duration in allDuration)
                    {
                        label3.Text = "Albüm dinleniliyor...";

                        var musicTime = duration.Text;
                        String[] cute = musicTime.Split(':');
                        minute += int.Parse(cute[0]);
                        second += int.Parse(cute[1]);
                    }

                }
                // button.Button-qlcn5g-0.fGLwlk -- old css selector(click big button)

                //new Actions(driver).DoubleClick(playButton[1]).Perform();


                int waitTime = minute * 60 * 1000 + second * 1000;
                Thread.Sleep(waitTime + 8000);
            }
            catch(Exception e)
            {
                String getText = label5.Text;
                int setText = int.Parse(getText.Substring(16)) - 1;
                if (setText != 0)
                {
                    label5.Text = "Dinlenme sayısı: " + setText.ToString();
                }
            }
            finally
            {
                driver.Quit();
            }
        }
       
        public void Start()
        {
            int _totalWatch = 0;
            //get url from textbox
            String watchUrl = textBox1.Text;
            int watchTotal = int.Parse(textBox2.Text);

            if(_totalWatch < watchTotal)
            {
                //start watching
                string[] allMail = File.ReadAllLines("mail.txt");
                foreach (String mail in allMail)
                {
                    if(_totalWatch >= watchTotal)
                    {
                        break;
                    }
                    else
                    {
                        String[] splited = mail.Split(':');
                        /* 
                         * mail@addres.com:password
                         */
                        //splited[0]= mail@addres.com
                        //splited[1] = password

                        String _mail = splited[0].Trim();
                        String _pass = splited[1].Trim();
                        SpotifyWatcher(_mail, _pass, watchUrl);

                        label3.Text = "Sıradaki hesaba geçiliyor...";
                        //raise watching
                        _totalWatch++;
                        label5.Text = "Dinlenme sayısı: " + _totalWatch.ToString();

                    }
                }
                label3.Text = "Tüm dinlenmeler yapıldı.";

                MessageBox.Show("Şarkı veya albüm " + _totalWatch.ToString() + " defa başarıyla izlendi.");
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            int countAccount = File.ReadLines("mail.txt").Count();
            label6.Text = "Mevcut hesap: " + countAccount.ToString();
            label3.Text = "Bot durumu: Çalışıyor...";
            await Task.Run(Start);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
