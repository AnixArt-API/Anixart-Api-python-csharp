
# Anixart API - Краткий гайд, что и как работает
## Введение
Все запросы необходимо отправлять на домен для API anixart - https://api.anixart.tv/
Само API Anixart'a до ужаса глючное, и для того, чтобы работали некоторые методы необходимо использовать разные библиотеки для rest запросов. Например, метод auth/verify не работает на всеми привычном HttpClient.

Единтсвенная библиотека, на которой работают все методы Anixart api - RestSharp для C#. 
На Python работает обычный requests

Так-же в этот репозиторий я прикрепил программу для автоматической регистрации аккаунтов Anixart по почтам mail.ru (временные не работаю, аникс банит их).<a href="https://mailru.top/"> Купить почты mailr.ru по 0.35р за штуку</a> (Выбираете из раздела MAIL.RU Б/У (WEB+IMAP+POP3), там вам сразу внешний паролей приходит, вообщем всё круто)

Про программу в репозитории будет в конце
## Авторизация & Регистрация

### Регистрация
Метод **auth/signUp**
<br> параметры:<br>
**login, email, password**<br><br>
Метод отправки: **POST**<br><br>
**Пример:**
```
RestClient client = new RestClient("https://api.anixart.tv/");
client.Post(new RestRequest("auth/signUp", Method.Post).AddParameter("login",currentNick).AddParameter("email",currentEmail).AddParameter("password",pass));
```

После регистрации необходимо отправить код с почты, для этого используем метод **auth/verify**

**Пример**

```
RestClient client = new RestClient("https://api.anixart.tv/");

RestRequest request = new RestRequest($"auth/verify", Method.Post);
request.AddParameter("login", currentNick);
request.AddParameter("email", currentEmail);
request.AddParameter("hash", hash);
request.AddParameter("password", pass);
request.AddParameter("code", code);

var res = client.Post(request);
```
Возвращает это всё дело JSON, с нашим заветным токеном

## Лайкинг комментария
Чуть позже залью в этот readme

## Как работает программа по автомтической регистрации
1) Программа из файла emails.txt программа берёт маилы
2) В качестве пароля и логина она генерирует случайную строку
3) Всё это отправляется в методе auth/signUp
4) На указанную почту отправляет код от аниксарта, его мы парсим через протокол соединения Imap (imap.mail.ru)
5) Отправляем код и все нужные данные на auth/verify
6) Получаем токен и сохраняем в файл tokens.txt

## Связь
Telegram: <a href="t.me/andrushku">t.me/andrushku</a>
