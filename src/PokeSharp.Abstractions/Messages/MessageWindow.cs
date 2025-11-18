using System.Drawing;
using PokeSharp.Core;

namespace PokeSharp.Messages;

public interface IMessageWindow
{
    Text Text { get; set; }

    Color BaseColor { get; set; }

    Color ShadowColor { get; set; }

    bool LetterByLetter { get; set; }
}
