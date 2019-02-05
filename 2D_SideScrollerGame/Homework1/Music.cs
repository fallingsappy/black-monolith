using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework1
{
    class Music
    {
        public static List<string> Playlist { get; set; }
        
        static Music()
        {
            Playlist = new List<string>();
            Playlist.Add("1.wav");
            Playlist.Add("5AMU5 - KLAS.wav");
        }

        public string PlaySongNumber(int n)
        {
            return Playlist[n]; 
        }
    }
}
