using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using WebView2 = Microsoft.Web.WebView2.WinForms.WebView2;
namespace ChatOverlay
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        public static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        public static uint CurrentResolution = 0;
        public static int x, y;
        public static WebView2 webView21 = new WebView2();
        private static int width = Screen.PrimaryScreen.Bounds.Width;
        private static int height = Screen.PrimaryScreen.Bounds.Height;
        private static string apikey, channelid;
        private static bool getstate = false;
        public static int[] wd = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        public static int[] wu = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        public static bool[] ws = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        static void valchanged(int n, bool val)
        {
            if (val)
            {
                if (wd[n] <= 1)
                {
                    wd[n] = wd[n] + 1;
                }
                wu[n] = 0;
            }
            else
            {
                if (wu[n] <= 1)
                {
                    wu[n] = wu[n] + 1;
                }
                wd[n] = 0;
            }
            ws[n] = val;
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            this.Size = new Size(width, height);
            this.Location = new Point(0, 0);
            using (System.IO.StreamReader file = new System.IO.StreamReader("params.txt"))
            {
                file.ReadLine();
                apikey = file.ReadLine();
                file.ReadLine();
                channelid = file.ReadLine();
            }
        }
        private async void Start()
        {
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--disable-web-security", "--allow-file-access-from-files", "--allow-file-access");
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            await webView21.EnsureCoreWebView2Async(environment);
            webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", "assets", CoreWebView2HostResourceAccessKind.DenyCors);
            webView21.CoreWebView2.Settings.AreDevToolsEnabled = true;
            webView21.CoreWebView2.AddHostObjectToScript("bridge", new Bridge());
            webView21.Source = new Uri("https://appassets/index.html");
            webView21.Dock = DockStyle.Fill;
            webView21.DefaultBackgroundColor = Color.Transparent;
            this.Controls.Add(webView21);
            webView21.NavigationCompleted += WebView21_NavigationCompleted;
            if (!File.Exists(Application.StartupPath + @"\ChatOverlay.exe.WebView2\EBWebView\Default\IndexedDB\https_www.youtube.com_0.indexeddb.leveldb/LOG.old"))
            {
                this.TransparencyKey = Color.Empty;
            }
        }
        private async void WebView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (webView21.Source == new Uri("https://appassets/index.html"))
            {
                webView21.ExecuteScriptAsync("getLoadPage('apikey', 'channelid');".Replace("apikey", apikey).Replace("channelid", channelid)).ConfigureAwait(false);
            }
            if (File.Exists(Application.StartupPath + @"\ChatOverlay.exe.WebView2\EBWebView\Default\IndexedDB\https_www.youtube.com_0.indexeddb.leveldb/LOG.old"))
            {
                try
                {
                    string stringinject = @"
                    var style = `<style>
                        body, html {
                            background-color: transparent !important;
                        }
                        * {
                            background-color: black !important;
                        }
                    </style>`;
                    document.getElementsByTagName('head')[0].innerHTML += style;
                    ";
                    await execScriptHelper(stringinject);
                }
                catch { }
            }
        }
        private async Task<String> execScriptHelper(String script)
        {
            var x = await webView21.ExecuteScriptAsync(script).ConfigureAwait(false);
            return x;
        }
        private async void timer2_Tick(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\ChatOverlay.exe.WebView2\EBWebView\Default\IndexedDB\https_www.youtube.com_0.indexeddb.leveldb/LOG.old"))
            {
                try
                {
                    string stringinject = @"
                    if (window.location.href.indexOf('youtube') > -1 | window.location.href.indexOf('youtu.be') > -1) {
                        try {
                            var playButton = document.querySelector('.ytp-large-play-button:visible');
                            if (playButton) {
                                playButton.click();
                            }
                        }
                        catch { }
                        try {
                            var skipButton = document.querySelector('.ytp-ad-skip-button');
                            if (skipButton) {
                                skipButton.click();
                            }
                        }
                        catch { }
                        try {
                            var closeButton = document.querySelector('.ytp-ad-overlay-close-button');
                            if (closeButton) {
                                closeButton.click();
                            }
                        }
                        catch { }
                        try {
                            document.cookie = 'VISITOR_INFO1_LIVE = oKckVSqvaGw; path =/; domain =.youtube.com';
                            var cookies = document.cookie.split('; ');
                            for (var i = 0; i < cookies.length; i++)
                            {
                                var cookie = cookies[i];
                                var eqPos = cookie.indexOf('=');
                                var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
                                document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT';
                            }
                        }
                        catch { }
                        try {
                            var els = document.getElementsByClassName('video-ads ytp-ad-module');
                            for (var i=0;i<els.length; i++) {
                                els[i].click();
                            }
                        }
                        catch { }
                        try {
                            var el = document.getElementsByClassName('ytp-ad-skip-button');
                            for (var i=0;i<el.length; i++) {
                                el[i].click();
                            }
                        }
                        catch { }
                        try {
                            var element = document.getElementsByClassName('ytp-ad-overlay-close-button');
                            for (var i=0;i<element.length; i++) {
                                element[i].click();
                            }
                        }
                        catch { }
                        try {
                            var scripts = document.getElementsByTagName('script');
                            for (let i = 0; i < scripts.length; i++)
                            {
                                var content = scripts[i].innerHTML;
                                if (content.indexOf('ytp-ad') > -1) {
                                    scripts[i].innerHTML = '';
                                }
                                var src = scripts[i].getAttribute('src');
                                if (src.indexOf('ytp-ad') > -1) {
                                    scripts[i].setAttribute('src', '');
                                }
                            }
                        }
                        catch { }
                        try {
                            var iframes = document.getElementsByTagName('iframe');
                            for (let i = 0; i < iframes.length; i++)
                            {
                                var content = iframes[i].innerHTML;
                                if (content.indexOf('ytp-ad') > -1) {
                                    iframes[i].innerHTML = '';
                                }
                                var src = iframes[i].getAttribute('src');
                                if (src.indexOf('ytp-ad') > -1) {
                                    iframes[i].setAttribute('src', '');
                                }
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#primary');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = 'none';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#container');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = 'none';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#related');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = 'none';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#panels');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = 'none';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('ytd-playlist-panel-renderer');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = 'none';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#donation-shelf');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = 'none';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#background');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = 'none';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#show-hide-button');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = 'none';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#columns');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = '';
                                allelements[i].classList.remove('ytd-watch-flexy');
                                allelements[i].style.width = '400px';
                                allelements[i].style.height = '600px';
                                allelements[i].style.minWidth = '400px';
                                allelements[i].style.minHeight = '600px';
                                allelements[i].style.padding = '0px';
                                allelements[i].style.positionTop = '0px';
                                allelements[i].style.positionLeft = '0px';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#content');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = '';
                                allelements[i].classList.remove('ytd-watch-flexy');
                                allelements[i].style.width = '400px';
                                allelements[i].style.height = '600px';
                                allelements[i].style.minWidth = '400px';
                                allelements[i].style.minHeight = '600px';
                                allelements[i].style.padding = '0px';
                                allelements[i].style.positionTop = '0px';
                                allelements[i].style.positionLeft = '0px';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('ytd-app');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.display = '';
                                allelements[i].classList.remove('ytd-watch-flexy');
                                allelements[i].style.width = '400px';
                                allelements[i].style.height = '600px';
                                allelements[i].style.minWidth = '400px';
                                allelements[i].style.minHeight = '600px';
                                allelements[i].style.padding = '0px';
                                allelements[i].style.positionTop = '0px';
                                allelements[i].style.positionLeft = '0px';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#secondary');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.width = '400px';
                                allelements[i].style.height = '600px';
                                allelements[i].style.padding = '0px';
                                allelements[i].style.positionTop = '0px';
                                allelements[i].style.positionLeft = '0px';
                            }
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('#chat-container');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.width = '400px';
                                allelements[i].style.height = '600px';
                                allelements[i].style.padding = '0px';
                                allelements[i].style.positionTop = '0px';
                                allelements[i].style.positionLeft = '0px';
                            }
                        }
                        catch { }
                        try {
                            document.documentElement.style.backgroundColor = 'transparent !important';
                        }
                        catch { }
                        try {
                            var allelements = document.querySelectorAll('*');
                            for (var i = 0; i < allelements.length; i++) {
                                allelements[i].style.background = 'transparent';
                                allelements[i].style.backgroundColor = 'transparent !important';
                                allelements[i].style.margin = '0px';
                                allelements[i].style.overflowY = 'hidden';
                                allelements[i].removeAttribute('darker-dark-theme');
                                allelements[i].removeAttribute('darker-dark-theme-deprecate');
                                allelements[i].removeAttribute('dark');
                                allelements[i].style.backgroundColor = '';
                            }
                        }
                        catch { }
                    }
                    ";
                    await execScriptHelper(stringinject);
                }
                catch { }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            valchanged(0, GetAsyncKeyState(Keys.PageUp));
            if (wd[0] == 1)
            {
                this.TopMost = false;
            }
            valchanged(1, GetAsyncKeyState(Keys.PageDown));
            if (wd[1] == 1)
            {
                width = Screen.PrimaryScreen.Bounds.Width;
                height = Screen.PrimaryScreen.Bounds.Height;
                this.Size = new Size(width, height);
                this.Location = new Point(0, 0);
                this.TopMost = true;
            }
            valchanged(2, GetAsyncKeyState(Keys.Add));
            if (wd[2] == 1)
            {
                if (!getstate)
                {
                    Start();
                    getstate = true;
                }
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            webView21.Dispose();
        }
    }
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class Bridge
    {
        public static Form1 form1 = new Form1();
        public static string txt;
        public string LoadPage(string param)
        {
            Form1.webView21.ExecuteScriptAsync("reLoadPlayer();").ConfigureAwait(false);
            return param;
        }
    }
}