using System.Drawing.Drawing2D;

namespace TicTacToe
{
    public partial class MainForm : Form
    {
        // ── Colour palette ────────────────────────────────────
        static readonly Color BG = Color.FromArgb(15, 15, 26);
        static readonly Color PanelBg = Color.FromArgb(22, 22, 40);
        static readonly Color GridClr = Color.FromArgb(42, 42, 74);
        static readonly Color XClr = Color.FromArgb(255, 107, 107);
        static readonly Color OClr = Color.FromArgb(78, 205, 196);
        static readonly Color TxtClr = Color.FromArgb(232, 232, 240);
        static readonly Color DimClr = Color.FromArgb(85, 85, 112);
        static readonly Color EasyClr = Color.FromArgb(107, 203, 119);
        static readonly Color MedClr = Color.FromArgb(255, 209, 102);
        static readonly Color HardClr = Color.FromArgb(239, 71, 111);
        static readonly Color WinClr = Color.FromArgb(255, 230, 109);

        // ── Layout constants ──────────────────────────────────
        const int CellSize = 130;
        const int CellPad = 14;
        const int LineW = 9;
        const int Radius = 46;

        // ── Game state ────────────────────────────────────────
        string?[] _board = new string?[9];
        string _current = "X";           // X = human, O = AI
        bool _gameOver;
        Difficulty _diff = Difficulty.Medium;
        int[]? _winLine;
        int[] _score = { 0, 0, 0 };  // [X wins, O wins, draws]

        // ── Controls ─────────────────────────────────────────
        Panel _canvas = null!;
        Label _lblStatus = null!;
        Label _lblX = null!;
        Label _lblO = null!;
        Label _lblDraw = null!;
        RadioButton _rbEasy = null!;
        RadioButton _rbMed = null!;
        RadioButton _rbHard = null!;

        readonly System.Windows.Forms.Timer _aiTimer = new() { Interval = 450 };

        // ─────────────────────────────────────────────────────
        public MainForm()
        {
            InitializeComponent();   // calls Designer file
            BuildUI();
            NewGame();
            _aiTimer.Tick += (_, _) => { _aiTimer.Stop(); DoAiMove(); };
        }

        // ── Build all controls ────────────────────────────────
        void BuildUI()
        {
            int boardPx = CellSize * 3 + CellPad * 4;   // 446 px
            int formW = boardPx + 60;                   // 506 px

            int y = 10;

            // Title
            Controls.Add(MakeLbl("TIC‑TAC‑TOE", 18, FontStyle.Bold, TxtClr,
                                  new Rectangle(0, y, formW, 36), ContentAlignment.MiddleCenter));
            y += 36;

            Controls.Add(MakeLbl("MinMax AI", 9, FontStyle.Regular, DimClr,
                                  new Rectangle(0, y, formW, 20), ContentAlignment.MiddleCenter));
            y += 24;

            // Difficulty row
            Controls.Add(MakeLbl("DIFFICULTY:", 9, FontStyle.Bold, DimClr,
                                  new Rectangle(20, y + 2, 90, 20), ContentAlignment.MiddleLeft));

            _rbEasy = MakeRadio("Easy", EasyClr, new Rectangle(118, y, 70, 24));
            _rbMed = MakeRadio("Medium", MedClr, new Rectangle(192, y, 80, 24));
            _rbHard = MakeRadio("Hard", HardClr, new Rectangle(276, y, 70, 24));
            _rbMed.Checked = true;

            _rbEasy.CheckedChanged += OnDiffChanged;
            _rbMed.CheckedChanged += OnDiffChanged;
            _rbHard.CheckedChanged += OnDiffChanged;
            y += 30;

            // Score panel
            var scorePanel = new Panel
            {
                BackColor = PanelBg,
                Bounds = new Rectangle(20, y, formW - 40, 64),
            };
            Controls.Add(scorePanel);

            _lblX = AddScoreCol(scorePanel, "YOU (X)", XClr, 0, formW - 40);
            _lblO = AddScoreCol(scorePanel, "AI  (O)", OClr, 1, formW - 40);
            _lblDraw = AddScoreCol(scorePanel, "DRAW", DimClr, 2, formW - 40);
            y += 70;

            // Board canvas
            _canvas = new Panel
            {
                Bounds = new Rectangle(20, y, boardPx, boardPx),
                BackColor = BG,
            };
            _canvas.Paint += OnBoardPaint;
            _canvas.MouseClick += OnBoardClick;
            Controls.Add(_canvas);
            y += boardPx + 8;

            // Status label
            _lblStatus = MakeLbl("", 11, FontStyle.Regular, TxtClr,
                                  new Rectangle(0, y, formW, 28), ContentAlignment.MiddleCenter);
            Controls.Add(_lblStatus);
            y += 32;

            // New Game button
            var btn = new Button
            {
                Text = "NEW GAME",
                FlatStyle = FlatStyle.Flat,
                BackColor = GridClr,
                ForeColor = TxtClr,
                Font = new Font("Courier New", 10f, FontStyle.Bold),
                Bounds = new Rectangle(formW / 2 - 70, y, 140, 34),
                Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (_, _) => NewGame();
            Controls.Add(btn);
            y += 50;

            ClientSize = new Size(formW, y);
        }

        // ── Helper: score column inside score panel ───────────
        Label AddScoreCol(Panel parent, string header, Color color, int col, int panelW)
        {
            int colW = panelW / 3;
            int x = col * colW;

            var h = MakeLbl(header, 8, FontStyle.Bold, color,
                             new Rectangle(x, 4, colW, 20), ContentAlignment.MiddleCenter);
            parent.Controls.Add(h);

            var v = MakeLbl("0", 13, FontStyle.Bold, TxtClr,
                             new Rectangle(x, 26, colW, 28), ContentAlignment.MiddleCenter);
            parent.Controls.Add(v);
            return v;
        }

        // ── Helper: generic label ─────────────────────────────
        static Label MakeLbl(string text, float size, FontStyle style, Color fg,
                              Rectangle bounds, ContentAlignment align)
            => new Label
            {
                Text = text,
                ForeColor = fg,
                BackColor = Color.Transparent,
                Font = new Font("Courier New", size, style),
                Bounds = bounds,
                TextAlign = align,
                AutoSize = false,
            };

        // ── Helper: radio button ──────────────────────────────
        RadioButton MakeRadio(string text, Color color, Rectangle bounds)
        {
            var rb = new RadioButton
            {
                Text = text,
                ForeColor = color,
                BackColor = BG,
                Font = new Font("Courier New", 9f, FontStyle.Bold),
                Bounds = bounds,
                Cursor = Cursors.Hand,
            };
            Controls.Add(rb);
            return rb;
        }

        // ── Game flow ─────────────────────────────────────────
        void NewGame()
        {
            _board = new string?[9];
            _current = "X";
            _gameOver = false;
            _winLine = null;
            _aiTimer.Stop();
            _canvas.Invalidate();
            SetStatus("Your turn  (X)");
        }

        void OnDiffChanged(object? sender, EventArgs e)
        {
            if (_rbEasy.Checked) _diff = Difficulty.Easy;
            else if (_rbMed.Checked) _diff = Difficulty.Medium;
            else _diff = Difficulty.Hard;
            NewGame();
        }

        void MakeMove(int idx, string player)
        {
            _board[idx] = player;
            _canvas.Invalidate();

            var result = GameLogic.CheckWinner(_board);
            if (result != null) { EndGame(result); return; }

            _current = player == "X" ? "O" : "X";
            if (_current == "O")
            {
                SetStatus("AI is thinking…");
                _aiTimer.Start();
            }
            else
            {
                SetStatus("Your turn  (X)");
            }
        }

        void DoAiMove()
        {
            if (_gameOver) return;
            int idx = GameLogic.AiMove(_board, _diff);
            if (idx >= 0) MakeMove(idx, "O");
        }

        void EndGame(string result)
        {
            _gameOver = true;
            _winLine = GameLogic.GetWinningLine(_board);
            _canvas.Invalidate();

            if (result == "X")
            {
                _score[0]++;
                _lblX.Text = _score[0].ToString();
                SetStatus("🎉  You win!");
            }
            else if (result == "O")
            {
                _score[1]++;
                _lblO.Text = _score[1].ToString();
                SetStatus($"🤖  AI wins!  ({_diff})");
            }
            else
            {
                _score[2]++;
                _lblDraw.Text = _score[2].ToString();
                SetStatus("🤝  It's a draw!");
            }
        }

        void SetStatus(string msg) => _lblStatus.Text = msg;

        // ── Board painting ────────────────────────────────────
        void OnBoardPaint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int total = CellSize * 3 + CellPad * 4;

            // Grid lines
            using var gridPen = new Pen(GridClr, 3)
            { StartCap = LineCap.Round, EndCap = LineCap.Round };
            for (int i = 1; i < 3; i++)
            {
                int pos = CellPad + i * (CellSize + CellPad) - CellPad / 2;
                g.DrawLine(gridPen, pos, CellPad, pos, total - CellPad);
                g.DrawLine(gridPen, CellPad, pos, total - CellPad, pos);
            }

            // Win highlight
            if (_winLine != null)
            {
                using var winBrush = new SolidBrush(Color.FromArgb(50, WinClr));
                using var winPen = new Pen(Color.FromArgb(160, WinClr), 2);
                foreach (int idx in _winLine)
                {
                    var (rx, ry, rw, rh) = CellRect(idx);
                    var rect = new Rectangle(rx, ry, rw, rh);
                    g.FillRectangle(winBrush, rect);
                    g.DrawRectangle(winPen, rect);
                }
            }

            // Pieces
            for (int i = 0; i < 9; i++)
                if (_board[i] != null)
                    DrawPiece(g, i, _board[i]!);
        }

        void DrawPiece(Graphics g, int idx, string val)
        {
            var (rx, ry, rw, rh) = CellRect(idx);
            int cx = rx + rw / 2;
            int cy = ry + rh / 2;

            if (val == "X")
            {
                int off = CellSize / 2 - 26;
                using var pen = new Pen(XClr, LineW)
                { StartCap = LineCap.Round, EndCap = LineCap.Round };
                g.DrawLine(pen, cx - off, cy - off, cx + off, cy + off);
                g.DrawLine(pen, cx + off, cy - off, cx - off, cy + off);
            }
            else
            {
                using var pen = new Pen(OClr, LineW);
                g.DrawEllipse(pen, cx - Radius, cy - Radius, Radius * 2, Radius * 2);
            }
        }

        (int x, int y, int w, int h) CellRect(int idx)
        {
            int row = idx / 3, col = idx % 3;
            int x = CellPad + col * (CellSize + CellPad);
            int y = CellPad + row * (CellSize + CellPad);
            return (x, y, CellSize, CellSize);
        }

        // ── Click handling ────────────────────────────────────
        void OnBoardClick(object? sender, MouseEventArgs e)
        {
            if (_gameOver || _current != "X" || _aiTimer.Enabled) return;

            int total = CellSize * 3 + CellPad * 4;
            int col = e.X * 3 / total;
            int row = e.Y * 3 / total;
            if (col < 0 || col > 2 || row < 0 || row > 2) return;

            int idx = row * 3 + col;
            if (_board[idx] == null)
                MakeMove(idx, "X");
        }
    }
}