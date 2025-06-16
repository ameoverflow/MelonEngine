using System.Drawing;
using System.Numerics;
using Chroma;
using Chroma.Input;
using Chroma.Physics;

namespace MelonEngine;

public class Player : Entity
{
    private float _gravity = 0.04f;

    internal Vector2 Velocity { get; set; } = new Vector2(0, 0);
    //RaycastHit has properties: Vector2 Position - where did raycast hit, Collider Collider - collider which was hit
    //public static bool Raycast.Cast(Vector2 origin, Vector2 direction, out RaycastHit raycastHit, float maxDistance = 500f, string[] skipTags = null)
    public void TryMove(Vector2 change)
    {
        // shoot a raycast in both of velocity axis and check if player can move. if not, reset respective axis
        RaycastHit hit;
        //Raycast.Cast(new Vector2(Position.X, Position.Y), )
    }

    public void FixedUpdate(float delta)
    {
        /*
        _gravity += 0.04f;
        Vector2 _move = TryMove(new Vector2(0, _gravity));
        if (_move != Vector2.Zero)
        {
            Position += new Vector2(0, _gravity);
        }
        else
        {
            _gravity = 0.04f;
        }
        */
    }
}
