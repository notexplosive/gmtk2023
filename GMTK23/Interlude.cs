﻿using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class Interlude : Widget, IEarlyDrawHook, IUpdateInputHook, IUpdateHook
{
    private readonly MainCartridge _mainCartridge;
    private string[]? _pages;
    private int _currentPage;
    private bool _isReady;
    private HashSet<PageDescriptor> _seenText = new();
    private float _offset;
    private int _soundIndex = 1;

    public Interlude(RectangleF rectangle, MainCartridge mainCartridge) : base(rectangle, Depth.Middle)
    {
        _mainCartridge = mainCartridge;
    }

    public void EarlyDraw(Painter painter)
    {
        Client.Graphics.PushCanvas(Canvas);

        painter.BeginSpriteBatch();

        painter.DrawAtPosition(Client.Assets.GetTexture("gmtk/producer"),Vector2.Zero);

        if (ShouldShowText)
        {
            painter.DrawStringWithinRectangle(Global.GetFont(32), _pages![_currentPage],
                new RectangleF(0, 162, Size.X, Size.Y - 162).Moved(new Vector2(0,_offset)), Alignment.TopLeft, new DrawSettings());
            
            painter.DrawStringWithinRectangle(Global.GetFont(16), "Click to continue",
                new RectangleF(0, 162, Size.X, Size.Y - 162), Alignment.BottomCenter, new DrawSettings());
        }

        painter.EndSpriteBatch();

        Client.Graphics.PopCanvas();
    }

    public bool ShouldShowText
    {
        get
        {
            return !IsAtEndOfPages && _isReady;
        }
    }

    public bool IsAtEndOfPages => _pages != null && _currentPage >= _pages.Length;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (!_mainCartridge.IsInterludeActive)
        {
            return;
        }
        UpdateHovered(hitTestStack);
        if (input.Mouse.GetButton(MouseButton.Left).WasPressed && IsHovered)
        {
            NextPage();
        }
    }

    private void NextPage()
    {
        _currentPage++;
        _offset = 10;

        PageSound();

        if (IsAtEndOfPages)
        {
            ReturnToGameplay();
        }
    }


    public void ReturnToGameplay()
    {
        _mainCartridge.SwitchToGameplay();
    }

    public void Initialize(PlayerStatistics playerStatistics, int level)
    {
        _isReady = false;
        _currentPage = 0;
        if (playerStatistics.SpawnedBoss)
        {
            _pages = WinTextForLevel(level);
            _seenText.Add(new PageDescriptor(true, level));
        }
        else
        {
            _pages = LoseTextForLevel(level);
            _seenText.Add(new PageDescriptor(false, level));
        }
    }

    private string[] LoseTextForLevel(int level)
    {
        Client.Debug.Log($"checking lose text for {level}");
        
        if (_seenText.Contains(new PageDescriptor(false, level)))
        {
            return new[]
            {
                "No good!",
                "Can't let the player die before the boss shows up.",
                "Try again."
            };
        }

        if (level == 0)
        {
            return new[]
            {
                "Hi there...",
                "You must be the new recruit!",
                "Welcome to the Pest Control Arcade Cabinet SKU 4322156-A.",
                "I know it's your first day but...",
                "You can't just go killing The Player like that.",
                "It's all about finding that balance.",
                "Too hard and the game is stressful...",
                "Too easy and the game is boring...",
                "We want that sweet spot, right in the middle.",
                "We call that the \"Flow\" state!",
                "Your job is to keep them in the \"Flow\" state for as long as possible.",
                "Then we drop the Boss on them!",
                "Now THAT's when you start rolling out the big guns, like you did just now.",
                "Gotta make our Coins somehow, right?",
                "Speaking of! There's a Coin coming down the slot...",
                "Showtime!"
            };
        }
        
        if (level == 1)
        {
            return new[]
            {
                "Hey, just so you know...",
                "This is a different Player than last time.",
                "Not every Player will play the same way...",
                "Or respond to the same way to enemies.",
                "You gotta tailor the experience to The Player in front of you.",
                "Go get 'em champ!",
            };
        }


        // default
        return new[]
        {
            "Oops.",
            "Try again!"
        };
    }

    private string[] WinTextForLevel(int level)
    {
        Client.Debug.Log($"checking win text for {level}");

        if (_seenText.Contains(new PageDescriptor(true, level)))
        {
            return new[] {"Nice work."};
        }
        
        if (level == 0)
        {
            return new[]
            {
                "Way to go, rookie!",
                "I think you got it figured out, but just to review...",
                "Keep the player in the Flow state until the boss comes out.",
                "Then slam 'em with everything you got!"
            };
        }
        
        if (level == 1)
        {
            return new[]
            {
                "Way to go!",
                "Keep it up kid."
            };
        }
        
        if (level == 2)
        {
            return new[]
            {
                "Hey, fun fact!",
                "You know how The Player's avatar is at the top of the screen?",
                "The Player actually sees the opposite, they're at the bottom of the screen and you're sending enemies down to them.",
                "You're seeing the whole game upside-down! Isn't that crazy!",
                "You might be thinking \"Then why can I still read \'Game Over\' and other text on screen.\"",
                "....",
                "I... don't know.",
            };
        }
        
        if (level == 3)
        {
            // END OF GAME MESSAGE
            return new[]
            {
                "Hey uhh...",
                "Is it cool if I break character for a moment?",
                "(ahem)",
                "Thanks for playing our GMTK Game Jam entry!",
                "This game was made in 48 Hours by NotExplosive, CreatiFish, The4thAD, and quarkimo.",
                "The theme was \"You Are The Dun--\" I mean \"Role Reversal\"",
                "You can keep playing if you want!",
                "But you've pretty much seen all the content."
            };
        }

        return new[]
        {
            "Nice work!",
        };
    }

    public void BecomeReady()
    {
        _isReady = true;
        PageSound();
    }

    private void PageSound()
    {
        if (IsAtEndOfPages)
        {
            return;
        }
        
        Global.PlaySound($"talk{_soundIndex}", 1f);
        _soundIndex++;
        if (_soundIndex > 4)
        {
            _soundIndex = 1;
        }
    }

    public void Update(float dt)
    {
        _offset -= dt * 60f;
        if (_offset < 0)
        {
            _offset = 0;
        }
    }
}

public record PageDescriptor(bool IsWin, int LevelNumber);
