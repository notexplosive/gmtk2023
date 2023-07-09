using System;
using System.Linq;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class World : IUpdateHook
{
    private int _totalScore;
    private float _time;

    public World(Vector2 worldSize, int level)
    {
        Entities = new EntityCollection(this);
        Bounds = new RectangleF(Vector2.Zero, worldSize);
        PlayerStatistics.Level = level;
    }

    public bool QuarterInserted { get; set; }

    public bool IsStarted { get; set; }

    public EntityCollection Entities { get; }
    public RectangleF Bounds { get; }

    public bool IsGameOver { get; private set; }

    public MultiplexTween ActiveTween { get; } = new();
    public PlayerStatistics PlayerStatistics { get; set; } = new();

    public void Update(float dt)
    {
        _time += dt;
        ActiveTween.Update(dt);

        if (ActiveTween.IsDone())
        {
            ActiveTween.Clear();
        }

        if (!IsStarted)
        {
            return;
        }

        PlayerStatistics.UpdateBossMeter(dt, Entities.OfType<EnemyShip>().Any());

        var bullets = Entities.OfType<Bullet>().ToList();
        var shipsThatAreInBounds =
            Entities.OfType<TeamedEntity>().Where(ship => Bounds.Contains(ship.BoundingBox)).ToList();
        var friendlyShips = shipsThatAreInBounds.Where(ship => ship.Team == Team.Player).ToList();
        var enemyShips = shipsThatAreInBounds.Where(ship => ship.Team == Team.Enemy).ToList();
        var powerUps = Entities.OfType<PowerUp>().ToList();
        var enemyShipsTypeSafe = enemyShips.OfType<EnemyShip>().ToList();

        if (enemyShips.Count == 0)
        {
            PlayerStatistics.Intensity -= dt * 20;
        }

        foreach (var bullet in bullets)
        {
            var affectedShips = bullet.Team == Team.Player ? enemyShips : friendlyShips;

            foreach (var ship in affectedShips)
            {
                if (bullet.DealDamageBox.Overlaps(ship.TakeDamageBox))
                {
                    bullet.OnHitTarget();
                    ship.TakeDamageFrom(bullet);
                    break;
                }
            }
        }

        foreach (var enemyShip in enemyShipsTypeSafe)
        {
            foreach (var friendlyShip in friendlyShips)
            {
                if (enemyShip.DealDamageBox.Overlaps(friendlyShip.BoundingBox))
                {
                    friendlyShip.TakeDamage();
                }
            }
        }

        foreach (var powerUp in powerUps)
        {
            foreach (var friend in friendlyShips)
            {
                if (friend is PlayerShip player)
                {
                    if (powerUp.BoundingBox.Overlaps(player.BoundingBox))
                    {
                        player.Equip(powerUp.Type);
                        powerUp.Destroy();
                        break;
                    }
                }
            }
        }
    }

    public void GameOver()
    {
        ActiveTween.AddChannel(
            new SequenceTween()
                .Add(new CallbackTween(()=>Global.MusicPlayer.FadeToInterlude()))
                .Add(new WaitSecondsTween(1))
                .Add(new CallbackTween(()=>Global.PlaySound("gmtk23_gameover")))
                .Add(new WaitSecondsTween(1))
                .Add(new CallbackTween(()=>OnGameOver?.Invoke(PlayerStatistics)))
        );
        IsGameOver = true;
    }

    public event Action<PlayerStatistics>? OnGameOver;

    public void DrawOverlay(Painter painter)
    {
        painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), _totalScore.ToString(), Bounds,
            Alignment.TopRight, new DrawSettings());

        if (!QuarterInserted)
        {
            if (MathF.Sin(_time * 5) > 0)
            {
                painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), "Insert Coin", Bounds,
                    Alignment.Center, new DrawSettings());
            }

            return;
        }
        
        var hpRect = new Rectangle(8, 8, 32, 32);
        
        for (int i = 0; i < PlayerStatistics.Health; i++)
        {
            Global.MainSheet.DrawFrameAsRectangle(painter, 16, hpRect, new DrawSettings());
            hpRect = hpRect.Moved(new Vector2(32, 0));
        }
        
        var bombRect = new Rectangle(8, 8 + 32, 32, 32);
        for (int i = 0; i < PlayerStatistics.Bombs; i++)
        {
            Global.MainSheet.DrawFrameAsRectangle(painter, 19, bombRect, new DrawSettings());
            bombRect = bombRect.Moved(new Vector2(20, 0));
        }

        if (!IsStarted)
        {
            painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), "GAME START", Bounds,
                Alignment.Center, new DrawSettings());
            return;
        }

        if (IsGameOver)
        {
            if (MathF.Sin(_time * 5) > 0)
            {
                painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), "GAME OVER", Bounds,
                    Alignment.Center, new DrawSettings());
            }
        }
    }

    public void ScoreDoober(Vector2 position, int score)
    {
        var vfx = Entities.AddImmediate(new TextVfx(score.ToString()));
        vfx.Position = position;
        _totalScore += score;
    }
    
    public void TextDoober(Vector2 position, string text)
    {
        var vfx = Entities.AddImmediate(new TextVfx(text));
        vfx.Position = position;
    }
}
