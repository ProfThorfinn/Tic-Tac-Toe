# Tic‑Tac‑Toe — MinMax AI (C# WinForms)
 
A fully playable Tic‑Tac‑Toe game built with **C# WinForms (.NET 8)**, featuring an AI opponent powered by the **MinMax algorithm with Alpha‑Beta Pruning** and three selectable difficulty levels.
 
---
 
## 📸 Preview
 
> Human plays **X** &nbsp;·&nbsp; AI plays **O**
 
```
┌───────┬───────┬───────┐
│   X   │       │   O   │
├───────┼───────┼───────┤
│       │   X   │       │
├───────┼───────┼───────┤
│   O   │       │   X   │
└───────┴───────┴───────┘
        🎉 You win!
```
 
---
 
## 🎮 Features
 
| Feature | Details |
|---|---|
| **3 Difficulty Levels** | Easy, Medium, Hard — switchable at any time |
| **MinMax AI** | Full game‑tree search with Alpha‑Beta Pruning |
| **Score Tracker** | Wins, losses, and draws persist across rounds |
| **Win Highlight** | Winning cells glow gold at the end of each game |
| **Custom Rendering** | Board drawn entirely with GDI+ (no images needed) |
| **Dark Theme UI** | Clean dark palette with color‑coded X / O symbols |
 
---
 
## 🧠 Difficulty Levels
 
| Level | Strategy | Behaviour |
|---|---|---|
| 🟢 **Easy** | 100% Random | Picks any empty cell at random — beatable every time |
| 🟡 **Medium** | 60% MinMax + 40% Random | Mostly smart, but makes occasional mistakes |
| 🔴 **Hard** | 100% MinMax | Plays perfectly — the best possible outcome is a draw |
 
---
 
## 🗂️ Project Structure
 
```
TicTacToe/
 ├── TicTacToe.csproj        # Project configuration (.NET 8 WinForms)
 ├── Program.cs              # Entry point — Main() + [STAThread]
 ├── GameLogic.cs            # All AI logic: MinMax, CheckWinner, AiMove
 ├── MainForm.Designer.cs    # WinForms designer file (InitializeComponent)
 └── MainForm.cs             # UI controls, board painting, game state
```
 
### File responsibilities
 
**`Program.cs`**
Standard WinForms entry point. Calls `ApplicationConfiguration.Initialize()` and launches `MainForm`.
 
**`GameLogic.cs`**
Pure static class — no UI dependencies.
- `CheckWinner()` — scans all 8 winning lines, returns `"X"`, `"O"`, `"draw"`, or `null`
- `GetWinningLine()` — returns the 3 cell indices of the winning line for highlighting
- `Minimax()` — recursive MinMax with Alpha‑Beta Pruning (O = maximiser, X = minimiser)
- `BestMinimaxMove()` — wraps Minimax to return the best cell index for O
- `AiMove()` — difficulty dispatcher; routes to random, mixed, or full MinMax
**`MainForm.Designer.cs`**
Standard WinForms designer partial class containing `InitializeComponent()`. Sets core form properties (title, background, border style, font, start position).
 
**`MainForm.cs`**
Partial class paired with the designer file.
- Builds all controls programmatically in `BuildUI()`
- Handles game flow: `NewGame()`, `MakeMove()`, `DoAiMove()`, `EndGame()`
- Custom GDI+ board rendering via `OnBoardPaint()`
- Click‑to‑play via `OnBoardClick()`
- 450 ms AI move delay via `System.Windows.Forms.Timer`
---
 
## ⚙️ Requirements
 
| Requirement | Version |
|---|---|
| Operating System | Windows 10 / 11 |
| .NET SDK | 8.0 or later |
| IDE (optional) | Visual Studio 2022+ or VS Code with C# extension |
 
> WinForms is a Windows‑only framework. The project will not run on macOS or Linux.
 
---
 
## 🚀 Getting Started
 
### 1 — Clone the repository
 
```bash
git clone https://github.com/your‑username/TicTacToe‑MinMax.git
cd TicTacToe‑MinMax
```
 
### 2 — Run with .NET CLI
 
```bash
dotnet run
```
 
### 3 — Or open in Visual Studio
 
1. Open **Visual Studio 2022**
2. Click **Open a project or solution**
3. Select `TicTacToe.csproj`
4. Press **F5** to build and run
---
 
## 🔬 How the AI Works
 
### MinMax Algorithm
 
MinMax is a recursive decision‑tree algorithm used in two‑player zero‑sum games. It simulates every possible future move from the current board state, assigns a score to each terminal outcome, and works backwards to determine the optimal move.
 
```
Score:  O wins → +10 │ X wins → −10 │ Draw → 0
```
 
- **O (AI)** is the **maximiser** — always tries to reach the highest score
- **X (Human)** is the **minimiser** — always tries to reach the lowest score
### Alpha‑Beta Pruning
 
Alpha‑Beta Pruning is an optimisation that eliminates branches of the game tree that cannot affect the final decision:
 
- **Alpha** — the best score the maximiser (O) is guaranteed so far
- **Beta** — the best score the minimiser (X) is guaranteed so far
- If `beta ≤ alpha` at any node, the remaining siblings are skipped (pruned)
This reduces the number of nodes evaluated without changing the result — the AI still plays perfectly, just faster.
 
### Game Tree (simplified)
 
```
                  [Board State]
                  AI to move (O)
               /        |        \
          [O→A]        [O→B]      [O→C]
        Human moves  Human moves  Human moves
         /     \
     [X→D]   [X→E]          ✂ pruned (β ≤ α)
      +10      recurse...
```
 
---
 
## 🖼️ UI Details
 
All rendering is done with **GDI+** inside a `Panel.Paint` event:
 
| Element | Drawn with |
|---|---|
| Grid lines | `Graphics.DrawLine` with `LineCap.Round` |
| X symbol | Two diagonal `DrawLine` calls |
| O symbol | `Graphics.DrawEllipse` |
| Win highlight | `FillRectangle` + `DrawRectangle` with semi‑transparent gold |
 
Colors use `Color.FromArgb()` throughout — no external image assets required.
 
---
 
## 📄 License
 
MIT License — free to use, modify, and distribute.
 
---
 
## 🙌 Acknowledgements
 
- MinMax algorithm — classic AI technique for combinatorial games
- Alpha‑Beta Pruning — introduced by John McCarthy, refined by Donald Knuth & Ronald Moore
- Built with Microsoft WinForms / GDI+ on .NET 8
