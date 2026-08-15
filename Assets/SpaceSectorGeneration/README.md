# Space Sector Generation

Процедурная генерация и сохранение 3D-секторов вокруг игрока для Unity.

## Что делает модуль

- Делит бесконечное пространство на 3D-сектора по `Vector3Int`.
- Загружает куб секторов вокруг `target`.
- При первом посещении генерирует содержимое по `worldSeed + coordinate`.
- Сразу сохраняет впервые сгенерированный сектор в JSON.
- При возвращении загружает сохранённый JSON, а не генерирует сектор заново.
- Сохраняет position / rotation / scale / active каждого объекта.
- Запоминает уничтоженные объекты.
- Сохраняет сектора при выгрузке, автосохранении и выходе из игры.
- Хранит каждый сектор отдельным файлом.

## Интеграция

1. Скопировать папку `Runtime` в проект, например:
   `Assets/Lucy/Features/SpaceSectors/Runtime/`.

2. Создать объект `SpaceSectorSystem`.

3. Добавить на него:
   - `SpaceSectorManager`
   - `RandomSectorContentGenerator`

4. В `SpaceSectorManager` назначить:
   - `Target` -> Transform корабля игрока.
   - `Content Generator` -> `RandomSectorContentGenerator` с этого же объекта.
   - `Sector Size` -> например `5000`.
   - `Load Radius` -> например `(1, 1, 1)` = 27 одновременно загруженных секторов.
   - `World Seed` -> любой int.
   - `Save Slot` -> имя сохранения, например `MainWorld`.

5. В `RandomSectorContentGenerator -> Spawn Rules` добавить правила.

Пример для планеты:

- Prefab Id: `PlanetRock`
- Prefab: prefab планеты
- Sector Chance: `0.25`
- Min Count: `1`
- Max Count: `2`
- Border Padding: `500`
- Minimum Spacing: `1500`
- Min Scale: `0.8`
- Max Scale: `1.5`

При `Sector Chance = 0.25` только около 25% новых секторов получат объекты этого правила.

## Сохранения

Путь можно увидеть в `SpaceSectorManager -> Debug -> Save Directory`.

Фактически используется:

`Application.persistentDataPath/SpaceSectors/<SaveSlot>/`

Внутри:

- `world.json` — seed мира.
- `sector_0_0_0.json`
- `sector_0_0_1.json`
- `sector_-1_0_1.json`
- и т.д.

Seed записывается в `world.json`. Если в Inspector потом поменять `World Seed`, уже существующий Save Slot продолжит использовать seed, с которым он был создан. Для другого мира используйте другой `Save Slot` или удалите старое сохранение.

## Уничтожение объектов

На каждый сгенерированный объект автоматически добавляется `SectorPersistentObject`.

Обычный:

```csharp
Destroy(gameObject);
```

считается игровым уничтожением. Объект будет помечен как `destroyed = true` и после возврата в сектор больше не появится.

Во время технической выгрузки сектора менеджер заранее вызывает `PrepareForSectorUnload()`, поэтому выгрузка не считается уничтожением объекта игроком.

## Сохранение изменённого объекта вручную

Если объект изменился и нужно немедленно записать его состояние:

```csharp
var persistentObject = GetComponent<SectorPersistentObject>();
persistentObject.SaveNow();
```

Также работает автосохранение через `Auto Save Interval`.

## Важное ограничение для врагов

Текущая версия считает объект принадлежащим тому сектору, в котором он был создан. Это идеально для планет, станций, астероидов и статичных объектов.

Для врагов, которые способны перелетать через границы сектора, лучше сделать отдельную миграцию persistent-объектов между секторами либо хранить врагов в отдельном динамическом world-state. Иначе враг останется записан в исходном секторе.
