# SEP Media Services
this is a media service for SEP ecosystem.

## Goal

- [ ] uploading content
    - [ ] store file metadata in Database
    - [x] store file content in physical storage
    
- [ ] downloading content
    - [ ] retrieve file metadata based on fileId
    - [ ] retrieve file from physical storage
    - [ ] stream file to download
    
## Challenges
- Security considerations
    - Execute denial of service attacks -> rate limiter
    - Upload viruses or malware -> move to temp directory and check for malwares
- File size limit

## File upload scenarios
 - Buffering
 - Streaming
as we have small files to upload, we choose buffering approach