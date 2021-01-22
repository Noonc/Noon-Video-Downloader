# COMMONBUGS
If a download fails please reference this in your troubleshooting, or search yt-dl [(youtube-dl)](https://github.com/ytdl-org/youtube-dl) and your problem/error online.
Any console message with the prefix "[VDDL]" is a VDDL error. Everything else should be refered to youtube-dl.

## Nothing happens at launch
- When you first open the app, you will not see anything for 15-30 seconds. In the background the necessary files are being downloaded and installed. This will change based on your download speed. Typically this shouldn't take more than a minute though.
- If the app fails to open even after a very long time, make sure you are opening it in admin. There is a tutorial [here](https://raw.githubusercontent.com/Noonc/Noon-Video-Downloader/master/GitHubResources/shortcut.gif) on how to create a shortcut to run as admin. alternatively you can also force the main exe ('Video Downloader.exe') to open as admin under Properties > Compatibility > "Run this program as an administrator". (In the future, I may allow the program to be run without admin. However, when I first coded it I ran into privledge problems and needed the right permissions to access %appdata% where most files are stored)


## Major Issue
- Currently as of 1/22/2021, the ffmpeg download link used in all versions previous to 1.7.0 will crash on install. In order to fix this, you must either download the newer version, or manually download and import ffmpeg.exe to the program files folder. This will not affect you if you previously (as of Sep 18, 2020) installed the program.

## [VDDL] Bugs and Errors
- If you get the error 'Log Directory does not exist', you must create a log first. (This should be fixed as of v1.0.0)

## Youtube-DL Bugs and Errors
- If the downloader is unable to extract the title a few things can be wrong. In my experience my cookie file is invalid. Simply create a new one and reload it.
- If the error is related to authentication, make sure you have permission to view the link you are trying to download. (If the video is blocked by a login wall, use a cookie file)
- (Playlist (not to confused with Batch Download) Download) If you get an error saying 'Cannot save audio and video on same file', this means you have a space in your output folder. I'm not sure why but it seems the output directory cannot have any spaces.
- Output directory cannot have any spaces or special characters.


## Cookies
- Cookies are needed for privated youtube videos, and anything that is blocked by a login or authentication.
- In order to create a proper cookie file, download any cookie plugin/addon for your browser. Then save the cookie file as a ".txt", the name doesn't matter.
- Then depending on what site you are downloading from, make sure you are logged in to the correct account.

