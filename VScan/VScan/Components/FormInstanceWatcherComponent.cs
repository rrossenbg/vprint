/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using PremierTaxFree.Extensions;

namespace PremierTaxFree
{
    public partial class FormInstanceWatcherComponent : Component
    {
        public event EventHandler FirstLoad;
        public event EventHandler LastClosed;

        private Form m_Owner;
        public Form Owner
        {
            set
            {
                if (value != null)
                {
                    m_Owner = value;
                    m_Owner.Load += new EventHandler(Owner_Load);
                    m_Owner.Closed += new EventHandler(Owner_Closed);
                }
            }
        }

        private void Owner_Load(object sender, EventArgs e)
        {
            Type t = m_Owner.GetType();            
            var q = from f in Application.OpenForms.Convert<Form>()
                    where f.GetType().Equals(t)
                    select f;
            if (q.Count() == 1 && FirstLoad != null)
                FirstLoad(sender, e);
        }

        private void Owner_Closed(object sender, EventArgs e)
        {
            Type t = m_Owner.GetType();
            var q = from f in Application.OpenForms.Convert<Form>()
                    where f.GetType().Equals(t)
                    select f;
            if (q.Count() == 1 && LastClosed != null)
                LastClosed(sender, e);
        }

        public FormInstanceWatcherComponent()
        {
            InitializeComponent();
        }

        public FormInstanceWatcherComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
