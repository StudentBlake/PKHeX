﻿using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    /// <summary>
    /// Utility logic for drawing individual Slot views that represent underlying <see cref="PKM"/> data.
    /// </summary>
    public static class SlotUtil
    {
        /// <summary>
        /// Gets the background image for a slot based on the provided <see cref="type"/>.
        /// </summary>
        public static Image GetTouchTypeBackground(SlotTouchType type)
        {
            return type switch
            {
                SlotTouchType.None => Resources.slotTrans,
                SlotTouchType.Get => Resources.slotView,
                SlotTouchType.Set => Resources.slotSet,
                SlotTouchType.Delete => Resources.slotDel,
                SlotTouchType.Swap => Resources.slotSet,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        /// <summary>
        /// Gets the type of action that should be performed for a Drag &amp; Drop request.
        /// </summary>
        public static DropModifier GetDropModifier()
        {
            return Control.ModifierKeys switch
            {
                Keys.Shift => DropModifier.Clone,
                Keys.Alt => DropModifier.Overwrite,
                _ => DropModifier.None
            };
        }

        /// <summary>
        /// Refreshes a <see cref="PictureBox"/> with the appropriate display content.
        /// </summary>
        public static void UpdateSlot(PictureBox pb, ISlotInfo c, PKM p, SaveFile s, bool flagIllegal, SlotTouchType t = SlotTouchType.None)
        {
            pb.BackgroundImage = GetTouchTypeBackground(t);
            if (p.Species == 0) // Nothing in slot
            {
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                return;
            }
            if (!p.Valid) // Invalid
            {
                // Bad Egg present in slot.
                pb.Image = null;
                pb.BackColor = Color.Red;
                return;
            }

            var img = c is SlotInfoBox b
                ? p.Sprite(s, b.Box, b.Slot, flagIllegal)
                : c is SlotInfoParty ps
                    ? p.Sprite(s, -1, ps.Slot, flagIllegal)
                    : p.Sprite(s, -1, -1, flagIllegal);

            pb.BackColor = Color.Transparent;
            pb.Image = img;
        }
    }
}