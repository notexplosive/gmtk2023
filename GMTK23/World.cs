using System;
using System.Linq;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace GMTK23;

public class World : IUpdateHook
{
    private int _totalScore;

    public bool QuarterInserted { get; set; }

    public bool IsStarted { get; set; }
    
    public World(Vector2 worldSize)
    {
        Entities = new EntityCollection(this);
        Bounds = new RectangleF(Vector2.Zero, worldSize);
    }

    public EntityCollection Entities { get; }
    public RectangleF Bounds { get; }

    public bool IsGameOver { get; private set; }

    public void Update(float dt)
    {
        var bullets = Entities.OfType<Bullet>().ToList();
        var shipsThatAreInBounds =
            Entities.OfType<TeamedEntity>().Where(ship => Bounds.Contains(ship.BoundingBox)).ToList();
        var friendlyShips = shipsThatAreInBounds.Where(ship => ship.Team == Team.Player).ToList();
        var enemyShips = shipsThatAreInBounds.Where(ship => ship.Team == Team.Enemy).ToList();
        var enemyShipsTypeSafe = enemyShips.OfType<EnemyShip>().ToList();

        foreach (var bullet in bullets)
        {
            var affectedShips = bullet.Team == Team.Player ? enemyShips : friendlyShips;

            foreach (var ship in affectedShips)
            {
                if (bullet.DealDamageBox.Overlaps(ship.TakeDamageBox))
                {
                    bullet.Destroy();
                    ship.TakeDamage();
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
    }

    public void GameOver()
    {
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
}
