using System;
using System.Linq;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using ExTween;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class World : IUpdateHook
{
    private int _totalScore;

    public World(Vector2 worldSize)
    {
        Entities = new EntityCollection(this);
        Bounds = new RectangleF(Vector2.Zero, worldSize);
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
        ActiveTween.Update(dt);

        if (ActiveTween.IsDone())
        {
            ActiveTween.Clear();
        }

        if (!IsStarted)
        {
            return;
        }

        PlayerStatistics.UpdateBossMeter(dt);

        var bullets = Entities.OfType<Bullet>().ToList();
        var shipsThatAreInBounds =
            Entities.OfType<TeamedEntity>().Where(ship => Bounds.Contains(ship.BoundingBox)).ToList();
        var friendlyShips = shipsThatAreInBounds.Where(ship => ship.Team == Team.Player).ToList();
        var enemyShips = shipsThatAreInBounds.Where(ship => ship.Team == Team.Enemy).ToList();
        var powerUps = Entities.OfType<PowerUp>().ToList();
        var enemyShipsTypeSafe = enemyShips.OfType<EnemyShip>().ToList();

        if (enemyShips.Count == 0)
        {
            PlayerStatistics.Intensity -= dt * 10;
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
                .Add(new WaitSecondsTween(1))
                .Add(new CallbackTween(()=>Global.PlaySound("gmtk23_gameover")))
        );
        IsGameOver = true;
        OnGameOver?.Invoke();
    }

    public event Action? OnGameOver;

    public void DrawOverlay(Painter painter)
    {
        painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), _totalScore.ToString(), Bounds,
            Alignment.TopRight, new DrawSettings());

        if (!QuarterInserted)
        {
            painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), "Insert Coin", Bounds,
                Alignment.Center, new DrawSettings());
            return;
        }

        if (!IsStarted)
        {
            painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), "FOR GREAT JUSTICE", Bounds,
                Alignment.Center, new DrawSettings());
            return;
        }

        if (IsGameOver)
        {
            painter.DrawStringWithinRectangle(Client.Assets.GetFont("gmtk/GameFont", 32), "GAME OVER", Bounds,
                Alignment.Center, new DrawSettings());
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
