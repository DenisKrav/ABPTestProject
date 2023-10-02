# ABPTestProject
This project is an API push using ASP.Net Core technology using controllers, that's why there is a Controllers folder in the project.
which contains the Experiment.cs controller, which contains the endpoints.

There is a DTO folder in the project that contains the kals to pass to the client: ButtonColorExperimentDTO.cs, PriceExperimentDTO.cs. and auxiliary
the UserPricesCountModel.cs class, which is a helper class that stores the price and how many users have a cb price.

The Models folder stores the models and data context associated with the database.

Migrations are stored in the Migrations folder.

Learn more about the Experiment class.
This file contains the endpoints for the project, which is what the GetButtonColor and GetPrice methods are responsible for. These methods are almost identical except that they return different values. These endpoints contain a CheckToken method that will check if such a user has already occurred, if so, the method exits, otherwise it adds this user to the database and exits as well. The CheckToken method is refactored and therefore this class also has the GetPrice and GetColor methods, which analyze the database and return the price and color, respectively. The GetPrice method contains the GetPercent function, which calculates the percentage of users who have a specific price. Special attention should be paid to the method
GetColor, which has two variations, is all due to insufficient information. The first variation is the main one and returns the color based on it
database analysis, another variation is a method that simply analyzes how many users are in the database with colored buttons and returns the color, but this option
commented and makes sense only when the experiment does not have pauses, interruptions and intentional shutdowns.

In the project folder, there is a file SQLQuery3.sql, which contains a query to create a table that is the only one in the database for this project.
The structure of the project is stored in the photo (Structure bd.png).

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

# ABPTestProject
Цей проект є API затсусунком з використанням технології ASP.Net Core з використанням контролерів, саме тму у проекті є папка Controllers,
у якій міститься контролер Experiment.cs, у якому знаходяться ендпоінти.

У проекті є папка DTO, яка містить калси як для передачі клієнту: ButtonColorExperimentDTO.cs, PriceExperimentDTO.cs. так і допоміжний 
клас UserPricesCountModel.cs, який є допоможним класом, який зберігає ціну, та скільки користувачів мають цб ціну.

У папці Models зберігаються моделі та контекст даних, який пов'язаний з бд.

У папці Migrations зберігаються міграції.

Детальніше про клас Experiment.
У цьому файлі містяться ендпоінти для проекту, а саме за це відповідають методи GetButtonColor та GetPrice ці методи є майже ідентичним, окрім того моменту, що вони повертають різні значення. Ці ендпоінти містять метод CheckToken, який перевірє чи траплявся вже такий користувач, якщо так, то метод закінчує роботу, інакше він додає цього користувача до бд і також закінчує роботу. Метод CheckToken є від рефакторингованим і тому цей клас має ще методи GetPrice та GetColor, які аналізують бд та повертають ціну та колір відповідно. Метод GetPrice містить функцію GetProcent, яка вираховує процент користувачів, які мають конкретну ціну. Особливу увагу треба приділити методу
GetColor, який має дві варіації, все пов'язано з недостатньої кількості інформації. Першиа варіаціє є основною і повертає колр на основі
аналізу бд, інша варіація є методом який просто аналізує скільки користувачів у бд з кольоровими кнопками та повертає колір, але цей варіант 
закоментований та має сенс ише тодя, коли експеремент не має пауз перерв та навмисних вимкнень.

У папці проекту є файл SQLQuery3.sql, у якому є запит на створення таблиці, яка є єдиною у бд для цього проекту.
Структура проекту зберігається у фото (Структура бд.png).
