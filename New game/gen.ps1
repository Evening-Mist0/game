$path = "E:/Unity/GameJam/KaiTuoXin/game/New game/Assets/Scripts/Level/Card/CardEffectControl.cs"
$src = Get-Content "E:/Unity/GameJam/KaiTuoXin/game/New game/Assets/Scripts/Level/Card/CardEffectUIControl.cs" -Raw -Encoding UTF8

# 1. Replace class name and remove IPointerDownHandler, IPointerUpHandler
$src = $src -replace 'public class CardEffectUIControl : MonoBehaviour, IBeginDragHandler, IDragHandler,\r?\n    IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler',
    'public class CardEffectControl : MonoBehaviour, IBeginDragHandler, IDragHandler,`r`n    IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler'

[System.IO.File]::WriteAllText($path, $src, [System.Text.Encoding]::UTF8)
Write-Output 'Step 1 done'