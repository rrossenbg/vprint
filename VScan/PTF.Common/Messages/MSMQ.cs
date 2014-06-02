/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Messaging;
using System.Collections.Generic;

namespace PremierTaxFree.PTFLib.Messages
{
    public class MSMQ
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName">.\\Private$\\myQueue</param>
        /// <param name="label">DateTime.Now.ToString()</param>
        /// <param name="data"></param>
        public static void SendToQueue(string queueName, string label, object data)
        {
            MessageQueue msmq = OpenOrCreateMessageQueue(queueName);
            try
            {
                msmq.Formatter = new BinaryMessageFormatter();
                msmq.Send(data, label);
            }
            catch (MessageQueueException ee)
            {
                throw new ApplicationException("Cannot read data", ee);
            }
            catch (Exception eee)
            {
                throw new ApplicationException("General exception. Cannot read data", eee);
            }
            finally
            {
                msmq.Close();
            }
        }

        /// <summary>
        /// Receives a message from message queue by queue name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static T ReceiveFromQueue<T>(string queueName)
        {
            MessageQueue msmq = OpenOrCreateMessageQueue(queueName);

            try
            {
                msmq.Formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
                var data = (T)msmq.Receive().Body;
                return data;
            }
            catch (MessageQueueException ee)
            {
                throw new ApplicationException("Cannot read data", ee);
            }
            catch (Exception eee)
            {
                throw new ApplicationException("General exception. Cannot read data", eee);
            }
            finally
            {
                msmq.Close();
            }
        }

        /// <summary>
        /// Gets message count in a message queue by queue name
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static int GetMessageCount(string queueName)
        {
            MessageQueue msmq = OpenOrCreateMessageQueue(queueName);
            try
            {
                int count = 0;
                var me = msmq.GetMessageEnumerator2();
                while(me.MoveNext())
                    count++;
                return count;
            }
            finally
            {
                msmq.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="timeout"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        /// <example>
        /// int count = MSMQ.GetMessageCount(Strings.All_SaveQueueName);
        /// foreach (var data in MSMQ.ReceiveAllFromQueue<Hashtable>(Strings.All_SaveQueueName, TimeSpan.FromSeconds(15), count))
        ///     Console.WriteLine(data);
        /// </example>
        public static IEnumerable<T> ReceiveAllFromQueue<T>(string queueName, TimeSpan timeout, int maximum)
        {
            MessageQueue msmq = OpenOrCreateMessageQueue(queueName);
            try
            {
                int index = 0;
                msmq.Formatter = new BinaryMessageFormatter();
                Message msg = null;
                while (index++ < maximum && (msg = msmq.Receive(timeout)) != null)
                    yield return (T)msg.Body;
            }
            finally
            {
                msmq.Close();
            }
        }

        private static MessageQueue OpenOrCreateMessageQueue(string queueName)
        {
            MessageQueue msmq = null;
            if (!MessageQueue.Exists(queueName))
                msmq = MessageQueue.Create(queueName);
            else
                msmq = new MessageQueue(queueName);
            return msmq;
        }
    }
}
