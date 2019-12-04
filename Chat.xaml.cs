using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using NewOptics.Administrateur;
using NewOptics.SVC;
using System.ServiceModel;
using System.IO;
using Microsoft.Win32;

using MahApps.Metro.Controls;

using System.Threading.Tasks;

using System.Windows.Threading;

namespace NewOptics
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        SVC.ServiceCliniqueClient proxy;
        SVC.Client receiver = null;
        SVC.Client localClient = null;
        private delegate void FaultedInvokerChat();
        string rcvFilesPath = @"C:/MedicaLogitechTransferedFiles/";
        SVC.MembershipOptic Membershipsession;
        ///  ICallback callback;
        Dictionary<ListBoxItem, SVC.Client> OnlineClients = new Dictionary<ListBoxItem, Client>();
        public Chat(SVC.Client LOCALCLIENTRECU, ICallback callbackrecu, SVC.ServiceCliniqueClient proxy1, DXWindowMain main, MembershipOptic memberrecu)
        {
            try
            {
                InitializeComponent();
                //  proxy.ReConnectAsync(LOCALCLIENTRECU);
                callbackrecu.callback += new ICallback.CallbackEventHandler(callbackrecu_callback);
                callbackrecu.callbackUserjoin += new ICallback.CallbackEventHandler1(callbackrecu_callbackjoin);
                callbackrecu.callbackUserLeave += new ICallback.CallbackEventHandler2(callbackrecu_callbackleave);
                callbackrecu.callbackMessageRecu += new ICallback.CallbackEventHandler3(callbackrecu_callbackReceive);
                callbackrecu.IsWritingCallbackEvent += new ICallback.CallbackEventHandler4(callbackrecu_callbackIswrite);
                callbackrecu.InsertReceiveFileCallbackevent += new ICallback.CallbackEventHandler47(callbackrecureceiveFile_callbackIswrite);
                callbackrecu.InsertReceiveWhisperCallbackevent += new ICallback.CallbackEventHandler48(callbackrecureceiveWhisper_callbackWhisperReceive);

                Membershipsession = memberrecu;

                chatListBoxNames.SelectionChanged += new SelectionChangedEventHandler(chatListBoxNames_SelectionChanged);
                chatTxtBoxType.KeyDown += new KeyEventHandler(chatTxtBoxType_KeyDown);
                chatTxtBoxType.KeyUp += new KeyEventHandler(chatTxtBoxType_KeyUp);
                proxy = proxy1;
                localClient = LOCALCLIENTRECU;

                chatLabelCurrentUName.Content = localClient.UserName;
                main.AddEvent += new EventHandler(addClientWindow_AddEvent);
                proxy.ReConnectAsync(LOCALCLIENTRECU);
                proxy.InnerDuplexChannel.Faulted += new EventHandler(InnerDuplexChannel_Faulted);
                proxy.InnerDuplexChannel.Closed += new EventHandler(InnerDuplexChannel_Closed);
                DirectoryInfo dir = new DirectoryInfo(rcvFilesPath);
                dir.Create();

                /* var ist = proxy.GetAllClientDict();

                  foreach(var Item in ist.ToList())
                  {
                      MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("List Dictionnary "  +Item.Key.UserName.ToString()+" "+Item.Value.ToString(), NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);

                  }

                 List<Client> ist1 = proxy.GetAllClient();

                 foreach(var Item in ist1.ToList())
                 {
                     MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("LIST CLIENT "+Item.UserName.ToString()+" ", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.YesNo, MessageBoxImage.Question);

                 } */

            }
            catch (Exception ex)

            {
                HandleProxy();
            }

        }
        void InnerDuplexChannel_Closed(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerChat(HandleProxy));
                return;
            }
            HandleProxy();
        }

        void InnerDuplexChannel_Faulted(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new FaultedInvokerChat(HandleProxy));
                return;
            }
            HandleProxy();
        }
        private void HandleProxy()
        {
            if (proxy != null)
            {
                switch (this.proxy.State)
                {
                    case CommunicationState.Closed:
                        proxy.Abort();
                        proxy = null;
                        this.Close();
                        break;
                    case CommunicationState.Closing:
                        break;
                    case CommunicationState.Created:
                        break;
                    case CommunicationState.Faulted:
                        proxy.Abort();
                        proxy = null;
                        this.Close();
                        break;
                    case CommunicationState.Opened:


                        break;
                    case CommunicationState.Opening:
                        break;
                    default:
                        break;
                }
            }

        }


        void addClientWindow_AddEvent(object sender, System.EventArgs e)
        {
            this.Close();

        }
        /*******************************************************************/
        void callbackrecu_callback(object source, CallbackEvent e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddMessage(e.clients);
                }));
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        public void AddMessage(List<Client> clients)
        {
            try
            {
                chatListBoxNames.Items.Clear();
                OnlineClients.Clear();
                foreach (SVC.Client c in clients)
                {
                    ListBoxItem item = MakeItem(c.UserName);
                    chatListBoxNames.Items.Add(item);
                    OnlineClients.Add(item, c);
                    //  ListeClients.Add(c);
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        /***************************************************************/
        void callbackrecu_callbackjoin(object source, CallbackEventJoin e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    Addjoin(e.clientsj);
                }));
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        public void Addjoin(Client client)
        {
            try
            {
                //  if (client.Status == true)
                // {
                ListBoxItem item = MakeItem("------------ " + client.UserName + " vient d'ouvrir sa session ------------");
                item.Foreground = Brushes.BlueViolet;
                chatListBoxMsgs.Items.Add(item);
                ScrollViewer sv = FindVisualChild(chatListBoxMsgs);
                sv.LineDown();
                //   }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        /***********************************************************************/
        void callbackrecu_callbackleave(object source, CallbackEventUserLeave e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    Addleave(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        public void Addleave(Client client)
        {
            try
            {
                //if (client.Status == true)
                // {
                ListBoxItem item = MakeItem("------------ " + client.UserName + " vient de quitter sa session ------------");
                item.Foreground = Brushes.SkyBlue;
                chatListBoxMsgs.Items.Add(item);
                ScrollViewer sv = FindVisualChild(chatListBoxMsgs);
                sv.LineDown();
                //  }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        /***********************************************/
        void callbackrecu_callbackReceive(object source, CallbackEventMessageRecu e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddMessageRecu(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        public void AddMessageRecu(Message msg)
        {
            try
            {
                if (Membershipsession.EnvoiReceptMessage == true)
                {
                    foreach (SVC.Client c in this.OnlineClients.Values)
                    {
                        if (c.UserName == msg.Sender && localClient.UserName == msg.Sender)
                        {
                            ListBoxItem item = MakeItem("Vous" + " : " + msg.Content);

                            chatListBoxMsgs.Items.Add(item);
                        }
                        else
                        {
                            if (c.UserName == msg.Sender && localClient.UserName != msg.Sender)
                            {
                                ListBoxItem item = MakeItem(msg.Sender + " : " + msg.Content);
                                item.Foreground = Brushes.Red;
                                chatListBoxMsgs.Items.Add(item);
                                this.WindowState = WindowState.Normal;
                                if (chatCheckBoxSonore.IsChecked == true)
                                {
                                    System.IO.Stream str = Properties.Resources.facebookme_lZ4iKc6J;
                                    System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                                    snd.Play();
                                }

                            }
                        }
                    }
                    ScrollViewer sv = FindVisualChild(chatListBoxMsgs);
                    sv.LineDown();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        /**********************************************************************************/
        void callbackrecu_callbackIswrite(object source, CallbackEventWriting e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    AddWriting(e.clientleav);
                }));
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        public void AddWriting(Client client)
        {
            try
            {
                if (client == null)
                {
                    chatLabelWritingMsg.Content = "";
                }
                else
                {
                    //   if (client.Status == true)
                    // {
                    chatLabelWritingMsg.Content += client.UserName + " est entrain d'ecrire ..";
                    // }
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        /***************************************************************************/

        void callbackrecureceiveFile_callbackIswrite(object source, CallbackEventReceiveFile e)

        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    ReceiverFile(e.clientleav, e.clientrec);
                }));
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        public void ReceiverFile(SVC.FileMessage fileMsg, SVC.Client receiver)
        {
            try
            {
                if (Membershipsession.EnvoiRécéptFichier == true)
                {
                    try
                    {
                        FileStream fileStrm = new FileStream(rcvFilesPath + fileMsg.FileName, FileMode.Create, FileAccess.ReadWrite);
                        fileStrm.Write(fileMsg.Data, 0, fileMsg.Data.Length);
                        chatLabelSendFileStatus.Content = "Fichier reçu, " + fileMsg.FileName;
                        this.WindowState = WindowState.Normal;
                        if (chatCheckBoxSonore.IsChecked == true)
                        {
                            System.IO.Stream str = Properties.Resources.facebookme_lZ4iKc6J;
                            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                            snd.Play();
                        }

                    }
                    catch (Exception ex)
                    {
                        chatLabelSendFileStatus.Content = ex.Message.ToString();
                    }
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }






        /***********************************************************************/

        void callbackrecureceiveWhisper_callbackWhisperReceive(object source, CallbackEventReceiveWhisper e)
        {
            try
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
                {
                    ReceiveWhisper(e.clientleav, e.clientrec);
                }));
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }



        public void ReceiveWhisper(SVC.Message msg, SVC.Client receiver)
        {
            try
            {
                if (Membershipsession.DiscussionPrivé == true)
                {
                    foreach (SVC.Client c in this.OnlineClients.Values)
                    {
                        if (c.UserName == msg.Sender)
                        {
                            //                    ListBoxItem item = MakeItem(" Message privé de la part de " + msg.Sender+" pour "+receiver.UserName +" :" + msg.Content );
                            ListBoxItem item = MakeItem(msg.Sender + "/" + receiver.UserName + " : " + msg.Content);

                            item.Foreground = Brushes.Green;
                            chatListBoxMsgs.Items.Add(item);
                            this.WindowState = WindowState.Normal;
                            if (chatCheckBoxSonore.IsChecked == true)
                            {
                                System.IO.Stream str = Properties.Resources.facebookme_lZ4iKc6J;
                                System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                                snd.Play();
                            }
                        }
                    }
                    ScrollViewer sv = FindVisualChild(chatListBoxMsgs);
                    sv.LineDown();
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        /************************************************************/
        private ScrollViewer FindVisualChild(DependencyObject obj)
        {

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is ScrollViewer)
                {
                    return (ScrollViewer)child;
                }
                else
                {
                    ScrollViewer childOfChild = FindVisualChild(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;

        }




        private ListBoxItem MakeItem(string text)
        {
            ListBoxItem item = new ListBoxItem();


            TextBlock txtblock = new TextBlock();
            txtblock.Text = text;
            txtblock.VerticalAlignment = VerticalAlignment.Center;

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            panel.Children.Add(item);
            panel.Children.Add(txtblock);

            ListBoxItem bigItem = new ListBoxItem();
            bigItem.Content = panel;

            return bigItem;
        }

        private void chatButtonSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Send();
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        private void Send()
        {
            try
            {
                if (proxy != null && chatTxtBoxType.Text != "")
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {

                    }
                    else
                    {
                        //Create message, assign its properties
                        SVC.Message msg = new Message();
                        msg.Sender = localClient.UserName;
                        msg.Content = chatTxtBoxType.Text.ToString();

                        //If whisper mode is checked and an item is
                        //selected in the list box of clients, it will
                        //arrange a client object called receiver
                        //to whisper
                        if ((bool)chatCheckBoxWhisper.IsChecked)
                        {
                            if (Membershipsession.DiscussionPrivé == true)
                            {
                                if (this.receiver != null)
                                {
                                    proxy.WhisperAsync(msg, this.receiver);
                                    chatTxtBoxType.Text = "";
                                    chatTxtBoxType.Focus();
                                }
                            }
                            else
                            {
                                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }
                        }

                        else
                        {
                            if (Membershipsession.EnvoiReceptMessage == true)
                            {
                                proxy.SayAsync(msg);
                                chatTxtBoxType.Text = "";
                                chatTxtBoxType.Focus();
                            }
                            else
                            {
                                MessageBoxResult result1 = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Information);

                            }

                        }
                        //Tell the service to tell back all clients that this client
                        //has just finished typing..
                        proxy.IsWritingAsync(null);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        private void chatButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                chatListBoxMsgs.Items.Clear();
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void chatButtonSendFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Membershipsession.EnvoiRécéptFichier == true)
                {
                    if (this.receiver != null && this.receiver.UserName != localClient.UserName)
                    {
                        Stream strm = null;
                        try
                        {
                            OpenFileDialog fileDialog = new OpenFileDialog();
                            fileDialog.Multiselect = false;

                            if (fileDialog.ShowDialog() == DialogResult.HasValue)
                            {
                                return;
                            }

                            strm = fileDialog.OpenFile();
                            if (strm != null)
                            {
                                byte[] buffer = new byte[(int)strm.Length];

                                int i = strm.Read(buffer, 0, buffer.Length);

                                if (i > 0)
                                {
                                    SVC.FileMessage fMsg = new FileMessage();
                                    fMsg.FileName = fileDialog.SafeFileName;
                                    fMsg.Sender = this.localClient.UserName;
                                    fMsg.Data = buffer;
                                    proxy.SendFile(fMsg, this.receiver);
                                    proxy.SendFileCompleted += new EventHandler<SendFileCompletedEventArgs>(proxy_SendFileCompleted);
                                    // chatLabelSendFileStatus.Content = "Transfert en cours....";
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            chatTxtBoxType.Text = ex.Message.ToString();
                        }
                        finally
                        {
                            if (strm != null)
                            {
                                strm.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show("Vous devez choisir un destinataire", NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                else
                {
                    MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(NewOptics.Properties.Resources.MessageBoxPrivilége, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        void proxy_SendFileCompleted(object sender, SendFileCompletedEventArgs e)
        {
            try
            {
                chatLabelSendFileStatus.Content = "Fichier envoyé";
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }
        private void chatButtonOpenReceived_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(rcvFilesPath);
            }
            catch (Exception ex)
            {
                MessageBoxResult result = Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, NewOptics.Properties.Resources.SiteWeb, MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void chatListBoxNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBoxItem item = chatListBoxNames.SelectedItem as ListBoxItem;
                if (item != null)
                {
                    this.receiver = this.OnlineClients[item];
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void chatTxtBoxType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {

                    }
                    else
                    {
                        if (e.Key == Key.Enter)
                        {
                            Send();
                        }
                        else if (chatTxtBoxType.Text.Length < 1)
                        {
                            proxy.IsWritingAsync(this.localClient);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

        private void chatTxtBoxType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (proxy != null)
                {
                    if (proxy.State == CommunicationState.Faulted)
                    {

                    }
                    else
                    {
                        if (chatTxtBoxType.Text.Length < 1)
                        {
                            proxy.IsWritingAsync(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleProxy();
            }
        }

    }

}
