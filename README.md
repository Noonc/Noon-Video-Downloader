# Noon-Video-Downloader
A GUI for the Youtube-DL library, using the .NET Framework.

My first real program that uses [Youtube-DL](https://github.com/ytdl-org/youtube-dl) to make an easy to use GUI.

Currently the functions are basic, nothing more than mp4 or mp3 download. 

However, as time goes on, I will add more features.

All bugs and issues should be refered to the common-bugs.txt either here in this repository or in the application.

## Installation
- Navigate to the releases page or use this link [here](https://github.com/Noonc/Noon-Video-Downloader/releases)
- Download the latest zip.
- Open with Winrar, 7Zip or extract onto computer.
- Then run "setup.exe" (The file "Noon's Video Downloader.msi" must be in the same directory as the exe to install)

## Usage
```
All you have to do is input the url in the top textboxt then select an ouput. 

There are other options like cookies for privated links, and format options like MP3 (Only current format). 

If you have multiple links to download (like a series) or an album, you can use the batch list. 
If you have a youtube playlist, you can just use that url in the normal textbox. 
Make sure you use the correct download button, there are two. One for the "normal" download/url box and a second for the batch list.

When you enter a link to the batch list your link will be added to the list and you will see the red box fill up. 
You also have the option to save that batch file.
On shutdown, the batch list is cleared and will not save. 
```
## VPN
If you are using a vpn, upon launch you will be 'notified' of your current IP. 
This has only been tested with NordVPN, however. 
If your vpn uses a local IP starting with 10, it will 'detect' a vpn. 

## Bugs and Errors
Any bugs or errors can first be refered to the common-bugs.txt, a shortcut to that exists at the bottom of the window. 

Any 'Error:' messages without the prefix '[YDDL]' should be researched according to youtube-dl. 

## Config
Currently there are two config options: Autolog and CheckIP.
Autolog creates a log everytime you close the application.
CheckIP prevents you from downloading unless you are using a VPN. Please note this is not 100% accurate, and depends on many factors including your current IP.
By default, Autolog is on and CheckIP is not.

## Saves and Logs
To access logs and batch saves, there is a button on the bottom of the application or you can navigate to C:\Users\User\Appdata\Roaming\vddl
To access the config file, navigate to your install directory. 
Find the file 'Video Downloader.dll.config' and edit it with notepad or any other text editor. 
Note, you must have admin permissions to edit the file.
Be careful not to mess up the layout of the document, not removing any characters. 
If the value is set to "true" no caps, then that feature will be enabled, anything else will act as "false" (Eg. "a", "false" "no")
Currently that is the only way to change the config. (Subject to change next update)


