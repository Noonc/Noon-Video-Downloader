# CHANGELOG
All notable changes to this project will be documented in this file.

## [1.8.5 - Unreleased] - Feature

### ADDED
- Added a new config option to set default directory of downloads.

## [1.8.4 - 2022-1-15] - Feature

### Added
- Added Catch exceptions for download allowing for app to open even if correct files are not installed. 
- Added a Veryify Files button to simply check for required files.
- Changed to YT-DLP (as Youtube-DL has been inactive)
- Added a new no-check-certificate option.
- Added a very rough delete option for batch download list

### Changed
- Changed the common-bugs.txt content

## [1.7.0 - 2021-1-22]
Fifth Release

### Changed
- 'Update Program Files' button to truly update by deleting and reinstalling dependencies. Not just checking for the existence of said files.
- Changed the ffmpeg download source, as the previous shutdown. (Caused app to fail to load/update)


## [1.6.4 - 2020-11-18]
- Added Batch Upload/Import Button

## [1.6.3] - 2020-07-30
Third Release

### Added
- Added minimize button
- Added more formats (MP3, OPUS, AAC, MP4, MKV) for both Batch and Default download.

### Changed
- Added/fixed taskbar icon

### Removed
- Nothing

## [1.3.3] - 2020-04-28
Second Release
### Added
- Added Config Editor
- Added Combobox with more formats

### Removed
- Cleaned up Console Logs
- Cleaned up install location - removed dlls, etc

## [1.0.0] - 2020-04-24
First Offical Release
### Added
- Created log delete button
- Added CHANGELOG.md

### Changed
- Changed console prefix from "[YDDL]" to "[VDDL]
- Offically moved out of beta version 5.8.9

### Removed
- Removed README.txt from Appdata

