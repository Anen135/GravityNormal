# Space HUD — Unity-ready assets

Содержимое архива разбито на независимые PNG с прозрачностью.

## Структура

- `Panels/`
  - `panel_generic_460x220.png` — универсальная HUD-панель.
  - `panel_left_460x220.png`, `panel_right_460x220.png` — готовые варианты.
  - `divider_400x8.png` — разделительная линия.
- `Icons/`
  - `icon_speed.png`
  - `icon_health.png`
  - `icon_distance.png`
  - `icon_coin.png`
- `Bars/`
  - `bar_health_background.png`
  - `bar_health_fill.png`
  - `bar_speed_segmented_background.png`
  - `bar_speed_segmented_fill.png`
- `Reticle/`
  - `reticle_center.png`
  - `reticle_bottom_arc.png`
- `Reference/hud_reference.png` — исходный макет.

## Как использовать в Unity

1. Импортируй PNG в `Assets/UI/SpaceHUD/`.
2. Для каждого файла:
   - Texture Type: `Sprite (2D and UI)`
   - Alpha Is Transparency: `On`
   - Compression: `None` или `High Quality`
3. Панели лучше использовать через `Image`.
4. Текст (`SPEED`, `HEALTH`, числа и т.п.) не запечён в ассеты — используй TextMeshPro.
5. `bar_health_fill.png`:
   - `Image Type = Filled`
   - `Fill Method = Horizontal`
6. Для изменения размеров панели без растягивания углов можно открыть Sprite Editor и настроить Border для 9-slice.
7. Иконки, полосы и прицелы сделаны отдельными слоями, поэтому их можно независимо перекрашивать через `Image.color`.

Макет рассчитан на HUD с холодным cyan-акцентом, зелёным здоровьем и жёлтыми монетами.
