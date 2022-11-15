using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using NetFwTypeLib;
using EO.WebBrowser;
namespace Streamchat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static int width = Screen.PrimaryScreen.Bounds.Width;
        private static int height = Screen.PrimaryScreen.Bounds.Height;
        public static Form1 form = (Form1)Application.OpenForms["Form1"];
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.pictureBox1.Dock = DockStyle.Fill;
            EO.WebEngine.BrowserOptions options = new EO.WebEngine.BrowserOptions();
            options.EnableWebSecurity = false;
            EO.WebBrowser.Runtime.DefaultEngineOptions.SetDefaultBrowserOptions(options);
            EO.WebEngine.Engine.Default.Options.AllowProprietaryMediaFormats();
            EO.WebEngine.Engine.Default.Options.SetDefaultBrowserOptions(new EO.WebEngine.BrowserOptions
            {
                EnableWebSecurity = false
            });
            this.webView1.Create(pictureBox1.Handle);
            this.webView1.Engine.Options.AllowProprietaryMediaFormats();
            this.webView1.SetOptions(new EO.WebEngine.BrowserOptions
            {
                EnableWebSecurity = false
            });
            this.webView1.Engine.Options.DisableGPU = false;
            this.webView1.Engine.Options.DisableSpellChecker = true;
            this.webView1.Engine.Options.CustomUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            Navigate("https://www.twitch.tv/login?popup=false");
        }
        private void webView1_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            Task.Run(() => LoadPage());
        }

        private void webView1_UrlChanged(object sender, EventArgs e)
        {
            if (webView1.Url != "https://www.twitch.tv/login?popup=false") 
            {
                Navigate("https://michaelandrefraniatte.web.app/ppia/"); 
            }
        }
        private void LoadPage()
        {
            if (webView1.Url == "https://michaelandrefraniatte.web.app/ppia/")
            {
                string stringinject;
                stringinject = @"
    <style>
body {
    font-family: sans-serif;
    background-color: #222222;
    color: #FFFFFF;
    overflow-y: hidden;
}

.row > .col-lg-4,
.col-6 {
    padding: 0;
}

#onair {
    position: absolute;
    background-color: #9147FF;
    outline: none;
    border-radius: 100%;
    border: none;
    width: 15px;
    height: 15px;
    left: 20px;
    top: 20px;
}

.folderopen {
    cursor: pointer;
    text-align: center;
    color: white;
    overflow: hidden;
}

.icon-upload > input {
    display: none;
}
    </style>
".Replace("\r\n", " ");
                stringinject = @"""" + stringinject + @"""";
                stringinject = @"$(" + stringinject + @" ).appendTo('head');";
                this.webView1.EvalScript(stringinject);
                stringinject = @"
    <div class='chat-container'>
    </div>

    <div class='onair-container' id='onair'>
    </div>

    <div class='open-container'>
        <div class='icon-upload' style='display:float;'>
            <label for='txtFileInput'>
                <div class='bg-light folderopen' style='display:float;position:absolute;left:15px;bottom:10px;'>
                    <i class='fa fa-folder-open' title='open favours'></i>
                </div>
            </label>
            <input type='file' id='txtFileInput' onchange='handleFiles(this.files)' accept='.txt'>
        </div>
    </div>

    <script>

var channel = '';
var parent = 'michaelandrefraniatte.web.app';
var redirecturi = 'https://michaelandrefraniatte.web.app/ppia';
var clientid = 'sgkzvz71pmvl54d1h2hv9j8a4ov00t';
var bearer = 'yr4vxobuaftfigconk3otngziyu0gg';
var token = '';
var resy = window.screen.availHeight - 70;
var element = document.getElementById('onair');

function changeTitle() {
    document.title = 'streamchat by michael franiatte';
}

$(function() {
    changeTitle();
    var getitem = localStorage.getItem('streamchat');
    if (getitem == '' | getitem == null | getitem == 'undefined') {
        localStorage.setItem('streamchat', '[]');
    }
    var obj = JSON.parse(getitem);
    channel = obj.channel;
    createChat();
    getId();
});

function createChat() {
    var htmlString = '';
    htmlString += `<iframe src=\'https://www.twitch.tv/embed/${channel}/chat?darkpopout&parent=${parent}\' height=\'` + resy + `\' width=\'100%\'></iframe>`;
    $('.chat-container').append(htmlString);
}

function getId() {
    $.ajax({
            type: 'POST',
            url: 'https://id.twitch.tv/oauth2/token?client_id=' + clientid + '&client_secret=' + bearer + '&grant_type=client_credentials',
            success: function(datas) {
                token = 'Bearer ' + datas['access_token'];
                onAir();
                setInterval(function(){
                onAir();
            }, 10000);
        }
    });
}

function onAir() {
    $.ajax({
        type: 'GET',
        url: 'https://api.twitch.tv/helix/users?login=' + channel,
        headers: {
            'Authorization': token,
            'Client-Id': clientid
        },
        success: function(datas) {
            if (datas.stream == null) {      
                element.style.display = 'none';
            }
            else {
                $('#onair').css('display', '');
            }
        }
    });
}

$(function() {
    $('#txtFileInput').click(function(){
        $(this).val('');
    });
});

function handleFiles(files) {
	getAsText(files[0]); 
}

function getAsText(fileToRead) {
	var reader = new FileReader();
	reader.onload = loadHandler;
	reader.onerror = errorHandler;   
	reader.readAsText(fileToRead);
}

function loadHandler(event) {
	var txt = event.target.result;
	processData(txt);     
}

function errorHandler(evt) {
	if(evt.target.error.name == 'NotReadableError') {
	}
}

function processData(txt) {
    var allTextLines = txt.split(', ');
    localStorage.setItem('streamchat', JSON.stringify({channel: allTextLines[0]}));
    location.reload();
}
    </script>
".Replace("\r\n", " ");
                stringinject = @"""" + stringinject + @"""";
                stringinject = @"$(document).ready(function(){$('body').append(" + stringinject + @");});";
                this.webView1.EvalScript(stringinject);
            }
        }
        private void Navigate(string address)
        {
            if (String.IsNullOrEmpty(address))
                return;
            if (address.Equals("about:blank"))
                return;
            if (!address.StartsWith("http://") & !address.StartsWith("https://"))
                address = "https://" + address;
            try
            {
                webView1.Url = address;
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.webView1.Dispose();
        }
    }
}