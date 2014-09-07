using System;
using ModestTree.Zenject;
using Silentor.TB.Client.Players;
using UnityEngine;

namespace Assets.Code.Visualization.Views
{
    /// <summary>
    /// Unity representation of Player
    /// </summary>
    public class TestPlayerView : MonoBehaviour, IPlayer
    {
        public Transform Head;

        [Inject]
        public IPlayer Player;

        public int Id { get { return Player.Id; }}

        public Vector3 Position { get { return Player.Position; } }

        public Quaternion Rotation { get { return Player.Rotation; }}

        [PostInject]
        public void Init()
        {
            transform.position = Position;
            Head.rotation = Rotation;
            transform.parent = null;

            Player.Moved += () => { transform.position = Position; };
            Player.Rotated += () => { Head.rotation = Rotation; };
        }

        public event Action Moved;
        public event Action Rotated;

        void OnDrawGizmos()
        {

        }


    }
}
