Конечно! Вот пример **реальной практической задачи линейного программирования** из бизнеса 📦:

---

## ✅ **Задача: Производственный план**

**Фабрика** производит два продукта — **Стул (x₁)** и **Стол (x₂)**.  
Для производства требуются ресурсы:

| Ресурс        | На 1 стул | На 1 стол | Доступно |
|---------------|-----------|-----------|----------|
| Дерево (м²)   | 3         | 5         | 240      |
| Человеко-часы | 2         | 3         | 150      |

**Прибыль:**
- Стул — $30
- Стол — $50

---

## 📌 Цель:
Составить такой план производства, чтобы **максимизировать прибыль** при ограничениях по ресурсам.

---

## 🧮 Математическая модель (ЛП):

**Переменные:**
- x₁ — количество стульев
- x₂ — количество столов

**Целевая функция (что максимизируем):**
```
Max Z = 30x₁ + 50x₂
```

**Ограничения:**
```
3x₁ + 5x₂ ≤ 240   (по дереву)
2x₁ + 3x₂ ≤ 150   (по труду)
x₁ ≥ 0, x₂ ≥ 0    (нельзя производить отрицательно)
```

---

### 📄 Файл `lp_furniture.txt` для твоей программы:

```txt
3 5 | 240
2 3 | 150
obj: 30 50
```

Ты можешь скопировать это в `.txt` и загрузить через свою программу.

---

Хочешь — сделаю аналогичную задачу про логистику, финансы или рекламу?


Transport1 answer:
```text
   10   10    0    0
    0   20   10    0
    0    0   15   10
```

Transport2 answer:
```text
   20   20    0
    0   10   20
    0    0   20
```
Transport3 answer:
```text
   15    0    0
   25    0    0
    0   10    0
    0   20   10
```

Transport4 answer:
```text
   10   10
    0   30
```

***
### Линейное программирование

---

### ✅ **Пример 1 — Классика: максимизация прибыли**

**`lp1.txt`**
```txt
2 1 | 100
1 1 | 80
obj: 40 30
```

📌 Модель:
- Max Z = 40x₁ + 30x₂
- при 2x₁ + x₂ ≤ 100  
  x₁ + x₂ ≤ 80

✅ **Ответ:**
- x₁ = 20
- x₂ = 60
- Z = **3000**

---

### ✅ **Пример 2 — Простое ограничение труда и материалов**

**`lp2.txt`**
```txt
1 3 | 90
2 1 | 80
obj: 50 20
```

📌 Модель:
- Max Z = 50x₁ + 20x₂
- при x₁ + 3x₂ ≤ 90  
  2x₁ + x₂ ≤ 80

✅ **Ответ:**
- x₁ = 30
- x₂ = 20
- Z = **1900**

---

### ✅ **Пример 3 — Производственная задача**

**`lp3.txt`**
```txt
3 2 | 60
2 5 | 100
obj: 10 20
```

📌 Модель:
- Max Z = 10x₁ + 20x₂
- при 3x₁ + 2x₂ ≤ 60  
  2x₁ + 5x₂ ≤ 100

✅ **Ответ:**
- x₁ = 10
- x₂ = 6
- Z = **220**

---

