using UnityEngine;

namespace OMG
{
    public class MoveCommand : BaseCommand
    {
        public Vector2 ViewportStart;
        public Vector2 ViewportEnd;

        public MoveCommand(Vector2 viewportStart, Vector2 viewportEnd)
        {
            ViewportStart = viewportStart;
            ViewportEnd = viewportEnd;
        }
    }
}
