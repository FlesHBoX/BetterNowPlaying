# BetterNowPlaying
This is just a way to take streamlabs chatbot (formerly Ankhbot) now playing text files and make the output more useful to me.

This takes the output text files from streamlabs chatbot, reads them, then outputs two new text files in the programs running directory
* NowPlaying.txt - Contains the currently playing song and if the song was requested by a viewer, an additional "requested by" section.
* RequestedSongs.txt - Logs all song requests that get played (does not know about songs that do not get played)

As it stands, this is highly customized to my environment, and will not simply work for anyone else without editing.  I will possibly update this with proper options at some point in the future.
