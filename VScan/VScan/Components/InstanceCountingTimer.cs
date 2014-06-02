/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Runtime.CompilerServices;

namespace PremierTaxFree.Components
{
    public class InstanceCountingTimer : System.Windows.Forms.Timer
    {
        private int m_instances = 0;

        public new event EventHandler Tick
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                base.Tick += value;
                m_instances++;
                base.Enabled = true;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                base.Tick -= value;
                m_instances--;
                base.Enabled = (m_instances != 0);
            }
        }
    }
}
