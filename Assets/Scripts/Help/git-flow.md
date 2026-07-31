# Git Flow

## 1. Что такое Git Flow

**Git Flow** — это модель ветвления в Git, которая разделяет работу над:
- стабильным продакшеном,
- подготовкой релизов,
- новыми фичами,
- срочными исправлениями.

Она особенно удобна, когда:
- есть релизы с версиями,
- над проектом работает несколько человек,
- нужно чётко отделять разработку от релизной стабилизации.

---

## 2. Основные ветки

### `main` / `master`
Содержит только стабильный код, который уже готов к продакшену.

### `develop`
Главная ветка разработки.  
В неё попадают завершённые фичи и подготовка к следующему релизу.

### `feature/*`
Ветки для новых задач, фич и локальных изменений.

### `release/*`
Ветки для подготовки релиза: фиксы, версионирование, финальная проверка.

### `hotfix/*`
Ветки для срочных исправлений продакшена.

---

## 3. Базовая схема работы

```text
main
 ├── hotfix/* ──> main + develop
 └── release/* ─> main + develop
develop
 ├── feature/* ─> develop
 └── release/* ─> main + develop
```

### Логика простыми словами
- **Новые задачи** делаются в `feature/*`
- **Подготовка версии** делается в `release/*`
- **Срочный баг в проде** исправляется в `hotfix/*`
- **Стабильный прод** хранится в `main`
- **Вся разработка** сходится в `develop`

---

## 4. Когда Git Flow подходит

Git Flow хорош, если:
- релизы выходят по версиям,
- важна строгая дисциплина веток,
- есть QA/стейджинг/релизный этап,
- релиз должен быть отдельным этапом, а не просто merge в main.

Git Flow не всегда удобен, если:
- вы делаете частые небольшие деплои,
- у вас trunk-based development,
- команда маленькая и релизы очень быстрые,
- вы хотите минимизировать количество веток.

---

## 5. Установка и инициализация

Сначала установите расширение `git-flow`:

**macOS:**
```bash
brew install git-flow-avh
```

**Ubuntu/Debian:**
```bash
sudo apt-get install git-flow
```

**Windows:**
```bash
choco install gitflow-avh
```
Или скачайте с [GitHub](https://github.com/petervanderdoes/gitflow-avh).

### Инициализация репозитория
`git flow init` запускает интерактивный процесс настройки:
- Запрашивает имена веток (main/master, develop, feature/, release/, hotfix/, support/).
- Позволяет выбрать типы веток и их префиксы.
- Создаёт начальную структуру веток.

**Автоматический режим с дефолтами:**
```bash
git flow init -d
```
Использует стандартные имена: `master`, `develop`, `feature/`, `release/`, `hotfix/`.

### Пресеты (--preset)
Ускоряют инициализацию предустановленными конфигурациями:

- **`--preset=classic`** (рекомендуется):
  - `main` или `master` — продакшен
  - `develop` — разработка
  - Подходит для современных проектов с `main`.

- **`--preset=gitflow`**:
  - `master` — продакшен
  - `develop` — разработка
  - Классическая схема для legacy-проектов.

- **Кастомный**: интерактивный `git flow init` без `-d`.

**Дополнительные ветки (опционально):**
- `bugfix/*` — для исправления **некритических** багов разработки (не в продакшене). Создаются от `develop`, мержатся обратно в `develop`. Используется для чёткого разделения багфиксов от новых фич.
- `support/*` — для долгосрочной поддержки старых релизов (LTS).

```bash
# Примеры
git flow init -d                    # дефолты
git flow init --preset=classic      # classic пресет
git flow init --preset=gitflow      # gitflow пресет
```

### Типичные конфигурации веток

| Пресет    | Main     | Develop | Feature  | Release | Hotfix  | Bugfix   |
|-----------|----------|---------|----------|---------|---------|----------|
| classic   | main     | develop | feature/ | release/| hotfix/ | bugfix/  |
| gitflow   | master   | develop | feature/ | release/| hotfix/ | bugfix/  |
| дефолт -d | master   | develop | feature/ | release/| hotfix/ | (опц.)   |

После инициализации проверьте:
```bash
git branch -a
git flow version
```


---

## 6. Команды Git Flow: полный список

> Ниже команды могут немного отличаться в зависимости от реализации `git-flow`, но логика одна и та же.

### Фичи
```bash
git flow feature start <name>
git flow feature finish <name>
git flow feature publish <name>
git flow feature track <name>
git flow feature delete <name>
git flow feature list
```

### Релизы
```bash
git flow release start <version>
git flow release finish <version>
git flow release publish <version>
git flow release track <version>
git flow release delete <version>
git flow release list
```

### Хотфиксы
```bash
git flow hotfix start <version>
git flow hotfix finish <version>
git flow hotfix publish <version>
git flow hotfix track <version>
git flow hotfix delete <version>
git flow hotfix list
```

---

## 7. Команды Git Flow по смыслу

### `start`
Создаёт новую ветку от нужной базы:
- feature — от `develop`
- release — от `develop`
- hotfix — от `main`

### `finish`
Завершает работу:
- сливает ветку в целевые ветки,
- часто создаёт тег версии,
- удаляет ветку, если это предусмотрено.

### `publish`
Отправляет локальную ветку в удалённый репозиторий.

### `track`
Создаёт локальную ветку, отслеживающую удалённую.

### `list`
Показывает список активных веток определённого типа.

---

## 8. Фичи: полный сценарий

### Начать фичу
```bash
git flow feature start login-form
```

Это создаст ветку:
```text
feature/login-form
```

### Работать в ветке
```bash
git status
git add .
git commit -m "Add login form UI"
git commit -m "Connect login form to API"
```

### Опубликовать фичу в remote
```bash
git flow feature publish login-form
```

### Завершить фичу
```bash
git flow feature finish login-form
```

Обычно это:
- merge в `develop`,
- удаление локальной feature-ветки,
- иногда удаление remote-ветки после cleanup.

---

## 9. Релиз: полный сценарий

### Начать релиз
```bash
git flow release start 1.4.0
```

Создаётся ветка:
```text
release/1.4.0
```

### Что обычно делают в release-ветке
- исправляют мелкие баги,
- обновляют номер версии,
- меняют changelog,
- проверяют финальную сборку,
- проводят QA.

### Опубликовать релиз
```bash
git flow release publish 1.4.0
```

### Завершить релиз
```bash
git flow release finish 1.4.0
```

Обычно это:
- merge `release/1.4.0` в `main`,
- создание тега `1.4.0`,
- merge обратно в `develop`,
- удаление release-ветки.

---

## 10. Hotfix: полный сценарий

### Когда нужен hotfix
Если баг уже в проде и его надо исправить срочно, не дожидаясь следующего релиза.

### Начать hotfix
```bash
git flow hotfix start 1.4.1
```

Создаётся ветка:
```text
hotfix/1.4.1
```

### Внести исправление
```bash
git add .
git commit -m "Fix payment crash on empty cart"
```

### Завершить hotfix
```bash
git flow hotfix finish 1.4.1
```

Обычно это:
- merge в `main`,
- создание тега `1.4.1`,
- merge в `develop`,
- удаление hotfix-ветки.

---

## 11. Важные правила ветвления

### Feature
- создаётся от `develop`
- живёт недолго
- содержит одну задачу или логически завершённый кусок работы

### Release
- создаётся от `develop`
- после старта релиза в неё не добавляют крупные фичи
- допускаются только фиксы и стабилизация

### Hotfix
- создаётся от `main`
- решает срочную проблему в проде
- после исправления обязательно попадает и в `develop`

---

## 12. Именование веток

### Хорошие варианты
```text
feature/auth-login
feature/checkout-validation
release/2.3.0
hotfix/2.3.1
```

### Рекомендации
- используйте короткие и понятные имена,
- придерживайтесь одного формата,
- не смешивайте языки и стили,
- избегайте слишком общих названий вроде `feature/test`.

---

## 13. Пример полного цикла

### Шаг 1. Создать feature
```bash
git flow feature start profile-page
```

### Шаг 2. Сделать коммиты
```bash
git add .
git commit -m "Add profile page layout"
git commit -m "Load user data on profile page"
```

### Шаг 3. Завершить feature
```bash
git flow feature finish profile-page
```

### Шаг 4. Подготовить релиз
```bash
git flow release start 1.5.0
```

### Шаг 5. Внести финальные правки
```bash
git add .
git commit -m "Update version and changelog"
```

### Шаг 6. Завершить релиз
```bash
git flow release finish 1.5.0
```

### Шаг 7. Если найден срочный баг в проде
```bash
git flow hotfix start 1.5.1
git add .
git commit -m "Fix crash on profile page"
git flow hotfix finish 1.5.1
```

### Шаг 8. Некритический баг в разработке
```bash
git flow bugfix start validation-error
git add .
git commit -m "Fix form validation error in develop"
git flow bugfix finish validation-error
```

---



## 14. Типичная схема команд

### Создать новую фичу
```bash
git flow feature start <name>
```

### Закоммитить изменения
```bash
git add .
git commit -m "Message"
```

### Закончить фичу
```bash
git flow feature finish <name>
```

### Начать релиз
```bash
git flow release start <version>
```

### Закончить релиз
```bash
git flow release finish <version>
```

### Начать hotfix
```bash
git flow hotfix start <version>
```

### Закончить hotfix
```bash
git flow hotfix finish <version>
```

---

## 15. Что происходит при `finish`

### Feature finish
Обычно:
- merge в `develop`
- удаление feature-ветки

### Release finish
Обычно:
- merge в `main`
- создание тега
- merge обратно в `develop`
- удаление release-ветки

### Hotfix finish
Обычно:
- merge в `main`
- создание тега
- merge обратно в `develop`
- удаление hotfix-ветки

---

## 16. Теги версий

Git Flow часто работает вместе с тегами:
```bash
git tag
git tag v1.4.0
git push --tags
```

Теги помогают:
- фиксировать точную версию релиза,
- быстро находить нужный коммит,
- откатываться к стабильной версии.

---

## 17. Полезные Git-команды рядом с Git Flow

### Посмотреть состояние
```bash
git status
git branch
git branch -a
git log --oneline --graph --decorate --all
```

### Переключиться на ветку
```bash
git checkout develop
git checkout main
```

### Обновить локальные ветки
```bash
git fetch --all --prune
git pull
```

### Слить ветку вручную
```bash
git checkout develop
git merge feature/login-form
```

### Удалить локальную ветку
```bash
git branch -d feature/login-form
```

### Удалить удалённую ветку
```bash
git push origin --delete feature/login-form
```

---

## 18. Важные отличия Git Flow от обычного Git workflow

### Git Flow
- больше веток,
- больше структуры,
- лучше для релизного процесса,
- удобен при командной разработке с QA и версиями.

### Простой feature-branch workflow
- чаще всего есть только `main` и `feature/*`,
- меньше правил,
- проще для маленьких команд,
- быстрее для частых деплоев.

---

## 19. Плюсы Git Flow

- понятная структура,
- легко разделять задачи,
- удобно для релизов и hotfix,
- хорошо подходит для версиированных продуктов,
- снижает риск случайно сломать прод.

---

## 20. Минусы Git Flow

- много веток и merge-операций,
- может быть тяжеловат для маленьких команд,
- иногда замедляет delivery,
- требует дисциплины,
- не всегда удобен для continuous deployment.

---

## 21. Типичные ошибки

### Ошибка 1. Делать feature от `main`
Нужно делать feature от `develop`.

### Ошибка 2. Добавлять большие фичи в release-ветку
В release-ветке должны быть только фиксы и финальные правки.

### Ошибка 3. Забывать merge hotfix обратно в `develop`
Тогда баг может вернуться в следующем релизе.

### Ошибка 4. Работать напрямую в `main`
Это нарушает модель Git Flow.

### Ошибка 5. Слишком долго держать feature-ветки
Чем дольше ветка живёт, тем сложнее merge.

---

## 22. Практические советы

- Делайте feature-ветки небольшими.
- Часто синхронизируйтесь с `develop`.
- Перед `finish` проверяйте, что CI зелёный.
- В release-ветке меняйте только то, что относится к выпуску версии.
- Сразу решайте, кто отвечает за version bump и changelog.
- После hotfix обязательно переносите исправление в `develop`.

---

## 23. Частые сценарии и что делать

### Нужна новая фича
```bash
git flow feature start <name>
```

### Нужно подготовить релиз
```bash
git flow release start <version>
```

### В проде критический баг
```bash
git flow hotfix start <version>
```

### Нужно закончить работу над фичей
```bash
git flow feature finish <name>
```

### Релиз готов
```bash
git flow release finish <version>
```

### Хотфикс готов
```bash
git flow hotfix finish <version>
```

### Ветка не удалилась в origin
```bash
git fetch --prune
```

---

## 24. Мини-шпаргалка по командам

```bash
# Инициализация
git flow init

# Feature
git flow feature start my-feature
git flow feature publish my-feature
git flow feature finish my-feature

# Release
git flow release start 1.0.0
git flow release publish 1.0.0
git flow release finish 1.0.0

# Hotfix
git flow hotfix start 1.0.1
git flow hotfix publish 1.0.1
git flow hotfix finish 1.0.1
```

---

## 25. Мини-таблица: что когда использовать

| Сценарий | Ветка | Откуда создаётся | Что потом |
|---|---|---|---|
| Новая задача | `feature/*` | `develop` | merge в `develop` |
| Подготовка версии | `release/*` | `develop` | merge в `main` и `develop` |
| Срочный баг в проде | `hotfix/*` | `main` | merge в `main` и `develop` |

---

## 26. Как запомнить за 10 секунд

- **feature** — делаю новую задачу
- **release** — готовлю выпуск
- **hotfix** — чиню прод
- **develop** — место для разработки
- **main** — стабильный прод

---

## 27. Короткий пример для заметок

```bash
git flow init

git flow feature start task-name
git commit -am "Work"
git flow feature finish task-name

git flow release start 1.0.0
git flow release finish 1.0.0

git flow hotfix start 1.0.1
git flow hotfix finish 1.0.1
```

---

## 28. Итог

Git Flow — это удобная модель, когда нужно чётко разделять:
- разработку,
- релизную стабилизацию,
- срочные исправления.

Главная идея:
- **фичи** идут в `develop`,
- **релизы** собираются отдельно,
- **hotfix** чинит `main`,
- **main** всегда остаётся стабильной.

---

## 29. Полезный шаблон для команды

### Правила
1. Все задачи начинаются в `feature/*`.
2. Перед релизом создаём `release/*`.
3. Срочные баги делаем через `hotfix/*`.
4. В `main` не коммитим напрямую.
5. Каждое исправление продакшена переносим в `develop`.

### Рекомендуемый формат имен
```text
feature/<short-description>
release/<version>
hotfix/<version>
```

### Пример
```text
feature/user-profile
release/2.1.0
hotfix/2.1.1
```

---

## 30. Быстрая памятка

```text
feature/start  -> новая задача
feature/finish -> merge в develop

release/start   -> подготовка версии
release/finish  -> merge в main + develop + tag

hotfix/start    -> срочный баг
hotfix/finish   -> merge в main + develop + tag
```
