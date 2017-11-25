using System.IO;

namespace Stardust
{
    public class ByteInArg
    {
        private readonly MemoryStream m_stream = new MemoryStream();
        private readonly BinaryWriter m_writer;

        public ByteInArg()
        {
            m_writer = new BinaryWriter(m_stream);
        }

        public void Write(bool value)
        {
            m_writer.Write(value);
        }

        public void Write(byte value)
        {
            m_writer.Write(value);
        }

        public void Write(char ch)
        {
            m_writer.Write(ch);
        }

        public void Write(int value)
        {
            m_writer.Write(value);
        }

        public void Write(string value)
        {
            m_writer.Write(value);
        }

        public byte[] GetBuffer()
        {
            return m_stream.GetBuffer();
        }
    }

    public class ByteOutArg
    {
        private readonly BinaryReader m_reader;

        public ByteOutArg(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            m_reader = new BinaryReader(stream);
        }

        public bool ReadBoolean()
        {
            return m_reader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return m_reader.ReadByte();
        }

        public int ReadInt32()
        {
            return m_reader.ReadInt32();
        }

        public string ReadString()
        {
            return m_reader.ReadString();
        }
    }
}