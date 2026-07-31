# Git-шпаргалка

## 1) Что Git вообще делает

Git хранит историю изменений в виде **коммитов**.
У тебя есть:

* **working tree** — файлы в папке проекта;
* **staging area** — то, что ты подготовил к коммиту;
* **repository** — история коммитов;
* **branch** — указатель на цепочку коммитов.

Схема простая:

```text
изменил файл → git add → git commit → git push
```

---

# 2) Базовая инициализация и настройка

## Создать репозиторий

```bash
git init
```

## Склонировать существующий

```bash
git clone <url>
```

## Посмотреть базовые настройки

```bash
git config --global user.name "Имя"
git config --global user.email "mail@example.com"
```

## Проверить настройки

```bash
git config --list
```

---

# 3) Самые нужные команды каждый день

## Проверить состояние

```bash
git status
```

Это первая команда почти перед каждым действием.

## Посмотреть историю

```bash
git log
git log --oneline
git log --oneline --graph --decorate --all
```

Последняя — одна из самых полезных.

## Посмотреть изменения

```bash
git diff
git diff --staged
```

* `git diff` — различия между рабочей папкой и staging area.
* `git diff --staged` — различия между staging area и последним коммитом.

## Добавить файлы в staging

```bash
git add file.txt
git add .
git add -A
```

Разница:

* `git add file.txt` — только один файл;
* `git add .` — всё новое и изменённое в текущей папке и ниже;
* `git add -A` — вообще всё, включая удаления.

## Сделать коммит

```bash
git commit -m "Сообщение коммита"
```

Хорошее сообщение — короткое и по делу.

Примеры:

```bash
git commit -m "Fix login validation"
git commit -m "Add user profile endpoint"
```

---

# 4) Ветки: создание, переключение, удаление

## Посмотреть ветки

```bash
git branch
git branch -a
```

`-a` показывает и локальные, и удалённые.

## Создать ветку

```bash
git branch feature/login
```

## Переключиться на ветку

```bash
git checkout feature/login
```

Старый вариант:

```bash
git checkout feature/login
```

Но сейчас удобнее `switch`.

## Создать и сразу перейти

```bash
git switch -c feature/login
```

```bash
git checkout -b feature/login
```

## Удалить ветку

```bash
git branch -d feature/login
```

Если ветка не слита и Git ругается:

```bash
git branch -D feature/login
```

`-D` — принудительно.

---

# 5) Слияние веток

## Слить ветку в текущую

```bash
git merge feature/login
```

Обычно работа идёт так:

```bash
git checkout main
git merge feature/login
```

## Что бывает при merge

### Fast-forward

Если история линейная, Git просто двигает указатель ветки.

### Merge commit

Если у веток уже есть разветвление, Git создаст отдельный merge-коммит.

## Когда merge удобен

* хочешь сохранить фактическую историю работы;
* не хочешь переписывать историю;
* работаешь в команде.

---

# 6) Rebase: когда и зачем

## Что делает rebase

```bash
git rebase main
```

Это как будто ты «переносишь» свои коммиты поверх другой базы.

## Когда использовать

* для локальных веток;
* чтобы история была линейной;
* перед вливанием feature-ветки в main, если команда так работает.

## Важное правило

**Не делай rebase веток, которые уже запушены и используются другими, если не понимаешь последствия.**
Это переписывает историю.

---

# 7) Remote: работа с удалённым репозиторием

## Посмотреть удалённые

```bash
git remote -v
```

## Добавить удалённый репозиторий

```bash
git remote add origin <url>
```

## Скачать изменения с сервера

```bash
git fetch
```

`fetch` просто обновляет ссылки на удалённые ветки. Он не сливает изменения в твою ветку.

## Скачать и сразу слить

```bash
git pull
```

Это примерно:

```bash
git fetch
git merge
```

Иногда `pull` делает rebase, если так настроено.

## Отправить изменения

```bash
git push
```

Если ветка новая:

```bash
git push -u origin feature/login
```

`-u` связывает локальную ветку с удалённой, дальше можно просто `git push`.

---

# 8) Типовые рабочие сценарии

## Сценарий 1: обычная работа над задачей

```bash
git checkout main
git pull
git checkout -c feature/auth
# работаешь
git add .
git commit -m "Add auth flow"
git push -u origin feature/auth
```

Потом на сервере или через PR ветку вливаешь в `main`.

---

## Сценарий 2: быстро подтянуть изменения из main в свою ветку

### Через merge

```bash
git checkout feature/auth
git merge main
```

### Через rebase

```bash
git checkout feature/auth
git rebase main
```

Если ветка только твоя, rebase часто удобнее.

---

## Сценарий 3: конфликт при merge

Git скажет, что есть конфликт.

Что делать:

1. Открыть конфликтные файлы.
2. Найти блоки вида:

```text
<<<<<<< HEAD
ваш код
=======
чужой код
>>>>>>> branch-name
```

3. Вручную оставить нужный вариант.
4. Затем:

```bash
git add <файл>
git commit
```

Если был rebase:

```bash
git rebase --continue
```

Если хочется отменить:

```bash
git merge --abort
```

или

```bash
git rebase --abort
```

---

# 9) Stash: отложить изменения

Если нужно срочно переключиться на другую задачу, а текущие изменения не готовы:

```bash
git stash
```

Посмотреть список:

```bash
git stash list
```

Вернуть:

```bash
git stash apply
```

Вернуть и удалить из stash:

```bash
git stash pop
```

Удалить stash:

```bash
git stash drop
```

Сохранить вместе с неотслеживаемыми файлами:

```bash
git stash -u
```

---

# 10) Отмена изменений: что и когда использовать

Вот здесь чаще всего путаются.

## 1. Отменить изменения в файле до staging

```bash
git restore file.txt
```

Это вернёт файл к состоянию последнего коммита.

---

## 2. Убрать файл из staging, но сохранить изменения в файле

```bash
git restore --staged file.txt
```

---

## 3. Отменить последний коммит, но оставить изменения в файлах

```bash
git reset --soft HEAD~1
```

* коммит исчезнет из истории;
* изменения останутся в staging.

---

## 4. Отменить последний коммит и убрать изменения из staging, но оставить в файлах

```bash
git reset --mixed HEAD~1
```

Это поведение по умолчанию.

---

## 5. Полностью откатить всё к прошлому состоянию

```bash
git reset --hard HEAD~1
```

Это опасно: изменения в рабочей папке и staging будут потеряны.

---

## 6. Отменить уже опубликованный коммит безопасно

```bash
git revert <commit_hash>
```

`revert` не удаляет историю, а создаёт новый коммит с обратным изменением.

Это лучший способ для публичных веток.

---

# 11) Reset vs Revert

Коротко и по делу:

* **reset** — переписывает локальную историю;
* **revert** — добавляет новый коммит, который отменяет старый.

Правило:

* локально и осторожно — `reset`;
* в общей ветке и безопасно — `revert`.

---

# 12) Detached HEAD: что это такое

Это состояние, когда ты смотришь не на ветку, а прямо на конкретный коммит.

Пример:

```bash
git checkout <commit_hash>
```

или

```bash
git checkout --detach <commit_hash>
```

Тогда ты можешь смотреть старый коммит, но если создашь новый коммит прямо отсюда, он может «потеряться», если не создать ветку.

Исправление:

```bash
git checkout -b my-fix
```

---

# 13) Как переименовать коммит

## Последний коммит

```bash
git commit --amend -m "Новое сообщение"
```

Если надо ещё и поменять файлы, сначала исправляешь их, потом снова `--amend`.

Если вы уже скинули неправильный коммит на сервер, то измнения применятся только на локальный репозиторий, удаленный придется обновлять через `git push --force`

## Не последний коммит

Тогда обычно используют интерактивный rebase:

```bash
git rebase -i HEAD~3
```

Потом меняют `pick` на `reword` у нужного коммита.

---

# 14) Интерактивный rebase: зачем нужен

```bash
git rebase -i HEAD~5
```

Можно:

* переименовать коммиты;
* объединить несколько коммитов;
* удалить лишние;
* переставить порядок.

Частый кейс — почистить историю перед PR.

---

# 15) Слияние нескольких коммитов в один

Через интерактивный rebase:

```bash
git rebase -i HEAD~3
```

Один коммит оставляешь `pick`, остальные меняешь на `squash` или `fixup`.

* `squash` — объединяет и позволяет отредактировать сообщение;
* `fixup` — объединяет молча.

---

# 16) Теги

Теги используют для версий: `v1.0.0`, `v2.1.3`.

## Создать тег

```bash
git tag v1.0.0
```

## Посмотреть теги

```bash
git tag
```

## Пушить тег

```bash
git push origin v1.0.0
```

или все теги сразу:

```bash
git push --tags
```

---

# 17) Поиск по истории

## Кто менял строку

```bash
git blame file.txt
```

## Найти коммит по тексту в сообщении

```bash
git log --oneline --grep="auth"
```

## Посмотреть, где строка была добавлена/удалена

```bash
git log -p file.txt
```

---

# 18) Проверка, что именно изменилось

## Кратко

```bash
git diff --stat
```

## Полностью

```bash
git diff
```

## Что уже подготовлено к коммиту

```bash
git diff --staged
```

## Что поменялось в конкретном коммите

```bash
git show <commit_hash>
```

---

# 19) Удобные команды для диагностики

## Показать граф веток

```bash
git log --oneline --graph --decorate --all
```

## Показать текущую ветку

```bash
git branch --show-current
```

## Проверить, на какой ветке ты сейчас

```bash
git status
```

## Показать удалённые ссылки и ветки

```bash
git remote show origin
```

---

# 20) Как вернуть потерянный коммит

Если кажется, что что-то пропало:

```bash
git reflog
```

`reflog` показывает, куда двигался `HEAD`. Это часто спасает после `reset --hard` и других неприятностей.

Потом можно вернуться к нужному состоянию:

```bash
git reset --hard <hash>
```

---

# 21) .gitignore

Файл `.gitignore` говорит Git, что не нужно отслеживать.

Пример:

```gitignore
bin/
obj/
*.log
.env
```

Важно: если файл уже был добавлен в Git, просто добавить его в `.gitignore` недостаточно. Сначала надо убрать его из индекса:

```bash
git rm --cached <file>
```

---

# 22) Частые команды одной строкой

```bash
git status
git add .
git commit -m "Message"
git push
git pull
git fetch
git merge branch-name
git rebase main
git stash
git stash pop
git log --oneline --graph --decorate --all
git reflog
```

---

# 23) Самые полезные сценарии “вопрос → команда”

## “Я изменил файл, но передумал”

```bash
git restore file.txt
```

## “Я добавил файл в staging, но передумал”

```bash
git restore --staged file.txt
```

## “Хочу сохранить изменения, но переключиться на другую ветку”

```bash
git stash
git switch other-branch
```

## “Я сделал коммит, но хочу исправить сообщение”

```bash
git commit --amend -m "New message"
```

## “Я сделал лишний локальный коммит”

```bash
git reset --soft HEAD~1
```

## “Я уже запушил плохой коммит”

```bash
git revert <hash>
```

## “Хочу обновить свою ветку из main”

```bash
git checkout my-branch
git rebase main
```

## “Хочу слить feature в main”

```bash
git checkout main
git merge feature
```

---

# 24) Безопасные и опасные команды

## Обычно безопасны

* `git status`
* `git log`
* `git diff`
* `git fetch`
* `git stash`
* `git revert`

## Осторожно

* `git reset --hard`
* `git rebase` на общей ветке
* `git push --force`

---

# 25) Push --force и почему его боятся

```bash
git push --force
```

Он перезаписывает удалённую историю. Это может сломать работу другим людям.

Если очень нужно, безопаснее:

```bash
git push --force-with-lease
```

Он хотя бы проверяет, что удалённая ветка не изменилась неожиданно.

---

# 26) Хороший стандартный workflow

Один из самых удобных вариантов:

```bash
git checkout main
git pull
git switch -c feature/x
# работа
git add .
git commit -m "..."
git push -u origin feature/x
```

Перед финальным PR:

```bash
git fetch origin
git rebase origin/main
```

Потом решаешь конфликты, если есть.

---

# 27) Мини-словарь

* **commit** — снимок состояния проекта;
* **branch** — линия истории;
* **merge** — объединение веток;
* **rebase** — перенос коммитов на новую основу;
* **stash** — временная отложенная пачка изменений;
* **HEAD** — указатель на текущий коммит/ветку;
* **remote** — удалённый репозиторий;
* **index / staging** — область подготовки к коммиту.

---

# 28) Практическая памятка для новичка в команде

Запомнить достаточно это:

```bash
git status
git add .
git commit -m "..."
git pull
git push
git checkout -b branch
git merge branch
git stash
git reflog
```

# 29) Untitled

```bash
```bash
git branch -f <branch-which> <branch-where>
```
```bash
git branch -f <branch-which> <HEAD>
```
```bash
git branch -f <branch-which> <HEAD>
```

Переместить указатель ветки
```bash
git fsck --lost-found | grep 'dangling commit' | awk '{print $3}' | while read c; do git branch "lost-$c" $c; done
```
Обнаружить все потерянные коммиты

```bash
git branch | grep 'lost-' | xargs git branch -D
```
Удалить все потерянные ветки

```bash
git branch --merged <branch>
```
Посмотреть слитые ветки

```bash
git log --oneline <branch1>..<branch2>
```

Посмотреть разницу  в коммитах между ветками

```bash
git branch --merged <branch> | grep -vE '^\*|main|master|develop' | xargs git branch -d
```
```bash
git branch --merged main | grep -vE '^\*|main|master|develop' | xargs git branch -d
```
```bash
git branch --merged develop | grep -vE '^\*|main|master|develop' | xargs git branch -d
```
Удалить все слитые ветки исключая `'^\*|main|master|develop'` из ветки.

```bash
git fetch --prune

git branch -r --merged develop | grep -vE 'origin/main|origin/master|origin/develop' | sed 's/origin\///' | xargs -I {} git push origin --delete {}
```

Более опасная команда, посылает сигнал об удалений на сервер, использовать с осторожностью. 

```bash
git fetch --prune
```
Обновляет указатели на удаленные ветки

```bash
git gc --prune=now
```
Встроенная оичстка репозитория git

```bash
git diff --name-only HEAD <COMMIT_HASH> -- '*.cs'
```
Находит все скрипты от нынешнего до указанного коммита

```bash
git diff-tree --no-commit-id --name-only -r <COMMIT_HASH> -- '*.cs'
```
Находит все скрипты внутри указанного коммита

```bash
git checkout <COMMIT_HASH> -- <file>
```
Восстанавлиавет файл с коммита.

```bash
git checkout --ours .
```
При конфликте оставить все свои изменения

И ещё 3 ключевых правила:

1. Сначала смотри `git status`.
2. Перед опасными командами думай, что будет с историей.
3. Для общей ветки чаще используй `revert`, а не `reset`.

---
