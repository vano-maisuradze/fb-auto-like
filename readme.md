# fb-auto-like
Automatically like Facebook profile picture updates


### Instructions

Before you run the program, you need to provide your username, password and 2FA seed in appsettings.json file.

Read [this article](https://odedolive.medium.com/facebook-2fa-login-using-selenium-and-c-net-core-e996d39027d7), if you don't know how to get 2FA seed.

If you don't want to automatically like certain users profile picture updates, you can add users in "ExcludedUsers" list.

```
{
  ...
  "IncludeUsers": [],
  "ExcludedUsers": [
    "FirstName LastName"
  ]
}
```

You can also add users in "IncludeUsers" list, so app will only like provided users profile picture updates.

```
{
  ...
  "IncludeUsers": [
    "FirstName LastName"
    ],
  "ExcludedUsers": [
  ]
}
```

