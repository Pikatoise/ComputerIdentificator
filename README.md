# Курсовая работа
## _Идентификатор ПК_

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

Программа, для устройств на операционной системе Windows, предоставляющая сведения об аппаратном и сетевом обеспечении устройства.

## Возможности ПО

- Редактирование имени устройства
- Сведения об ОС
- Аппаратное обеспечение устройства ( видеокарта, процессор, оперативная память и т.д )
- Доступ к сетевым настройкам устройства ( параметры IPV4, прокси)
- Сохранение устройства в БД (MySQL), просмотр имеющихся устройств

## Технологии

- [C#](https://dotnet.microsoft.com/en-us/languages/csharp) 
- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) - Платформа ПО
- [WPF](https://github.com/dotnet/wpf) - Клиентское приложение для визуального взаимодействия
- [MySQL 8.0.31](https://www.mysql.com/) - Основная база данных
- [Entity Framework Core](https://github.com/dotnet/efcore) - ORM для взаимодействия с БД
- [Material Design In XAML](http://materialdesigninxaml.net/) - UI библиотека

## Дизайн

## Установка

Идентификатор ПК использует для запуска [.NET 7.0+](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).

1. Клонируйте репозиторий 
```sh
git clone https://github.com/Pikatoise/ComputerIdentificator.git
```

2. Импортируйте схему БД локально/на сервер из файла computers.sql 

**Free Software**