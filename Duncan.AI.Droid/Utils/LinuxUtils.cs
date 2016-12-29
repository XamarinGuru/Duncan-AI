using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Duncan.AI.Droid.Utils
{
    using System.Runtime.InteropServices;

    static class Syscall
    {

        public const int S_IRWXU = 0x1C0; // 00700
        public const int S_IRUSR = 0x100; // 00400
        public const int S_IWUSR = 0x080; // 00200
        public const int S_IXUSR = 0x040; // 00100

        public const int S_IRWXG = 0x038; // 00070
        public const int S_IRGRP = 0x020; // 00040
        public const int S_IWGRP = 0x010; // 00020
        public const int S_IXGRP = 0x008; // 00010

        public const int S_IRWXO = 0x007; // 00007
        public const int S_IROTH = 0x004; // 00004
        public const int S_IWOTH = 0x002; // 00002
        public const int S_IXOTH = 0x001; // 00001

        [DllImport("c")]
        public static extern int chmod(string path, int mode);
    }
}
