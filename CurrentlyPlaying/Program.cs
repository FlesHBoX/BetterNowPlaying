using System;
using System.Timers;
using System.Text;

namespace CurrentlyPlaying
{
    class Program
    {
        private static Timer aTimer;    // Declare timer object

        static void Main(string[] args)
        {
            // Instantiate and setup timer, then start it
            aTimer = new Timer(1000);

            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = true;
            
            Console.ReadLine();
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e) // event method to call on timer
        {
            // Read information from streamlabs chatbot text files and init playingOutput for new text file.
            string requestedBy = System.IO.File.ReadAllText(@"C:\Users\Jesse\AppData\Roaming\Streamlabs\Streamlabs Chatbot\Twitch\Files\RequestedBy.txt").Trim();
            string currentSong = System.IO.File.ReadAllText(@"C:\Users\Jesse\AppData\Roaming\Streamlabs\Streamlabs Chatbot\Twitch\Files\CurrentSong.txt").Trim();
            string playingOutput = "";

            // Split artist and song from rest of string
            string[] substrings = currentSong.Split('-');

            // Trim any whitespace from song and artist after splitting them out
            string song = substrings[1].Trim();
            string artist = substrings[0].Trim();

            if (GlobalThings.previousSong == "" && GlobalThings.previousArtist == "")   // This should only ever resolve to tru on app launch
            {
                // Set previous song and artist so that it does not falsly detect a new song playing on startup
                GlobalThings.previousSong = song;
                GlobalThings.previousArtist = artist;

                // Fill in now playing information so that this is not blank in the console on startup
                GlobalThings.consoleNowPlaying = $"[Now Playing?] {song} by {artist}";
                Console.SetCursorPosition(0, 1);
                ClearCurrentConsoleLine();
                Console.WriteLine(GlobalThings.consoleNowPlaying);
            }

            if (song == GlobalThings.previousSong || artist == GlobalThings.previousArtist)   // Just update ticker if the song has not changed
            {
                switch (GlobalThings.counter)    // This is simply so that the same song text stays at the top and has a ticking period to show activity
                {
                    case 1:
                        Console.SetCursorPosition(0, 0);
                        ClearCurrentConsoleLine();
                        Console.Write("\rSong is the same, skipping");
                        GlobalThings.counter++;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        Console.Write(".");
                        GlobalThings.counter++;
                        break;
                    case 5:
                        Console.Write(".");
                        GlobalThings.counter = 1;
                        break;
                    default:
                        GlobalThings.counter = 1;
                        break;
                }
            }
            else    // when song has changed, we need to update the files and console to reflect this
            {
                GlobalThings.counter = 1;   // reset counter back to 1 so that the same song text is appropriate


                if (requestedBy.ToLower() == "fleshbox")    // Don't want the "requested by" tacked on if it's not an actual song request
                {
                    // Update necessary vars for song change
                    GlobalThings.consoleNowPlaying = $"[Now Playing] {song} by {artist}";
                    playingOutput = $"                                                             🎶 Currently Playing: {song} by {artist} - Request A Song Using !sr in chat 🎶";   // Whitespace is to pad text scroll in obs
                }
                else
                {
                    // Update necessary vars for song change and new song request
                    GlobalThings.consoleNowPlaying = $"[Now Playing] {song} by {artist} | Requested by {requestedBy}";
                    string newSongRequestText = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] SONG REQUEST: {artist} - {song} | Requested by {requestedBy}" + Environment.NewLine;
                    playingOutput = $"                                                             🎶 Currently Playing: {song} by {artist}, requested by {requestedBy} - Request A Song Using !sr in chat 🎶";   // Whitespace is to pad the text scroll in obs

                    // Log new song request to console and increment line counter for next song request
                    Console.SetCursorPosition(0, GlobalThings.line);
                    Console.WriteLine(newSongRequestText);
                    GlobalThings.line++;

                    // Log all new song requests for later since streamlabs chatbot is deficient
                    System.IO.File.AppendAllText("RequestedSongs.txt",newSongRequestText,Encoding.UTF8);
                }

                // Write info to output file.
                System.IO.File.WriteAllText("NowPlaying.txt", playingOutput, Encoding.UTF8);

                // Update console with currently playing track in the correct position
                Console.SetCursorPosition(0, 1);
                ClearCurrentConsoleLine();
                Console.WriteLine(GlobalThings.consoleNowPlaying);

                // Set previous song and artist to currently playing so that next event will trigger the "same song" resolve
                GlobalThings.previousSong = song;
                GlobalThings.previousArtist = artist;
            }


        }
        public static class GlobalThings    // Collection of variables that wen need inside the timed event class that we don't want reinitialized on every event.
        {
            public static string previousSong = ""; // previous song and artist for comparison to detect song changes on each event
            public static string previousArtist = "";
            public static int counter = 1;  // Just a counter used for pretty .... in same song console output
            public static int line = 3;     // Song request console output line (starts on line 3 to leave a gap for readability)
            public static string consoleNowPlaying = "";    // I'll be honest, I wasn't sure where else to init this one...
        }
        public static void ClearCurrentConsoleLine()    // snipped code to clear console line
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}