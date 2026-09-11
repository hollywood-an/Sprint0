using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprint0.Interfaces;

public interface IPlayer
{
    void Move(Direction direction);
    void MoveTo(Vector2 target);
    void StopMoving();
    void Dash();
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}
