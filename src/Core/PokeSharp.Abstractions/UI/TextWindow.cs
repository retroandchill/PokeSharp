using System.Drawing;
using PokeSharp.Core;

namespace PokeSharp.UI;

public interface ITextWindow : IWindow
{
    Text Text { get; set; }

    Color BaseColor { get; set; }

    Color ShadowColor { get; set; }

    bool LetterByLetter { get; set; }
}
