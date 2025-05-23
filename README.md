﻿# Dynamic-Flow-LayoutGroup

A flexible and optimized layout system for Unity UI, supporting dynamic child sizing and responsive arrangements based on available space.

## ✨ Features

- Supports both **vertical** and **horizontal** layout axes.
- Automatically handles **wrapping**, **spacing**, and **padding**.
- Works in both **runtime** and **editor mode**.
- Efficient recalculation using minimal `LayoutRebuilder` calls.
- Compatible with Unity’s built-in layout system.

## 📦 Installation

1. Clone or download this repository.
2. Copy the `FlowLayoutGroup` folder into your Unity project's `Assets/` directory.

## 🧰 Usage

1. Add the `FlowLayoutGroup` component to a `GameObject` with a `RectTransform`.
2. Add child UI elements (e.g., `Image`, `Button`, etc.).
3. Configure layout options in the inspector:
   - **Start Axis**
   - **Spacing**
   - **Child Control Width/Height**
   - **Child Alignment**

## 🧪 Demo

![Demo](Docs/demo.gif)

## 🔧 Customization

You can override methods like:

- `CalculateLayoutInputHorizontal`
- `CalculateLayoutInputVertical`
- `SetLayoutHorizontal`
- `SetLayoutVertical`

to fine-tune the behavior or integrate with custom UI logic.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributions

Feel free to open issues or submit pull requests. Contributions are welcome!
