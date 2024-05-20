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
using System.Threading;
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
        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll")]
        static extern bool DrawMenuBar(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
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
        private static ThreadStart threadstart;
        private static Thread thread;
        public static WebView2 webView21 = new WebView2();
        private static int width = Screen.PrimaryScreen.Bounds.Width;
        private static int height = Screen.PrimaryScreen.Bounds.Height;
        private static string apikey, channelid;
        private static bool getstate = false;
        private const int GWL_STYLE = -16;
        private const uint WS_BORDER = 0x00800000;
        private const uint WS_CAPTION = 0x00C00000;
        private const uint WS_SYSMENU = 0x00080000;
        private const uint WS_MINIMIZEBOX = 0x00020000;
        private const uint WS_MAXIMIZEBOX = 0x00010000;
        private const uint WS_OVERLAPPED = 0x00000000;
        private const uint WS_POPUP = 0x80000000;
        private const uint WS_TABSTOP = 0x00010000;
        private const uint WS_VISIBLE = 0x10000000;
        public static int[] wd = { 2, 2, 2 };
        public static int[] wu = { 2, 2, 2 };
        public static bool[] ws = { false, false, false };
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
            this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }
        private async void Start()
        {
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--disable-web-security --allow-file-access-from-files --allow-file-access", "en");
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            await webView21.EnsureCoreWebView2Async(environment);
            webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", "assets", CoreWebView2HostResourceAccessKind.DenyCors);
            webView21.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView21.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView21.CoreWebView2.AddHostObjectToScript("bridge", new Bridge());
            webView21.Source = new Uri("https://appassets/index.html");
            webView21.Dock = DockStyle.Fill;
            webView21.DefaultBackgroundColor = Color.Transparent;
            webView21.KeyDown += WebView21_KeyDown;
            this.Controls.Add(webView21);
            webView21.NavigationCompleted += WebView21_NavigationCompleted;
            if (File.Exists(Application.StartupPath + @"\ChatOverlay.exe.WebView2\EBWebView\Default\IndexedDB\https_www.youtube.com_0.indexeddb.leveldb/LOG.old"))
            {
                threadstart = new ThreadStart(ShowStream);
                thread = new Thread(threadstart);
                thread.Start();
            }
            else
            {
                this.TransparencyKey = Color.Empty;
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
                this.Controls.Remove(this.pictureBox1);
                this.pictureBox1.Dispose();
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e.KeyData);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            OnKeyDown(keyData);
            return true;
        }
        private void WebView21_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e.KeyData);
        }
        private void OnKeyDown(Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                const string message = "• Author: Michaël André Franiatte.\n\r\n\r• Contact: michael.franiatte@gmail.com.\n\r\n\r• Publisher: https://github.com/michaelandrefraniatte.\n\r\n\r• Copyrights: All rights reserved, no permissions granted.\n\r\n\r• License: Not open source, not free of charge to use.";
                const string caption = "About";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ShowStream()
        {
            System.Threading.Thread.Sleep(20000);
            this.pictureBox1.BackColor = Color.Magenta;
            this.pictureBox1.Image.Dispose();
            this.pictureBox1.Image = null;
            this.Controls.Remove(this.pictureBox1);
            this.pictureBox1.Dispose();
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
                            var skipButton = document.querySelector('.ytp-ad-skip-button-modern');
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
                            var allelements = document.querySelectorAll('*');
                            for (var i = 0; i < allelements.length; i++) {
                                var classname = allelements[i].className;
                                if (classname.indexOf('ytp-ad') > -1 | classname.indexOf('-ad-') > -1 | classname.indexOf('ad-') > -1 | classname.indexOf('ads-') > -1 | classname.indexOf('ad-showing') > -1 | classname.indexOf('ad-container') > -1 | classname.indexOf('ytp-ad-overlay-open') > -1 | classname.indexOf('video-ads') > -1)  {
                                    allelements[i].innerHTML = '';
                                }
                            }
                        }
                        catch { }
                        try {
                            var players = document.getElementById('movie_player');
                            for (let i = 0; i < players.length; i++) {
                                players.classList.remove('ad-interrupting');
                                players.classList.remove('playing-mode');
                                players.classList.remove('ytp-autohide');
                                players.classList.add('ytp-hide-info-bar');
                                players.classList.add('playing-mode');
                                players.classList.add('ytp-autohide');
                            }
                        }
                        catch { }
                        try {
                            var fabelements = document.querySelectorAll('yt-reaction-control-panel-button-view-model');
                            for (var i = 0; i < fabelements.length; i++) {
                                    fabelements[i].innerHTML = '';
                            }
                        }
                        catch { }
                        try {
                            var fabelement = document.querySelector('#fab-container');
                            fabelement.innerHTML = '';
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('.ad-showing');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('.ad-container');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('.ytp-ad-overlay-open');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('.video-ads');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('.ytp-ad-overlay-image');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('.ytp-ad-overlay-container');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('.ytd-carousel-ad-renderer');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('ytd-ad-slot-renderer');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('ytd-action-companion-ad-renderer');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('ytd-engagement-panel-section-list-renderer');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('ytd-player-legacy-desktop-watch-ads-renderer');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('yt-reaction-control-panel-button-view-model');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('tp-yt-paper-dialog');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('ytd-statement-banner-renderer');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelectorAll('ytd-brand-video-singleton-renderer');
                            contents.forEach(elem => elem.style.display = 'none');
                        }
                        catch { }
                        try {
                            var contents = document.querySelector('#reaction-control-panel').style.display = 'none';
                        }
                        catch { }
                        try {
                            var contents = document.querySelector('#emoji-fountain').style.display = 'none';
                        }
                        catch { }
                        try {
                            var contents = document.querySelector('#fab-container').style.display = 'none';
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
            valchanged(0, GetAsyncKeyState(Keys.PageDown));
            if (wd[0] == 1)
            {
                int width = Screen.PrimaryScreen.Bounds.Width;
                int height = Screen.PrimaryScreen.Bounds.Height;
                IntPtr window = GetForegroundWindow();
                SetWindowLong(window, GWL_STYLE, WS_SYSMENU);
                SetWindowPos(window, -2, 0, 0, width, height, 0x0040);
                DrawMenuBar(window);
                width = Screen.PrimaryScreen.Bounds.Width;
                height = Screen.PrimaryScreen.Bounds.Height;
                this.Size = new Size(width, height);
                this.Location = new Point(0, 0);
                this.TopMost = true;
            }
            valchanged(1, GetAsyncKeyState(Keys.PageUp));
            if (wd[1] == 1)
            {
                IntPtr window = GetForegroundWindow();
                SetWindowLong(window, GWL_STYLE, WS_CAPTION | WS_POPUP | WS_BORDER | WS_SYSMENU | WS_TABSTOP | WS_VISIBLE | WS_OVERLAPPED | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
                DrawMenuBar(window);
                this.TopMost = false;
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