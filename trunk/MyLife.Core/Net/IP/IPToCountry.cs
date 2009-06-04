/*
 * GameWatch - Server Browser for online games
 * Copyright (C) 2003 Rodrigo Reyes <reyes@charabia.net>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
 * 02111-1307, USA.
 *
 */
using System;
using System.IO;

namespace MyLife.Net.IP
{
    public class IPToCountry
    {
        public static int NetworkCodeCount;
        private readonly BitVectorTrie m_trie = new BitVectorTrie();

        public void Load(string filename)
        {
            var nccin = new StreamReader(filename);
            Load(nccin);
        }

        public void Load(StreamReader nccin)
        {
            try
            {
                string line;
                var seps = new[] {'|'};
                while ((line = nccin.ReadLine()) != null)
                {
                    var data = line.Split(seps);

                    // Make the following assumption:
                    // if 2nd entry is 2 chars, it's a country code.
                    // if there is not dot in 4th entry, it's an IP (not an ASN)
                    if ((data.Length > 2) && (data[1].Length == 2) && (data[3].IndexOf('.') >= 0))
                    {
                        AddIp(data[3], data[1]);
                        NetworkCodeCount++;
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("exc: {0}", exc);
            }
            finally
            {
                nccin.Close();
            }
        }

        public string GetCountry(string ip)
        {
            var key = IpToBitVector(ip);
            return (string) m_trie.GetBest(key);
        }

        private void AddIp(string ip, string country)
        {
            var key = IpToBitVector(ip);
            m_trie.Add(key, String.Intern(country.ToUpper()));
        }

        private static BitVector IpToBitVector(string ip)
        {
            var elements = ip.Split('.');
            var bv = new BitVector();
            foreach (var e in elements)
            {
                var i = Int32.Parse(e);
                bv.AddData(i, 8);
            }
            return bv;
        }
    }
}