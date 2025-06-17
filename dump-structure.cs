unsafe struct Emoji
{
    const int MAX_PATH = 256;

    public short Index;
    public short ID;
    public fixed byte FilePath[MAX_PATH];
    public fixed byte Name[MAX_PATH];

    public char U16Char => (char)(ID + 0xE000);
    
    public string FilePathString 
    {
        get
        {
            fixed (Emoji* self = &this)
                return Encoding.UTF8.GetString(self->FilePath, MAX_PATH);
        } 
        set
        {
            fixed (Emoji* self = &this)
            {
                var chars = (ReadOnlySpan<char>)value;
                var bytes = new Span<byte>(self->FilePath, MAX_PATH);

                Encoding.UTF8.GetBytes(chars, bytes);
            }
        }
    }

    public string NameString
    {
        get
        {
            fixed (Emoji* self = &this)
                return Encoding.UTF8.GetString(self->Name, MAX_PATH);
        }
        set
        {
            fixed (Emoji* self = &this)
            {
                var chars = (ReadOnlySpan<char>)value;
                var bytes = new Span<byte>(self->Name, MAX_PATH);

                Encoding.UTF8.GetBytes(chars, bytes);
            }
        }
    }
}
