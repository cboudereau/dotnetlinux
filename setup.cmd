@echo off

installdeps.cmd && docker-compose run -d --rm -p 3306:3306 --name localdb db 2>NUL