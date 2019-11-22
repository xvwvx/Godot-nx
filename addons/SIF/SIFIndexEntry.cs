namespace SIF
{
    public enum SifSection
    {
        Session,
        Sheets,
        Sprites,
        Names,
        Groups,
        Comments,
        Paths,
        DirNames,

        Count
    }

    public struct SIFIndexEntry
    {
        public SifSection Type;
        public uint Offset;
        public int Length;
        public byte[] Data;
    }
}