@echo off

call installdeps.cmd && docker-compose run -d --rm -p 3306:3306 --name localdb db || goto MYSQL_CHECK

:MYSQL_CHECK
docker logs localdb 2>&1 | find /I "MySQL init process done. Ready for start up."
if errorlevel 1 (
    timeout /T 5
    goto MYSQL_CHECK
)