# SEP Media Services
this is a media service for SEP ecosystem.

## Goal

- [ ] uploading content
    - [x] store file metadata in Database
    - [x] store file content in physical storage
    
- [x] downloading content
    - [x] retrieve file metadata based on fileId
    - [x] retrieve file from physical storage
    - [x] stream file to download
    
## Challenges
- Security considerations
    - Execute denial of service attacks -> rate limiter
    - Upload viruses or malware -> move to temp directory and check for malwares
- File size limit

## File upload scenarios
 - Buffering
 - Streaming
as we have small files to upload, we choose buffering approach