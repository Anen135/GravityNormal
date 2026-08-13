# Space HUD — Prefab Builder

Файлы рассчитаны на то, что HUD-спрайты уже лежат прямо в:

`Assets/SpaceHUD/`

Ожидаемые имена:

- `panel_left_460x220.png`
- `panel_right_460x220.png`
- `divider_400x8.png`
- `icon_speed.png`
- `icon_health.png`
- `icon_distance.png`
- `icon_coin.png`
- `bar_health_background.png`
- `bar_health_fill.png`
- `bar_speed_segmented_background.png`
- `bar_speed_segmented_fill.png`
- `reticle_center.png`
- `reticle_bottom_arc.png`

## Установка

1. Скопируй `Runtime/SpaceHUDView.cs` в проект, например:
   `Assets/Scripts/UI/SpaceHUDView.cs`

2. Скопируй `Editor/SpaceHUDPrefabBuilder.cs` обязательно в Editor-папку, например:
   `Assets/Editor/SpaceHUDPrefabBuilder.cs`

3. Дождись компиляции Unity.

4. Запусти:
   `Tools -> Lucy -> Space HUD -> Create Prefab`

5. Скрипт автоматически создаст:
   `Assets/SpaceHUD/SpaceHUD.prefab`

6. Перетащи `SpaceHUD.prefab` на сцену.

## Что создаётся

Canvas:
- Screen Space Overlay
- Reference Resolution: 1920x1080
- Scale With Screen Size
- Match: 0.5

Слева:
- SPEED
- speed value
- segmented speed bar
- HEALTH
- health percentage
- health bar

Справа:
- CLOSEST DISTANCE
- distance value
- COINS
- coin counter

По центру:
- crosshair
- нижняя HUD-дуга

## Обновление HUD из кода

```csharp
public SpaceHUDView hud;

void Update()
{
    hud.SetSpeed(shipRigidbody.linearVelocity.magnitude);
    hud.SetHealth(currentHealth / maxHealth);
    hud.SetClosestDistance(distance);
    hud.SetCoins(coins, 5);
}
```

Префаб генерируется непосредственно внутри Unity, потому что Unity назначает GUID импортированным спрайтам локально. Готовый `.prefab`, созданный вне твоего проекта, не смог бы надёжно ссылаться на уже импортированные PNG.
