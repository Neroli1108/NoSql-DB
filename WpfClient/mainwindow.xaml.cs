///////////////////////////////////////////////////////////////////////////
// WPFwriteClient.xaml.cs                                               //
// ver 1.0                                                             //
// Yunding LI, CSE681 - Software Modeling and Analysis, Project #4    //
///////////////////////////////////////////////////////////////////////

/* Plans:
 * - Query Message from the server
 *
 * Maintenance History:
 * --------------------
 * * ver 1.0 : 17 Nov 2015
 * - first release
 * - Based on the WPFReadClient
 */
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
using System.Threading;
using Project4Starter;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Threading;



namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        static bool firstConnect = true;
        static Receiver rcvr = null;
        static wpfSender sndr = null;
        string localAddress = "localhost";
        string localPort = "8089";
        string remoteAddress = "localhost";
        string remotePort = "8080";
        Stopwatch Timer = new Stopwatch();
        Message rmsg = null;
        private void Addlog(string content)
        {
            Log.Text += content + "\n";
        }


        /////////////////////////////////////////////////////////////////////
        // nested class wpfSender used to override Sender message handling
        // - routes messages to status textbox
        public class wpfSender : Sender
        {
            TextBox lStat_ = null;  // reference to UIs local status textbox
            System.Windows.Threading.Dispatcher dispatcher_ = null;

            public wpfSender(TextBox lStat, System.Windows.Threading.Dispatcher dispatcher)
            {
                dispatcher_ = dispatcher;  // use to send results action to main UI thread
                lStat_ = lStat;
            }
            public override void sendMsgNotify(string msg)
            {
                Action act = () => { lStat_.Text = msg; };
                dispatcher_.Invoke(act);

            }
            public override void sendExceptionNotify(Exception ex, string msg = "")
            {
                Action act = () => { lStat_.Text = ex.Message; };
                dispatcher_.Invoke(act);
            }
            public override void sendAttemptNotify(int attemptNumber)
            {
                Action act = null;
                act = () => { lStat_.Text = String.Format("attempt to send #{0}", attemptNumber); };
                dispatcher_.Invoke(act);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            lAddr.Text = localAddress;
            lPort.Text = localPort;
            rAddr.Text = remoteAddress;
            rPort.Text = remotePort;
            Title = "WPF Write Client";
            send.IsEnabled = false;

        }
        //----< trim off leading and trailing white space >------------------

        string trim(string msg)
        {
            StringBuilder sb = new StringBuilder(msg);
            for (int i = 0; i < sb.Length; ++i)
                if (sb[i] == '\n')
                    sb.Remove(i, 1);
            return sb.ToString().Trim();
        }
        //----< indirectly used by child receive thread to post results >----

        public void postRcvMsg(string content)
        {
            TextBlock item = new TextBlock();
            item.Text = trim(content);
            item.FontSize = 16;
            rcvmsgs.Items.Insert(0, item);

        }
        //----< used by main thread >----------------------------------------

        public void postSndMsg(string content)
        {
            TextBlock item = new TextBlock();
            item.Text = trim(content);
            item.FontSize = 16;
            sndmsgs.Items.Insert(0, item);
        }
        //----< get Receiver and Sender running >----------------------------

        void setupChannel()
        {
            rcvr = new Receiver(localPort, localAddress);
            Action serviceAction = () =>
            {
                try
                {

                    while (true)
                    {
                        rmsg = rcvr.getMessage();
                        Action act = () => { postRcvMsg(rmsg.content); postRcvMsg2(rmsg.Rcontent); postRcvMsg3(rmsg.Rcontent); postRcvMsg4(rmsg.Tcontent); };
                        Dispatcher.Invoke(act);
                    }
                }
                catch (Exception ex)
                {
                    Action act = () => { lStat.Text = ex.Message; };
                    Dispatcher.Invoke(act);
                }
            };
            if (rcvr.StartService())
            {
                rcvr.doService(serviceAction);
            }

            sndr = new wpfSender(lStat, this.Dispatcher);
        }
        //----< set up channel after entering ports and addresses >----------

        private void start_Click(object sender, RoutedEventArgs e)
        {
            localPort = lPort.Text;
            localAddress = lAddr.Text;
            remoteAddress = rAddr.Text;
            remotePort = rPort.Text;

            if (firstConnect)
            {
                firstConnect = false;
                if (rcvr != null)
                    rcvr.shutDown();
                setupChannel();
            }
            rStat.Text = "connect setup";
            send.IsEnabled = true;
            connect.IsEnabled = false;
            lPort.IsEnabled = false;

            lAddr.IsEnabled = false;
        }
        //----< send a demonstraton message >--------------------------------



        private void send_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!remoteAddress.Equals(rAddr.Text) || !remotePort.Equals(rPort.Text))
                {
                    remoteAddress = rAddr.Text;
                    remotePort = rPort.Text;
                }
                // - Make a demo message to send
                // - You will need to change MessageMaker.makeMessage
                //   to make messages appropriate for your application design
                // - You might include a message maker tab on the UI
                //   to do this.

                MessageMaker maker = new MessageMaker();
                Message msg = maker.makeMessage(
                  Utilities.makeUrl(lAddr.Text, lPort.Text),
                  Utilities.makeUrl(rAddr.Text, rPort.Text));
                msg.Rcontent = " ";//msg.Rcontent is the new datamember ,the function of it is to transmit the information
                lStat.Text = "sending to" + msg.toUrl;
                sndr.localUrl = msg.fromUrl;
                sndr.remoteUrl = msg.toUrl;
                lStat.Text = "attempting to connect";
                if (sndr.sendMessage(msg))
                    lStat.Text = "connected";
                else
                    lStat.Text = "connect failed";
                postSndMsg(msg.content);
            }
            catch (Exception ex)
            {
                lStat.Text = ex.Message;
            }
        }

        //-< Click the Excute button to send the add, delete, edit, edit children, persist and restore message. The timer will record the Time>-
        private void Excute_Click(object sender, RoutedEventArgs e)
        {
            Timer.Reset();
            Timer.Start();
            Add();
            Delete();
            Edit();
            Editchildren();
            Persist();
            Restore();
            Timer.Stop();
            Time.Text = Timer.Elapsed.ToString();
        }


        //-< Add KEY function>- 
        private void Add()
        {
            MessageMaker Maker = new MessageMaker();
            Message msg = Maker.makeMessage(
            Utilities.makeUrl(lAddr.Text, lPort.Text),
            Utilities.makeUrl(rAddr.Text, rPort.Text));
            msg.Rcontent = WriteClient.AddKey();//msg.Rcontent send the add message
            sndr.localUrl = msg.fromUrl;
            sndr.remoteUrl = msg.toUrl;
            if (sndr.sendMessage(msg))
                lStat.Text = "connected";
            else
                lStat.Text = "connect failed";
            postSndMsg(msg.content);
            Addlog("Send the Add Key = 3 message");
        }
        //-<Delete KEY operation>-
        private void Delete()
        {
            MessageMaker Maker = new MessageMaker();
            Message msg = Maker.makeMessage(
         Utilities.makeUrl(lAddr.Text, lPort.Text),
         Utilities.makeUrl(rAddr.Text, rPort.Text));
            msg.Rcontent = WriteClient.DeleteKey();//msg.Rcontent send the delete message
            sndr.localUrl = msg.fromUrl;
            sndr.remoteUrl = msg.toUrl;
            if (sndr.sendMessage(msg))
                lStat.Text = "connected";
            else
                lStat.Text = "connect failed";
            postSndMsg(msg.content);
            Addlog("Send the message: Delete the Key ");
        }
        //-<Edit KEY function>-
        private void Edit()
        {

            MessageMaker Maker = new MessageMaker();
            Message msg = Maker.makeMessage(
         Utilities.makeUrl(lAddr.Text, lPort.Text),
         Utilities.makeUrl(rAddr.Text, rPort.Text));
            msg.Rcontent = WriteClient.EditKey();//msg.Rcontent send the edit the key message 
            sndr.localUrl = msg.fromUrl;
            sndr.remoteUrl = msg.toUrl;
            if (sndr.sendMessage(msg))
                lStat.Text = "connected";
            else
                lStat.Text = "connect failed";
            postSndMsg(msg.content);
            Addlog("Send the message: Edit the Key ");
        }

        //-<Edit the children function>-
        private void Editchildren()
        {
            MessageMaker Maker = new MessageMaker();
            Message msg = Maker.makeMessage(
         Utilities.makeUrl(lAddr.Text, lPort.Text),
         Utilities.makeUrl(rAddr.Text, rPort.Text));
            msg.Rcontent = WriteClient.EditChidren();//msg.Rcontent send the edit the Children message 
            sndr.localUrl = msg.fromUrl;
            sndr.remoteUrl = msg.toUrl;
            if (sndr.sendMessage(msg))
                lStat.Text = "connected";
            else
                lStat.Text = "connect failed";
            postSndMsg(msg.content);
            Addlog("Send the message: Edit the Children ");
        }

        //-< Persist Function>-
        private void Persist()
        {
            MessageMaker Maker = new MessageMaker();
            Message msg = Maker.makeMessage(
         Utilities.makeUrl(lAddr.Text, lPort.Text),
         Utilities.makeUrl(rAddr.Text, rPort.Text));
            msg.Rcontent = "Persist";
            sndr.localUrl = msg.fromUrl;
            sndr.remoteUrl = msg.toUrl;
            if (sndr.sendMessage(msg))
                lStat.Text = "connected";
            else
                lStat.Text = "connect failed";
            postSndMsg(msg.content);
            Addlog("Send the message: Persist the database from xml file ");
        }
        //-<Restore function>-
        private void Restore()
        {
            MessageMaker Maker = new MessageMaker();
            Message msg = Maker.makeMessage(
         Utilities.makeUrl(lAddr.Text, lPort.Text),
         Utilities.makeUrl(rAddr.Text, rPort.Text));
            msg.Rcontent = "Restore a xml";
            sndr.localUrl = msg.fromUrl;
            sndr.remoteUrl = msg.toUrl;
            if (sndr.sendMessage(msg))
                lStat.Text = "connected";
            else
                lStat.Text = "connect failed";
            postSndMsg(msg.content);
            Addlog("Send the message: restore the database from xml file ");
        }
        //-<show the Receive content in the ResultMsg box>-
        public void postRcvMsg2(string content)
        {
            ResultMsg.FontSize = 14.0f;
            ResultMsg.AppendText(content);
        }
        //-<show the Receive content in the Demo box>-
        public void postRcvMsg3(string content)
        {
            Demo.FontSize = 14.0f;
            Demo.AppendText(content);
        }
        //-<show the Receive content in the SvrTime box>-
        public void postRcvMsg4(string content)
        {
            SvrTime.FontSize = 14.0f;
            SvrTime.AppendText(content);
        }
        //-<This is the function that make the wpfwrite to run automatically>-
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Environment.CommandLine.Split().Any(x => x == "-L" || x == "-l"))
            {
                Utilities.verbose = true;
            }
            else
            {
                Utilities.verbose = false;
            }
            start_Click(connect, new RoutedEventArgs());
            send_Click(send, new RoutedEventArgs());
            T();
        }
        //-<wait the Excute_click until connect>-
        void T()
        {
            var tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(3);
            tmr.Tick += (_, e) =>
            {
                tmr.Stop();
                Excute_Click(Excute, new RoutedEventArgs());
            };
            tmr.Start();
        }


    }
}
