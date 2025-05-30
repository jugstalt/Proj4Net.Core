// CharBuffer.cs
// 
// Copyright (C) 2003-2004 Ryan Seghers
//
// This software is provided AS IS. No warranty is granted, 
// neither expressed nor implied. USE THIS SOFTWARE AT YOUR OWN RISK.
// NO REPRESENTATION OF MERCHANTABILITY or FITNESS FOR ANY 
// PURPOSE is given.
//
// License to use this software is limited by the following terms:
// 1) This code may be used in any program, including programs developed
//    for commercial purposes, provided that this notice is included verbatim.
//    
// Also, in return for using this code, please attempt to make your fixes and
// updates available in some way, such as by sending your updates to the
// author.

using System;
using System.Text;
#if SILVERLIGHT
using HighResClock = System.DateTime;
#endif

namespace RTools.Util
{
    /// <summary>
    /// Buffer for characters.  This approximates StringBuilder
    /// but is designed to be faster for specific operations.
    /// This is about 30% faster for the operations I'm interested in
    /// (Append, Clear, Length, ToString).
    /// This trades off memory for speed.
    /// </summary>
    /// <remarks>
    /// <para>To make Remove from the head fast, this is implemented
    /// as a ring buffer.</para>
    /// <para>This uses head and tail indices into a fixed-size 
    /// array. This will grow the array as necessary.</para>
    /// </remarks>
    public class CharBuffer
    {
        #region Fields

        int capacity = 128;
        char[] buffer;
        int headIndex;  // index of first char
        int tailIndex;  // index 1 past last char

        #endregion

        #region Properties

        /// <summary>
        /// Gets/Sets the number of characters in the character buffer.
        /// Increasing the length this way provides indeterminate results.
        /// </summary>
        public int Length
        {
            get { return (tailIndex - headIndex); }
            set
            {
                tailIndex = headIndex + value;
                if (tailIndex >= capacity) throw new
                    IndexOutOfRangeException("Tail index greater than capacity");
            }
        }

        /// <summary>
        /// Returns the capacity of this character buffer.
        /// </summary>
        public int Capacity
        {
            get { return (capacity); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CharBuffer()
        {
            buffer = new char[capacity];
        }

        /// <summary>
        /// Construct with a specific capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public CharBuffer(int capacity)
        {
            this.capacity = capacity;
            buffer = new char[capacity];
        }

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Reallocate the buffer to be larger. For the new size, this
        /// uses the max of the requested length and double the current
        /// capacity.
        /// This does not shift, meaning it does not change the head or
        /// tail indices.
        /// </summary>
        /// <param name="requestedLen">The new requested length.</param>
        protected void Grow(int requestedLen)
        {
            int newLen = Math.Max(capacity * 2, requestedLen);
            newLen = Math.Max(newLen, 16);
            char[] newBuffer = new char[newLen];
            Array.Copy(buffer, 0, newBuffer, 0, capacity);
            buffer = newBuffer;
            capacity = newLen;
        }

        /// <summary>
        /// Ensure that we're set for the requested length by 
        /// potentially growing or shifting contents.
        /// </summary>
        /// <param name="requestedLength"></param>
        protected void CheckCapacity(int requestedLength)
        {
            if (requestedLength + headIndex >= capacity)
            {
                // have to do something
                if ((requestedLength + headIndex > (capacity >> 1))
                    && (requestedLength < capacity - 1))
                {
                    // we're more than half-way through the buffer, and shifting is enough
                    // so just shift
                    ShiftToZero();
                }
                else
                {
                    // not far into buffer or shift wouldn't be enough anyway
                    Grow(0);
                }
            }
        }

        /// <summary>
        /// Move the buffer contents such that headIndex becomes 0.
        /// </summary>
        protected void ShiftToZero()
        {
            int len = Length;
            for (int i = 0; i < len; i++)
            {
                buffer[i] = buffer[i + headIndex];
            }
            headIndex = 0;
            tailIndex = len;
        }

        #endregion

        #region Public Methods and Indexer

        /// <summary>
        /// Overwrite this object's underlying buffer with the specified
        /// buffer.
        /// </summary>
        /// <param name="b">The character array.</param>
        /// <param name="len">The number of characters to consider filled
        /// in the input buffer.</param>
        public void SetBuffer(char[] b, int len)
        {
            capacity = b.Length;
            buffer = b;
            headIndex = 0;
            tailIndex = len;
        }

        /// <summary>
        /// Append a character to this buffer.
        /// </summary>
        /// <param name="c"></param>
        public void Append(char c)
        {
            if (tailIndex >= capacity) CheckCapacity(Length + 1);
            buffer[tailIndex++] = c;
        }

        /// <summary>
        /// Append a string to this buffer.
        /// </summary>
        /// <param name="s">The string to append.</param>
        public void Append(string s)
        {
            if (s.Length + tailIndex >= capacity) CheckCapacity(Length + s.Length);
            for (int i = 0; i < s.Length; i++)
                buffer[tailIndex++] = s[i];
        }

        /// <summary>
        /// Append a string to this buffer.
        /// </summary>
        /// <param name="s">The string to append.</param>
        public void Append(CharBuffer s)
        {
            if (s.Length + tailIndex >= capacity) CheckCapacity(Length + s.Length);
            for (int i = 0; i < s.Length; i++)
                buffer[tailIndex++] = s[i];
        }

        /// <summary>
        /// Remove a character at the specified index.
        /// </summary>
        /// <param name="i">The index of the character to remove.</param>
        /// <returns></returns>
        public void Remove(int i)
        {
            Remove(i, 1);
        }

        /// <summary>
        /// Remove a specified number of characters at the specified index.
        /// </summary>
        /// <param name="i">The index of the characters to remove.</param>
        /// <param name="n">The number of characters to remove.</param>
        public void Remove(int i, int n)
        {
            n = Math.Min(n, Length);
            if (i == 0)
            {
                headIndex += n;
            }
            else
            {
                Array.Copy(buffer, i + headIndex + n, buffer, i + headIndex,
                    tailIndex - (i + headIndex + n));
            }
        }

        /// <summary>
        /// Find the first instance of a character in the buffer, and
        /// return its index.  This returns -1 if the character is
        /// not found.
        /// </summary>
        /// <param name="c">The character to find.</param>
        /// <returns>The index of the specified character, or -1
        /// for not found.</returns>
        public int IndexOf(char c)
        {
            for (int i = headIndex; i < tailIndex; i++)
            {
                if (buffer[i] == c) return (i - headIndex);
            }
            return (-1);
        }

        /// <summary>
        /// Empty the buffer.
        /// </summary>
        public void Clear()
        {
            headIndex = 0;
            tailIndex = 0;
        }

        /// <summary>
        /// Indexer.
        /// </summary>
        public char this[int index]
        {
            get { return (buffer[index + headIndex]); }
            set { buffer[index + headIndex] = value; }
        }

        /// <summary>
        /// Return the current contents as a string.
        /// </summary>
        /// <returns>The new string.</returns>
        public override String ToString()
        {
            return (new String(buffer, headIndex, tailIndex - headIndex));
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// Simple self test.
        /// </summary>
        /// <returns>bool - true for test passed, false otherwise</returns>
        public static bool TestSelf()
        {
            Logger log = new Logger("CharBuffer: TestSelf");
            log.Info("Starting...");

            // Append
            CharBuffer cb = new CharBuffer();
            cb.Append('a'); cb.Append('b'); cb.Append('c');
            log.Info("cb after Append: '{0}'", cb);
            if (cb[0] != 'a') { log.Error("Append or indexer failed."); return (false); }
            if (cb[1] != 'b') { log.Error("Append or indexer failed."); return (false); }
            if (cb[2] != 'c') { log.Error("Append or indexer failed."); return (false); }

            // Append string
            cb.Append("_hello");
            log.Info("cb after Append string: '{0}'", cb);
            if (cb[4] != 'h') { log.Error("Append or indexer failed."); return (false); }

            // Clear
            cb.Clear();
            if (cb.Length != 0) { log.Error("Clear failed."); return (false); }
            log.Info("cb after Clear: '{0}'", cb);

            // Grow
            cb = new CharBuffer(0);
            for (int i = 0; i < 33; i++) cb.Append('a');
            log.Info("cb after Growth: '{0}'", cb);
            if (cb[32] != 'a') { log.Error("Append or indexer failed."); return (false); }

            // IndexOf
            cb.Clear();
            cb.Append("This is a sentence");
            if (cb.IndexOf('a') != 8) { log.Error("IndexOf failed."); return (false); }

            // remove
            cb.Remove(0);
            log.Info("cb after Remove: '{0}'", cb);
            if (cb.IndexOf('a') != 7) { log.Error("IndexOf failed."); return (false); }

            cb.Remove(1);
            log.Info("cb after Remove: '{0}'", cb);
            if (cb.IndexOf('i') != 3) { log.Error("IndexOf failed."); return (false); }

            cb.Remove(2, 4);
            log.Info("cb after Remove: '{0}'", cb);
            if (cb[4] != 's') { log.Error("IndexOf failed."); return (false); }

            // use as a ring
            log.Info("Test ring buffer:");
            cb = new CharBuffer(16);
            for (int i = 0; i < 32; i++)
            {
                cb.Append("hello");
                if (!cb.ToString().Equals("hello")) { log.Error("Not hello after append."); return (false); }
                cb.Remove(0, 5);
                if (cb.Length != 0) { log.Error("Len wrong after remove."); return (false); }
            }

            log.Info("Done.");
            return (true);
        }

        /// <summary>
        /// Compare speed to StringBuilder.
        /// </summary>
        public static void SpeedTest()
        {
            Logger log = new Logger("CharBuffer: SpeedTest");
            log.Info("Starting...");

            char c = 'a';
            string s = null;
            int cycles = 1000000;

            // a sequence of common ops
            StringBuilder sb = new StringBuilder();
            DateTime startTime = HighResClock.Now;
            for (int i = 0; i < cycles; i++)
            {
                sb.Append(c);
                sb.Append(c);
                sb.Append(c);
                sb.Append(c);
                sb.Append("hello");
                s = sb.ToString();
                sb.Length = 0;
            }
            log.Info("StringBuilder paces took: {0} us",
                (HighResClock.Now - startTime).TotalMilliseconds * 1000.0 / cycles);

            // a sequence of common ops
            CharBuffer cb = new CharBuffer(16); // match StringBuilder's default length
            startTime = HighResClock.Now;
            for (int i = 0; i < cycles; i++)
            {
                cb.Append(c);
                cb.Append(c);
                cb.Append(c);
                cb.Append(c);
                cb.Append("hello");
                s = cb.ToString();
                cb.Length = 0;
            }
            log.Info("CharBuffer paces took: {0} us",
                (HighResClock.Now - startTime).TotalMilliseconds * 1000.0 / cycles);

            log.Info("Done.");
        }

        #endregion
    }
}
