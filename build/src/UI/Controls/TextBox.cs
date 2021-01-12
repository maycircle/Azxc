using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Harmony;
using DuckGame;

using Azxc.Bindings;
using Azxc.UI.Events;

namespace Azxc.UI.Controls
{
    class InputDialog : Window, IDialog, IBinding
    {
        private Label<FancyBitmapFont> _dialogTitle, _userInput;

        private bool _showDialogTitle;

        private string _hintsTextBackup;

        public string text { get; private set; }
        public DialogResult dialogResult { get; set; }

        public InputDialog(Vec2 position, bool dialogTitle) : base(position, SizeModes.Flexible)
        {
            _showDialogTitle = dialogTitle;
            dialogResult = DialogResult.Idle;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            FancyBitmapFont hugeFont = new FancyBitmapFont("smallFont");
            hugeFont.scale = new Vec2(0.5f);

            _dialogTitle = new Label<FancyBitmapFont>("Default title:", Azxc.core.uiManager.font);
            if (_showDialogTitle)
                AddItem(_dialogTitle);
            _userInput = new Label<FancyBitmapFont>("Default text_", hugeFont); AddItem(_userInput);
        }

        [Binding(Keys.Right, InputState.Pressed)]
        public void Accept()
        {
            if (_showFlashingCursor)
                text = _userInput.text.Remove(_userInput.text.Length - 1);
            else
                text = _userInput.text;
            dialogResult = DialogResult.Accept;
        }

        private float _flashingCursorFrame;
        private bool _showFlashingCursor;
        public override void Update()
        {
            base.Update();

            // To prevent "accepting event" on the dialog start-up
            if (_flashingCursorFrame != 0)
                BindingManager.UsedBinding(this, "Accept");

            UpdateUserInput();
            UpdateFlashingCursor();
        }

        private void UpdateUserInput()
        {
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

        public void ThrowResult()
        {
            OnResult(new ControlEventArgs(this));
        }

        public event EventHandler<ControlEventArgs> onResult;
        protected void OnResult(ControlEventArgs e)
        {
            onResult?.Invoke(this, e);
        }

        public void ShowDialog(string title, string defaultText)
        {
            _dialogTitle.text = title;
            _userInput.text = defaultText;
            Keyboard.keyString = defaultText;
            Azxc.core.uiManager.inputHook = true;

            width = 80.0f;
            x -= width / 2;

            Azxc.core.uiManager.forceHints = true;
            _hintsTextBackup = Azxc.core.uiManager.hintsText;
            Azxc.core.uiManager.hintsText = "@AZXCACTIVATE@ACCEPT  @AZXCRIGHTMOUSE@@AZXCBACK@CANCEL";
            Show();
        }

        public override void Close()
        {
            base.Close();
            Azxc.core.uiManager.inputHook = false;
            Azxc.core.uiManager.forceHints = false;
            Azxc.core.uiManager.hintsText = _hintsTextBackup;
        }

        public override void Appear()
        {
            if (Keyboard.keyString != _userInput.text)
                Keyboard.keyString = _userInput.text;
        }
    }

    public class TextBox<T> : Button<T>
    {
        private string _shadowText;

        public string inputDialogTitle { get; set; }
        public string placeholderText { get; set; }

        public bool nullOrWhitespace { get; set; }

        public TextBox(string text, T font) : base(text, font)
        {
            nullOrWhitespace = false;
        }

        public TextBox(string text, string placeholderText, T font) : base(text, font)
        {
            this.placeholderText = placeholderText;
            nullOrWhitespace = false;
        }

        public TextBox(string text, string placeholderText, string toolTipText, T font) : 
            base(text, toolTipText, font)
        {
            this.placeholderText = placeholderText;
            nullOrWhitespace = false;
        }

        public override void Update()
        {
            float textWidth = GetWidth() + indent.x * 2;

            MethodInfo getWidth = AccessTools.Method(typeof(T), "GetWidth");
            float placeholderTextWidth = (float)getWidth.Invoke(font,
                new object[] { placeholderText, false }) + indent.x * 2;

            width = textWidth >= placeholderTextWidth ? textWidth : placeholderTextWidth;
            height = characterHeight * GetScale().y + indent.y * 2;
        }

        public override void Draw()
        {
            Vec2 start = new Vec2(x + 0.1f, y);
            Vec2 end = new Vec2(x + width, y + height + 0.1f);

            float border = 0.5f;
            Graphics.DrawRect(start, end, selected ? new Color(59, 109, 79) : new Color(39, 69, 49),
                0.9f, false, border);

            // TODO: I should do something with this...
            MethodInfo draw = AccessTools.Method(typeof(T), "Draw",
                new Type[] { typeof(string), typeof(Vec2), typeof(Color), typeof(Depth), typeof(bool) });
            MethodInfo getWidth = AccessTools.Method(typeof(T), "GetWidth");

            if (!string.IsNullOrEmpty(_shadowText) &&
                (float)getWidth.Invoke(font, new object[] { _shadowText, false }) + 2.0f >= width)
            {
                text = "";
                draw.Invoke(font, new object[] { "...", position + indent,
                    Color.Gray, new Depth(1f), true });
            }
            else if ((!nullOrWhitespace && string.IsNullOrEmpty(_shadowText)) ||
                (nullOrWhitespace && string.IsNullOrWhiteSpace(_shadowText)))
            {
                // Draw placeholder text
                draw.Invoke(font, new object[] { placeholderText, position + indent,
                    Color.Gray, new Depth(1f), true });
            }
            else
                text = _shadowText;

            base.Draw();
        }

        public override void Click()
        {
            base.Click();

            Edit();
        }

        public virtual void Edit()
        {
            Vec2 position = new Vec2(Layer.HUD.width / 2, Layer.HUD.height / 2);
            InputDialog inputDialog = new InputDialog(position, !string.IsNullOrEmpty(inputDialogTitle));
            inputDialog.onResult += InputDialog_Result;
            inputDialog.ShowDialog(inputDialogTitle, _shadowText);
        }

        private void InputDialog_Result(object sender, ControlEventArgs e)
        {
            InputDialog inputDialog = e.item as InputDialog;
            _shadowText = inputDialog.text;
        }
    }
}
