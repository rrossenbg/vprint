/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Reflection;

namespace VPrinting.Colections
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers= true)]
    public class bitArray
    {
        const int NUMBITS = 32;		// Bits = int = 4 byte = 32 bits

        private int m_Bits;

        public bool this[int BitPos]
        {
            get
            {
                BitPosValid(BitPos);
                return ((m_Bits & (1 << (BitPos % 8))) != 0);
            }
            set
            {
                BitPosValid(BitPos);
                if (value)
                {
                    // Set the bit to 1
                    m_Bits |= (1 << (BitPos % 8));
                }
                else
                {
                    // Set the bit to 0
                    m_Bits &= ~(1 << (BitPos % 8));
                }
            }
        }

        [Obfuscation]
        private void BitPosValid(int BitPos)
        {
            if ((BitPos < 0) || (BitPos >= NUMBITS))
                throw new ArgumentOutOfRangeException();
        }

        public void Clear()
        {
            m_Bits = 0x00;
        }
    }
}
