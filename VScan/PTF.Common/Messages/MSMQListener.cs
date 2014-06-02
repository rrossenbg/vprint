/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Messaging;

namespace PremierTaxFree.PTFLib.Messages
{
    public delegate void MessageReceivedEventHandler(object sender, MessageEventArgs args);

    /// <summary>
    /// Listens in a message queue
    /// </summary>
    public class MSMQListener
    {
        private bool m_started;
        private MessageQueue m_queue;

        public event MessageReceivedEventHandler MessageReceived;

        public Type[] FormatterTypes { get; set; }

        public MSMQListener(string queuePath)
        {
            m_queue = new MessageQueue(queuePath);
        }

        /// <summary>
        /// Starts listening 
        /// </summary>
        public void Start()
        {
            m_started = true;

            if (FormatterTypes != null && FormatterTypes.Length > 0)
                // Using only the XmlMessageFormatter. You can use other formatters as well
                m_queue.Formatter = new XmlMessageFormatter(FormatterTypes);

            m_queue.PeekCompleted += new PeekCompletedEventHandler(OnPeekCompleted);
            m_queue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);

            StartListening();
        }

        /// <summary>
        /// Stop listening
        /// </summary>
        public void Stop()
        {
            m_started = false;
            m_queue.PeekCompleted -= new PeekCompletedEventHandler(OnPeekCompleted);
            m_queue.ReceiveCompleted -= new ReceiveCompletedEventHandler(OnReceiveCompleted);
        }

        private void StartListening()
        {
            if (!m_started)
                return;

            // The MSMQ class does not have a BeginRecieve method that can take in a
            // MSMQ transaction object. This is a workaround - we do a BeginPeek and then
            // recieve the message synchronously in a transaction.
            // Check documentation for more details
            if (m_queue.Transactional)
                m_queue.BeginPeek();
            else
                m_queue.BeginReceive();
        }

        private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            m_queue.EndPeek(e.AsyncResult);
            MessageQueueTransaction trans = new MessageQueueTransaction();
            Message msg = null;
            try
            {
                trans.Begin();
                msg = m_queue.Receive(trans);
                trans.Commit();

                StartListening();

                FireRecieveEvent(msg.Body);
            }
            catch
            {
                trans.Abort();
            }
        }

        private void FireRecieveEvent(object body)
        {
            if (MessageReceived != null)
                MessageReceived(this, new MessageEventArgs(body));
        }

        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            Message msg = m_queue.EndReceive(e.AsyncResult);

            StartListening();

            FireRecieveEvent(msg.Body);
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public object MessageBody { get; private set; }

        public MessageEventArgs(object body)
        {
            MessageBody = body;
        }
    }
}
