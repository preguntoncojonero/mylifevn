/*
  GameWatch: a c# online game browser
  Copyright (C) 2002 Rodrigo Reyes, reyes@charabia.net
  
  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.
  
  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.
  
  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Text;

namespace MyLife.Net.IP
{
    public class BitVector
    {
        private byte[] m_data = new byte[0];
        private int m_maxoffset = -1;

        public BitVector()
        {
        }

        public BitVector(byte[] data)
        {
            m_data = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                m_data[i] = data[i];
            }
            m_maxoffset = (m_data.Length*8) - 1;
        }

        public BitVector(long val, int length)
        {
            AddData(val, length);
        }

        public int Length
        {
            get { return m_maxoffset + 1; }
        }

        public void AddAscii(string val)
        {
            for (var i = 0; i < val.Length; i++)
            {
                AddData(val[i], 8);
            }
        }

        public void AddData(long val, int length)
        {
            var offset = m_maxoffset + 1;
            for (var i = 0; i < length; i++)
            {
                Set(offset + i, (val & (1 << (length - i - 1))) != 0);
            }
        }

        public byte[] GetByteArray()
        {
            var result = new byte[m_data.Length];
            for (var i = 0; i < m_data.Length; i++)
                result[i] = m_data[i];
            return result;
        }

        public void Set(int offset, bool val)
        {
            var byteoffset = offset/8;
            var bitoffset = offset%8;

            if (byteoffset >= m_data.Length)
            {
                var data = new byte[byteoffset + 1];
                for (var i = 0; i < m_data.Length; i++)
                {
                    data[i] = m_data[i];
                }
                m_data = data;
                m_maxoffset = offset;
            }
            else if (offset > m_maxoffset)
            {
                m_maxoffset = offset;
            }

            if (val)
                m_data[byteoffset] |= (byte) (1 << (7 - bitoffset));
            else
                m_data[byteoffset] &= (byte) (0xff - (1 << (7 - bitoffset)));
        }

        public bool Get(int offset)
        {
            if (offset > m_maxoffset)
            {
                throw new Exception("OutOfBound offset " + offset);
            }
            var byteoffset = offset/8;
            var bitoffset = offset%8;

            if (byteoffset >= m_data.Length)
            {
                throw new Exception("OutOfBound offset " + offset);
            }

            return (m_data[byteoffset] & (1 << (7 - bitoffset))) != 0;
        }

        public BitVector Range(int start, int length)
        {
            var result = new BitVector();
            for (var i = start; i < (start + length); i++)
            {
                result.Set(i - start, Get(i));
            }
            return result;
        }

        public int LongestCommonPrefix(BitVector other)
        {
            var i = 0;
            while ((i <= other.m_maxoffset) && (i <= m_maxoffset))
            {
                if (other.Get(i) != Get(i))
                {
                    return i;
                }
                i++;
            }
            return i;
        }

        public override String ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i <= m_maxoffset; i++)
            {
                sb.Append(Get(i) ? "1" : "0");
                if (i%8 == 7)
                    sb.Append(" ");
            }
            return sb.ToString();
        }
    }
}