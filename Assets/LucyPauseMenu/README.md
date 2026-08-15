# Lucy Pause Menu

## Что делает

- `Esc` открывает/закрывает паузу.
- При паузе `Time.timeScale = 0`.
- `Resume` продолжает игру.
- `Settings` открывает дочернюю панель Settings.
- `Main Menu` загружает сцену из `mainMenuSceneName` (по умолчанию `MainMenu`).
- При паузе курсор разблокируется и показывается.
- После Resume курсор снова скрывается/блокируется.
- Если на сцене нет EventSystem, он создаётся автоматически.
- Поддерживается как новый Input System, так и старый Input Manager через compile defines.

## Установка

1. Скопируй папки `Runtime` и `Editor` в проект, например:
   `Assets/Lucy/Features/UI/PauseMenu/`
2. Дождись компиляции.
3. Выполни:
   `Tools -> Lucy -> UI -> Create Pause Menu Prefab`
4. Unity создаст:
   `Assets/Lucy/Features/UI/Prefabs/PauseMenu.prefab`
5. Перетащи `PauseMenu.prefab` на игровую сцену.

После этого меню уже работает.

## Main Menu

По умолчанию ищется сцена:
1. значение `mainMenuSceneName`;
2. `MainMenu`;
3. `Menu`.

Сцена должна быть добавлена в Build Settings / Build Profiles.

## Settings

В prefab есть простая Settings-панель-заглушка.
Если у тебя уже есть собственный Audio/Settings UI:

- помести его внутрь `SettingsPanel`, либо
- замени ссылку `settingsPanel` в компоненте `PauseMenu`.

Кнопка `BACK` уже возвращает в основное меню паузы.
