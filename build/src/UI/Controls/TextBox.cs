using Azxc.UI.Events;
using DuckGame;
using System;

namespace Azxc.UI.Controls
{
    class InputDialog : Window, IDialog
    {
        private Label _dialogTitle, _userInput;

        private float _flashingCursorFrame;
        private bool _showFlashingCursor;

        private string _oldHintsText;

        public string text { get; private set; }
        public DialogResult dialogResult { get; set; }

        public InputDialog(Vec2 position) : base(position, SizeModes.Flexible)
        {
            dialogResult = DialogResult.Idle;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            FancyBitmapFont hugeFont = new FancyBitmapFont("smallFont");
            hugeFont.scale = new Vec2(0.5f);

            _dialogTitle = new Label(string.Empty);
            if (_dialogTitle.text == "")
            {
                _dialogTitle.text = "Default title:";
                AddItem(_dialogTitle);
            }

            _userInput = new Label("Default text_");
            _userInput.font = hugeFont;
            AddItem(_userInput);
        }

        public override void Update()
        {
            base.Update();

            // To prevent "accepting event" on the dialog start-up
            if (_flashingCursorFrame != 0 && Keyboard.Pressed(Keys.Right))
                Accept();

            UpdateUserInput();
            UpdateFlashingCursor();
        }

        private void UpdateUserInput()
        {
            Keyboard.repeat = true;
            if (Keyboard.Pressed(Keys.A, true))
            {
                _flashingCursorFrame = 0;
                _showFlashingCursor = false;
            }
            _userInput.text = Keyboard.keyString;
            if (_showFlashingCursor)
                _userInput.text += '_';
        }

        private void UpdateFlashingCursor()
        {
            _flashingCursorFrame += Maths.IncFrameTimer();
            if (_flashingCursorFrame >= 1.0f)
            {
                _showFlashingCursor = !_showFlashingCursor;
                _flashingCursorFrame = 0;
            }
        }

        public void ShowDialog(string title, string defaultText)
        {
            _dialogTitle.text = title;
            _userInput.text = defaultText;
            Keyboard.keyString = defaultText;

            KeyboardHook.repeat = true;
            Azxc.GetCore().GetUI().SetKeyboardLock(true);

            Azxc.GetCore().GetUI().forceHints = true;
            _oldHintsText = Azxc.GetCore().GetUI().hintsText;
            Azxc.GetCore().GetUI().SetHintsText(UserInterfaceCore.DialogHintsText);

            width = 80.0f;
            x -= width / 2;
            Show();
        }

        public void Accept()
        {
            if (_showFlashingCursor)
                text = _userInput.text.Remove(_userInput.text.Length - 1);
            else
                text = _userInput.text;
            dialogResult = DialogResult.Accept;
        }

        public override void Close()
        {
            base.Close();

            KeyboardHook.repeat = false;
            Azxc.GetCore().GetUI().SetKeyboardLock(false);

            Azxc.GetCore().GetUI().forceHints = false;
            Azxc.GetCore().GetUI().SetHintsText(_oldHintsText);
        }

        public override void Appear()
        {
            if (Keyboard.keyString != _userInput.text)
                Keyboard.keyString = _userInput.text;
        }

        public void ThrowResult()
        {
            OnResult(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onResult;
        protected void OnResult(ControlEventArgs e)
        {
            onResult?.Invoke(this, e);
        }
    }

    public class TextBox : Button
    {
        private string _shadowText;

        public string inputDialogTitle { get; set; }
        public string placeholderText { get; set; }

        public string inputText
        {
            get { return _shadowText; }
            set { _shadowText = value; }
        }

        public bool nullOrWhitespace { get; set; }

        public TextBox(string text) : base(text)
        {
            _shadowText = text;
            nullOrWhitespace = false;
        }

        public TextBox(string text, string placeholderText) : base(text)
        {
            this.placeholderText = placeholderText;
            _shadowText = text;
            nullOrWhitespace = false;
        }

        public TextBox(string text, string placeholderText, string toolTipText) :
            base(text, toolTipText)
        {
            this.placeholderText = placeholderText;
            _shadowText = text;
            nullOrWhitespace = false;
        }

        public override void Update()
        {
            float textWidth = font.GetWidth(text) + indent.x * 2;
            float placeholderTextWidth = font.GetWidth(placeholderText) + indent.x * 2;

            width = textWidth >= placeholderTextWidth ? textWidth : placeholderTextWidth;
            height = font.characterHeight * font.scale.y + indent.y * 2;
        }

        public override void Draw()
        {
            Vec2 start = new Vec2(x + 0.1f, y);
            Vec2 end = new Vec2(x + width, y + height + 0.1f);

            float border = 0.5f;
            Graphics.DrawRect(start, end, isSelected ? new Color(59, 109, 79) : new Color(39, 69, 49),
                0.9f, false, border);

            if (!string.IsNullOrEmpty(_shadowText) &&
                font.GetWidth(_shadowText) + 2.0f >= width)
            {
                text = "";
                // Draw "..." in case if input is too long
                font.Draw("...", position + indent, Color.Gray, new Depth(1.0f), true);
            }
            else if ((!nullOrWhitespace && string.IsNullOrEmpty(_shadowText)) ||
                (nullOrWhitespace && string.IsNullOrWhiteSpace(_shadowText)))
            {
                text = "";
                // Draw placeholder text
                font.Draw(placeholderText, position + indent, Color.Gray, new Depth(1.0f), true);
            }
            else
                text = _shadowText;

            base.Draw();
        }

        public virtual void Edit()
        {
            Vec2 position = new Vec2(Layer.HUD.width / 2, Layer.HUD.height / 2);
            InputDialog inputDialog = new InputDialog(position);
            inputDialog.onResult += InputDialog_Result;
            inputDialog.ShowDialog(inputDialogTitle, _shadowText);
        }

        private void InputDialog_Result(object sender, ControlEventArgs e)
        {
            InputDialog inputDialog = e.item as InputDialog;
            _shadowText = inputDialog.text;
            OnTextChanged(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onTextChanged;
        protected void OnTextChanged(ControlEventArgs e)
        {
            onTextChanged?.Invoke(this, e);
        }

        public override void Click()
        {
            base.Click();

            Edit();
        }
    }
}
