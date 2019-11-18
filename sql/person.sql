    create database data;

    use data;

    -- Mysql 8 User creation syntax
    CREATE USER 'sqluser'@'%' IDENTIFIED BY 'sqluserpwd';
    GRANT SELECT, INSERT ON data.* TO 'sqluser'@'%' WITH GRANT OPTION;

    use data;
    create table person (id int not null auto_increment primary key, name text);

    -- Sample data
    insert into person (name) values ('clem');
