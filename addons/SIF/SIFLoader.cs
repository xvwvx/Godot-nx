using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;
using File = System.IO.File;

namespace SIF
{
    public struct Dir
    {
        public Vector2 SheetOffset;
        public Vector2 Drawpoint;
        public Vector2 Actionpoint;
        public Vector2 Actionpoint2;
        public Rect2 PfBbox;
    }

    public struct Frame
    {
        public Dir[] Dirs;
    }

    public struct Sprite
    {
        public Vector2 Size;
        public int SpriteSheet;
        public string SpriteName;
        public int Nframes;
        public int Ndirs;
        public Rect2[] Bbox;
        public Rect2 Solidbox;
        public Vector2 SpawnPoint;

        public Vector2[] BlockL;
        public Vector2[] BlockR;
        public Vector2[] BlockU;
        public Vector2[] BlockD;

        public Frame[] Frames;
    }

    public class SIFLoader
    {
        private readonly Dictionary<SifSection, SIFIndexEntry> _entries;

        public SIFLoader(string filename)
        {
            using (var ms = new MemoryStream(File.ReadAllBytes(filename)))
            using (var binaryReader = new BinaryReader(ms))
            {
                var bytes = binaryReader.ReadBytes(4);
                Array.Reverse(bytes);
                var magic = Encoding.Default.GetString(bytes);
                if (magic != "SIF2")
                {
                    throw new Exception("无效文件类型");
                }

                var nsections = binaryReader.ReadByte();
                _entries = new Dictionary<SifSection, SIFIndexEntry>(nsections);
                for (var i = 0; i < nsections; i++)
                {
                    var entry = new SIFIndexEntry
                    {
                        Type = (SifSection) binaryReader.ReadByte(),
                        Offset = binaryReader.ReadUInt32(),
                        Length = (int) binaryReader.ReadUInt32()
                    };
                    _entries[entry.Type] = entry;
                }

                var sheetfiles = ReadStrings(FindSection(binaryReader, SifSection.Sheets));
                var sprites = ReadSprites(FindSection(binaryReader, SifSection.Sprites));
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].SpriteName = sheetfiles[sprites[i].SpriteSheet];
                }

                for (int i = 0; i < sprites.Length; i++)
                {
                    var sprite = sprites[i];
                    Console.WriteLine(i + " " + sprite.Frames[0].Dirs[0].Drawpoint);
                }
            }
        }

        private byte[] FindSection(BinaryReader binaryReader, SifSection type)
        {
            if (_entries.TryGetValue(type, out var entry))
            {
                if (entry.Data == null)
                {
                    binaryReader.BaseStream.Position = entry.Offset;
                    entry.Data = binaryReader.ReadBytes(entry.Length);
                }

                return entry.Data;
            }

            return null;
        }

        private string[] ReadStrings(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                var nstrings = br.ReadUInt16();
                var list = new List<string>(nstrings);
                for (int i = 0; i < nstrings; i++)
                {
                    var str = br.ReadString();
                    list.Add(str);
                }

                return list.ToArray();
            }
        }

        private Sprite[] ReadSprites(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var br = new BinaryReader(ms))
            {
                var nsprites = br.ReadUInt16();
                var sprites = new Sprite[nsprites];

                for (int i = 0; i < nsprites; i++)
                {
                    var sprite = new Sprite();
                    sprite.Size = new Vector2(br.ReadByte(), br.ReadByte());
                    sprite.SpriteSheet = br.ReadByte();

                    sprite.Nframes = br.ReadByte();
                    sprite.Ndirs = br.ReadByte();

                    sprite.Bbox = new Rect2[sprite.Ndirs];
                    for (int j = 0; j < sprite.Bbox.Length; j++)
                    {
                        sprite.Bbox[j] = br.ReadRect2();
                    }

                    sprite.Solidbox = br.ReadRect2();

                    sprite.SpawnPoint = br.ReadVector2();

                    sprite.BlockL = br.ReadBlock();
                    sprite.BlockR = br.ReadBlock();
                    sprite.BlockU = br.ReadBlock();
                    sprite.BlockD = br.ReadBlock();

                    sprite.Frames = new Frame[sprite.Nframes];
                    for (int j = 0; j < sprite.Nframes; j++)
                    {
                        sprite.Frames[j] = br.ReadFrame(sprite.Ndirs);
                    }

                    sprites[i] = sprite;
                }

                return sprites;
            }
        }
    }

    public static class Vector2Extension
    {
        public static Vector2 ReadVector2(this BinaryReader br)
        {
            return new Vector2(br.ReadInt16(), br.ReadInt16());
        }

        public static Rect2 ReadRect2(this BinaryReader br)
        {
            return new Rect2(br.ReadInt16(), br.ReadInt16(),
                br.ReadInt16(), br.ReadInt16());
        }

        public static Vector2[] ReadBlock(this BinaryReader br)
        {
            var count = br.ReadByte();
            if (count == 0)
            {
                return null;
            }

            var points = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                points[i] = br.ReadVector2();
            }

            return points;
        }


        public static Frame ReadFrame(this BinaryReader br, int dirCount)
        {
            var frame = new Frame();
            frame.Dirs = new Dir[dirCount];
            for (int i = 0; i < dirCount; i++)
            {
                var dir = new Dir();
                dir.SheetOffset = br.ReadVector2();

                while (true)
                {
                    var t = br.ReadByte();
                    if (t == 0)
                    {
                        break;
                    }

                    switch (t)
                    {
                        case 1:
                            dir.Drawpoint = br.ReadVector2();
                            break;
                        case 2:
                            dir.Actionpoint = br.ReadVector2();
                            break;
                        case 3:
                            dir.Actionpoint2 = br.ReadVector2();
                            break;
                        case 4:
                            dir.PfBbox = br.ReadRect2();
                            break;
                    }
                }

                frame.Dirs[i] = dir;
            }

            return frame;
        }
    }
}