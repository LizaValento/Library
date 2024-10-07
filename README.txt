Установка
Склонируйте репозиторий:

git clone https://github.com/LizaValento/Library/tree/master

Откройте проект:

Откройте решение в Visual Studio.
Восстановите зависимости: Выполните команду в терминале:

dotnet restore

Настройка базы данных
Создайте базу данных:

Откройте SQL Server Management Studio.
Создайте новую базу данных с именем, указанным в строке подключения (appsettings.json).

Примените миграции
Выполните команду в терминале:

dotnet ef database update

Запуск приложения
Выполните команду:

Нажмите F5 или выберите "Start Debugging" из меню.
