using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCF.DemoRules.Test
{
    public class MySpecialBitArray
    {
        public MySpecialBitArray(int size) : this((uint)size) { }
        public MySpecialBitArray(uint size)
        {
            m_size = size;
            m_data = new uint[size / 32 + 1];
        }

        public void Set(uint index)
        {
            if (index >= m_size)
            {
                throw new ArgumentOutOfRangeException(@"ArgumentOutOfRange_Index");
            }
            m_data[index / 0x20] |= (uint)((1) << ((int)(index % 0x20)));
        }

        public uint AndCounting(MySpecialBitArray other)
        {
            if (other.m_size != m_size) throw new ArgumentException("Size is not the same");
            uint res = 0;
            for (int i = 0; i < m_data.Length; i++)
            {
                uint and = m_data[i] & other.m_data[i];
                while (and > 0)
                {
                    and ^= and & (~and + 1);
                    res++;
                }
            }
            return res;
        }

        public uint NextSetIndex()
        {
            return NextSetIndex(m_size);
        }

        public uint NextSetIndex(uint index)
        {
            int cur_int = 0;
            int cur_bit = 0;
            if (index < m_size) // from the beginning
            {
                cur_int = (int)((index+1) / 32);
                cur_bit = (int)((index+1) % 32);
            }
            uint t = m_data[cur_int];
            if (t>0) t &= (uint)((~0) << cur_bit); // leave only bits after specified
            while (t == 0 && ++cur_int < m_data.Length) // move to the next int if necessary
            {
                cur_bit = 0;
                t = m_data[cur_int];
            }
            if (cur_int < m_data.Length)
            {
                uint next_bit_val = (uint) ((t & (~t + 1)) >> cur_bit); // next bit is on the left of cur bit
                while (next_bit_val > 1)
                {
                    cur_bit++;
                    next_bit_val >>= 1;
                }
                return (uint)(cur_bit + cur_int * 32);
            }
            return m_size;
        }

        public uint Size { get { return m_size; } }

        private uint[] m_data;
        private uint m_size;
    }
}
