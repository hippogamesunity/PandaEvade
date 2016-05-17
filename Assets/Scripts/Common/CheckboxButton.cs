using System;

namespace Assets.Scripts.Common
{
    public class CheckboxButton : GameButton
    {
        public string Checked;
        public string Unckecked;
        public Func<bool> Condition;

        public override void OnPress(bool down)
        {
            base.OnPress(down);
            Refresh();
        }

        public void Refresh()
        {
            if (Condition != null)
            {
                Get<UISprite>().spriteName = Condition() ? Checked : Unckecked;
            }
        }
    }
}