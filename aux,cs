using System.Runtime.InteropServices;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF;
using QuestPDF.Drawing;

Console.OutputEncoding = Encoding.Unicode;
/*
unsafe
{
    var structuresStartAddress = 0x189E40640;
    var structuresCount = 1843;

    var process = new Process(11636);
    var memory = process.Memory;

    var structures = Memory.Alloc<EmojiStruct>(structuresCount);
    memory.Read(structuresStartAddress, structures, sizeof(EmojiStruct) * structuresCount);

    var emojies = stackalloc Emoji[structuresCount];
    for (var index = 0; index < structuresCount; index++)
    {
        var structure = structures[index];
        var id = structure.ID;
        var path = memory.ReadUTF8((nint)structure.FilePath);
        var name = memory.ReadUTF8(memory.Read<nint>(structure.EmojiNamePointer));

        Console.WriteLine($"{$"[{index}]", -6} {id, -5} {(char)(id + 0xE000)} {path} :{name}:");

        var emoji = emojies + index;
        emoji->Index = (short)index;
        emoji->ID = (short)id;
        emoji->FilePathString = path;
        emoji->NameString = name;
    }

    var emojiesSpan = new ReadOnlySpan<byte>(emojies, sizeof(Emoji) * structuresCount);
    File.WriteAllBytes(@"C:\dota_emojies_dump.bin", emojiesSpan);

    _ = 3;
}
*/

unsafe
{
    Settings.License = LicenseType.Community;
    Settings.CheckIfAllTextGlyphsAreAvailable = false;

    using var fontStream = File.OpenRead(@"C:\Windows\Fonts\consola.ttf");
    FontManager.RegisterFont(fontStream);

    var emojiesCount = 1843;
    var emojiesBytes = File.ReadAllBytes(@"C:\dota_emojies_dump.bin");
    fixed (byte* emojiesPointer = emojiesBytes)
    {
        var emojies = (Emoji*)emojiesPointer;
        var emojies_end = emojies + emojiesCount;
        var emoji = emojies;

        var pdfBytes = Document.Create(document =>
        {
            while (emoji < emojies_end)
            {
                document.Page(page =>
                {
                    page.PageColor(QuestPDF.Infrastructure.Color.FromRGB(31, 31, 31));

                    page
                    .Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .PaddingHorizontal(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        x.Spacing(5);

                        var emojiOnPageIndex = 0;
                        do
                        {
                            x
                            .Item()
                            .Text($"{$"[{emoji->Index}]", -6} {emoji->ID, -5} :{emoji->NameString}:")
                            .FontColor(QuestPDF.Infrastructure.Color.FromRGB(210, 210, 210))
                            .FontFamily("Consolas")
                            .FontSize(16);

                            var path = emoji->FilePathString;
                            var outputPath = Path.Combine(@"C:\s2v\output", Path.GetDirectoryName(path)!, Path.GetFileName(path));
                            var image = (Bitmap)Bitmap.FromFile(outputPath);
                            
                            var width = image.Width;
                            var height = image.Height;

                            var count = width / height;
                            if (count > 1)
                            {
                                if (count > 16)
                                {
                                    height = width / 512 * 32;
                                    width = 512;
                                    var newImage = new Bitmap(width, height);
                                    var graphics = Graphics.FromImage(newImage);
                                    for (var segmentIndex = 0; segmentIndex < count; segmentIndex++)
                                    {
                                        int segmentX = segmentIndex % 16;
                                        int segmentY = segmentIndex / 16;
                                        graphics.DrawImage(image, new Rectangle(segmentX * 32, segmentY * 32, 32, 32), new Rectangle(segmentIndex * 32, 0, 32, 32), GraphicsUnit.Pixel);
                                    }

                                    image = newImage;
                                }
                            }

                            using var ms = new MemoryStream();
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            var bytes = ms.ToArray();

                            x
                            .Item()
                            .Width(width)
                            .Height(height)
                            .Image(bytes);

                            x
                            .Item()
                            .Text(string.Empty)
                            .FontSize(6);

                            emoji++;
                        }
                        while (emojiOnPageIndex++ <= 7 && emoji < emojies_end);

                    });
                });
            }
        })
        .GeneratePdf();

        File.WriteAllBytes(@"C:\a.pdf", pdfBytes);
    }
}

[StructLayout(LayoutKind.Explicit, Size = 0x58)]
unsafe struct EmojiStruct
{
    [FieldOffset(0x10)] public int ID;
    [FieldOffset(0x28)] public byte* FilePath;
    [FieldOffset(0x48)] public byte** EmojiNamePointer;
}

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
                return Marshal.PtrToStringUTF8((nint)self->FilePath)!;
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
                return Marshal.PtrToStringUTF8((nint)self->Name)!;
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
