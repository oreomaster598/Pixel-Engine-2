//Set Platform
#define WINDOWS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PE2.Media
{

    #if WINDOWS
    struct MediaPtr
    {
        internal UInt16 ptr;

        internal MediaPtr(UInt16 ptr) => this.ptr = ptr;
    }
    internal static class MCI
    {
        [DllImport("winmm.dll")]
        static extern int mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        static List<MediaPtr> aliases = new List<MediaPtr>();

        public static MediaPtr Open(string file)
        {
            MediaPtr ptr = new MediaPtr((UInt16)aliases.Count);
            mciSendString($"open {file} type waveaudio alias {ptr.ptr}", null, 0, IntPtr.Zero);
            aliases.Add(ptr);
            return ptr;
        }
        public static void Close(MediaPtr id)
        {
            mciSendString($"close {id.ptr}", null, 0, IntPtr.Zero);
            aliases.Remove(id);
        }
        public static void Play(MediaPtr id)
        {
            mciSendString($"play {id.ptr}", null, 0, IntPtr.Zero);
        }
    }
    #elif IOS
    //Not Implemented
    #elif ANDROID
    //Not Implemented
    #elif MACOS
    //Not Implemented
    #endif
}
