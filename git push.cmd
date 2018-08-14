@echo off
set /p newversion=Enter new version: 
cd /d D:\GitHub\Centrofinans
git status
git add -A
git commit -m "version %newversion%"
git push
pause