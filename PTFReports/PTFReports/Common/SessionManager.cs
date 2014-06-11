/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.SessionState;

namespace PTF.Reports.Common
{
    public class SessionManager
    {
        // Singleton 
        protected SessionManager() { }

        public readonly static SessionManager Instant = new SessionManager();

        protected readonly Hashtable m_sessions = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));

        public IEnumerable<Client> Clients
        {
            get
            {
                lock (m_sessions.SyncRoot)
                {
                    foreach (DictionaryEntry entry in m_sessions)
                        yield return (Client)entry.Value;
                }
            }
        }

        public void Add(HttpSessionState session, string sessionID, string ip, string userAgent)
        {
            if (!m_sessions.ContainsKey(sessionID))
                m_sessions[sessionID] =
                    new Client { Session = new ObjectWrapper<HttpSessionState>(session), SessionID = sessionID, IP = ip, UserAgent = userAgent, StartedAt = DateTime.Now };
        }

        public void Remove(string sessionID)
        {
            m_sessions.Remove(sessionID);
        }

        public void CloseAndBlockIP(string sessionID)
        {
            var client = (Client)m_sessions[sessionID];
            if (client != null)
            {
                m_sessions.Remove(client.SessionID);
                Helper.SaveIPBlockedAsynch(client.IP);
                if (client.Session.IsAlive)
                {
                    client.Session.Object.Clear();
                    client.Session.Object.Abandon();
                }
            }
        }

        public void Clear()
        {
            m_sessions.Clear();
        }
    }

    public class Client
    {
        public ObjectWrapper<HttpSessionState> Session { get; set; }
        public string SessionID { get; set; }
        public string IP { get; set; }
        public string UserAgent { get; set; }
        public DateTime StartedAt { get; set; }
    } 
}